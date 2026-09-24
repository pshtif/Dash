/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Dash.Attributes;
using OdinSerializer.Utilities;
using UnityEngine;
using Attribute = System.Attribute;
using Object = System.Object;
#if UNITY_EDITOR
using OdinSerializer;
using UnityEditor;
using Dash.Editor;

#endif

namespace Dash
{
    [Serializable]
    [Size(150,85)]
    [Category(NodeCategoryType.OTHER)]
    public abstract class NodeBase 
    {
        static public NodeBase Create(Type p_nodeType, DashGraph p_graph)
        {
            NodeBase node = (NodeBase)Activator.CreateInstance(p_nodeType);
            node._graph = p_graph;
            node.CreateModel();

            return node;
        }

        [NonSerialized]
        internal Action<NodeConnection> OnConnectionRemoved;

        internal void Stop()
        {
            Stop_Internal();

            KillActiveTweens();

            ExecutionCount = 0;
        }

        protected virtual void Stop_Internal() { }

        // ---- Tween tracking -------------------------------------------------------------------
        // One list per node (all flows), consolidated here from six per-node copies. Each tween is
        // double-booked: on this node (so node-scoped stops can kill it) and on the flow's
        // GraphExecution (so per-flow stops can kill it and prune it from this list).

        [NonSerialized]
        protected List<DashTween> _activeTweens;

        /// <summary>Books a tween on this node and on the flow's execution. Returns it for chaining.</summary>
        protected DashTween TrackTween(DashTween p_tween, NodeFlowData p_flowData)
        {
            if (p_tween == null)
                return null;

            if (_activeTweens == null)
                _activeTweens = new List<DashTween>();

            _activeTweens.Add(p_tween);
            p_flowData?.execution?.TrackTween(this, p_tween);

            return p_tween;
        }

        /// <summary>Removes both bookings; call when a tween completes naturally.</summary>
        protected void UntrackTween(DashTween p_tween, NodeFlowData p_flowData)
        {
            _activeTweens?.Remove(p_tween);
            p_flowData?.execution?.UntrackTween(p_tween);
        }

        // Node-list prune used by GraphExecution when it kills a flow's tweens, so no stale
        // reference outlives the kill (killed tweens are pooled and reused).
        internal void RemoveActiveTween(DashTween p_tween)
        {
            _activeTweens?.Remove(p_tween);
        }

        // Kills every tween on this node regardless of flow. The whole-node stop path.
        private void KillActiveTweens()
        {
            if (_activeTweens == null)
                return;

            _activeTweens.ForEach(t => t.Kill(false));
            _activeTweens.Clear();
        }

        /// <summary>
        /// killOnNullEncounter path: the animated target died mid-tween, so kill this node's tweens
        /// for the CURRENT flow only — concurrent flows animating live targets on this same node
        /// keep running. Falls back to a whole-node kill when the flow has no execution (editor
        /// preview drives AnimateOnTarget directly).
        /// </summary>
        protected void KillFlowTweens(NodeFlowData p_flowData)
        {
            GraphExecution execution = p_flowData?.execution;

            if (execution != null)
                execution.KillTweens(this);
            else
                KillActiveTweens();
        }

        [SerializeField]
        internal NodeModelBase _model;

        public bool HasModel()
        {
            return _model != null;
        }
        
        public NodeModelBase GetModel()
        {
            return _model;
        }
        
        [NonSerialized] 
        public bool hasErrorsInExecution = false;
        
        public string Id => _model.id;

        [SerializeField] 
        internal DashGraph _graph;

        public DashGraph Graph => _graph;

        public DashController Controller => Graph.Controller;

        public int Index => Graph.Nodes.IndexOf(this);

        [NonSerialized]
        protected GraphParameterResolver _parameterResolver;

        public GraphParameterResolver ParameterResolver
        {
            get
            {
                if (_parameterResolver == null) _parameterResolver = new GraphParameterResolver(Graph);

                return _parameterResolver;
            }
        }

        [field:NonSerialized]
        public virtual int ExecutionCount { get; protected set; } = 0;
        

        public virtual bool IsExecuting => ExecutionCount > 0;

        internal virtual void Initialize() { }

        internal virtual void Remove() { }

        public void Execute(NodeFlowData p_flowData)
        {
            // A stopped flow must not run at all — without this gate the increment below would
            // never be matched (OnExecuteEnd bails on stopped executions) and the count would
            // leak. Reachable via callbacks that survive a stop, e.g. a sequencer advancing into
            // a flow stopped while it waited in the queue.
            if (p_flowData?.execution != null && p_flowData.execution.IsStopped)
                return;

            ExecutionCount++;

            // Safety net: any flow that reaches a node without an execution identity is a genuine
            // origin (direct Execute calls from event nodes, third-party hand-built flow data).
            // Mint here so it propagates through the clone below and onward. Origin type is NONE
            // (unknown entry point) but the target is stamped so the flow stays addressable via
            // StopExecutionsByTarget.
            if (p_flowData != null && p_flowData.execution == null && _graph != null)
            {
                p_flowData.execution = _graph.CreateExecution();
                p_flowData.execution.SetOrigin(ExecutionOriginType.NONE, null, p_flowData);
            }

            // Mirror the node-level count onto the owning execution's per-node frame map. The clone
            // handed to OnExecuteStart shares this same execution reference, so the matching
            // OnExecuteEnd(flowData) closes the frame on the same object. Kept in lockstep with
            // ExecutionCount (same increment site, same error guard on the way out).
            if (p_flowData != null)
                p_flowData.execution?.EnterNode(this);

#if UNITY_EDITOR
            if (!HasDebugOverride)
            {
                DashEditorDebug.Debug(new NodeDebugItem(NodeDebugItem.NodeDebugItemType.EXECUTE, Graph.Controller, Graph.GraphPath, _model.id,
                    p_flowData.GetAttribute<Transform>("target")));
            }

            executeTime = 1;
#endif

            // Errors are per-run, not forever: reset the (now purely visual) node error flag so a
            // past error cannot poison this fresh run — the historic never-resets latch is gone.
            hasErrorsInExecution = false;

            NodeFlowData flowData = p_flowData == null ? new NodeFlowData() : p_flowData.Clone();

            // Capture the flow for the duration of the synchronous node body so SetError can fail
            // its execution without every call site threading flow data through. Save/restore
            // handles re-entrancy (a body triggering this same node again via outputs). Async
            // continuations (code after an await, tween callbacks) run after the restore and fall
            // back to flag-only error marking — acceptable, teardown gates still apply.
            NodeFlowData previousFlow = _currentFlowData;
            _currentFlowData = flowData;

            OnExecuteStart(flowData);

            _currentFlowData = previousFlow;
        }

        [NonSerialized]
        private NodeFlowData _currentFlowData;

        protected abstract void OnExecuteStart(NodeFlowData p_flowData);

        protected void OnExecuteOutput(int p_index, NodeFlowData p_flowData)
        {
            // A stopped or failed flow does not propagate: its async work was killed and its
            // frames released. (SetError fails the execution directly, so no node-flag check —
            // the old node-level check was the never-resets latch that blocked healthy flows.)
            GraphExecution execution = p_flowData?.execution;
            if (execution != null && execution.IsStopped)
                return;

            Graph.ExecuteNodeOutputs(this, p_index, p_flowData);
        }

        protected void OnExecuteEnd(NodeFlowData p_flowData)
        {
            // A stopped/failed execution already released every open frame in
            // GraphExecution.Stop(), so skip accounting here to avoid double-closing this node's
            // frame and count.
            GraphExecution execution = p_flowData?.execution;
            if (execution != null && execution.IsStopped)
                return;

            ExecutionCount--;
            execution?.ExitNode(this);
        }

        // Releases p_count open frames for this node without the normal per-node end path. Called
        // by GraphExecution.Stop() when a flow is torn down mid-flight and the frames' OnExecuteEnd
        // will never fire, keeping ExecutionCount (editor viz / idle detection) honest.
        internal void ReleaseExecutionFrames(int p_count)
        {
            ExecutionCount -= p_count;
            if (ExecutionCount < 0)
            {
#if UNITY_EDITOR
                // A negative count means the execution frame map and ExecutionCount desynced —
                // the exact bug this accounting exists to prevent. Surface it, don't hide it.
                Debug.LogError("ExecutionCount went negative on node " + this + " (released " + p_count + " frames)");
#endif
                ExecutionCount = 0;
            }
        }

        // Legacy overload for third-party nodes compiled against the old signature. It can only
        // touch the node-level count — with no flow data it cannot attribute the frame to an
        // execution, so such nodes will not participate in per-execution stop/teardown. All
        // built-in nodes call OnExecuteEnd(NodeFlowData).
        [Obsolete("Use OnExecuteEnd(NodeFlowData p_flowData) so the frame can be attributed to its execution.")]
        protected void OnExecuteEnd()
        {
            if (!hasErrorsInExecution)
            {
                ExecutionCount--;
            }
        }

        public virtual bool IsSynchronous()
        {
            return true;
        } 

        protected abstract void CreateModel();
        
        protected bool CheckException(NodeFlowData p_flowData, string p_variableName)
        {
            if (!p_flowData.HasAttribute(p_variableName))
            {
                SetError("Attribute "+p_variableName+" not found during execution of "+this);

                return true;
            }

            return false;
        }

        protected bool CheckException(Object p_object, string p_warning = null)
        {
            if (p_object == null || (p_object is UnityEngine.Object && (UnityEngine.Object)p_object == null))
            {
                SetError(p_warning);
                
                return true;
            }

            return false;
        }

        protected void SetError(string p_message = null)
        {
            if (!string.IsNullOrEmpty(p_message))
            {
                DashCore.Instance.OnError?.Invoke(p_message);
                Debug.LogWarning(p_message+" on node: " + _model.id+ " in graph: "+Graph+" running controller: "+Controller);
                #if UNITY_EDITOR
                DashEditorDebug.Debug(new ErrorDebugItem(p_message));
                #endif
            }
            else
            {
                p_message = "Unknown error occured.";
                DashCore.Instance.OnError?.Invoke(p_message);
                Debug.LogWarning(p_message+" on node: " + _model.id+ " in graph: "+Graph+" running controller: "+Controller);
            }
            hasErrorsInExecution = true;

            // Errors are execution-scoped: fail the flow that hit them. Full teardown (frames
            // released — including sites that SetError-and-return without OnExecuteEnd, which used
            // to leak their frame forever), remaining branches halt, disposables run, and
            // OnComplete fires with HasErrors set. Concurrent flows through this node and future
            // runs are untouched.
            _currentFlowData?.execution?.Fail();
        }
        
        protected void ValidateUniqueId()
        {
            string id = _model.id;
            if (string.IsNullOrEmpty(id))
            {
                string type = GetType().ToString();
                int dotIndex = type.IndexOf(".");
                id = type.Substring(dotIndex + 1, type.Length - (dotIndex + 5)) + "1";
            }

            while (Graph.Nodes.Exists(n => n != this && n.Id == id))
            {
                string number = string.Concat(id.Reverse().TakeWhile(char.IsNumber).Reverse());
                id = id.Substring(0, id.Length - number.Length) + (Int32.Parse(number) + 1);
            }
            
            _model.id = id;
        }
        
        protected virtual void Invalidate() { }
        
        protected object GetUntypedParameterValue(Parameter p_parameter, NodeFlowData p_flowData = null, int p_index = 0)
        {
            if (p_parameter == null)
                return null;
            
            object value = p_parameter.GetUntypedValue(ParameterResolver, p_flowData, p_index);
            if (!hasErrorsInExecution && p_parameter.hasErrorInEvaluation)
            {
                SetError(p_parameter.errorMessage);
            }
            hasErrorsInExecution = hasErrorsInExecution || p_parameter.hasErrorInEvaluation;
            return value;
        }
        
        public T GetParameterValue<T>(Parameter<T> p_parameter, NodeFlowData p_flowData)
        {
            if (p_parameter == null)
                return default(T);

            T value = p_parameter.GetValue(ParameterResolver, p_flowData);
            if (!hasErrorsInExecution && p_parameter.hasErrorInEvaluation)
            {
                SetError(p_parameter.errorMessage);
            }
            
            hasErrorsInExecution = hasErrorsInExecution || p_parameter.hasErrorInEvaluation;
            return value;
        }
        
        /// <summary>
        /// Attributes section
        /// </summary>

        #region ATTRIBUTES

        [NonSerialized]
        private bool _attributesInitialized = false;
        
        [NonSerialized] 
        private bool _isObsolete;
        public bool IsObsolete
        {
            get
            {
                if (!_attributesInitialized)
                    InitializeAttributes();

                return _isObsolete;
            }
        }
        
        [NonSerialized] 
        private bool _isExperimental;
        public bool IsExperimental
        {
            get
            {
                if (!_attributesInitialized)
                    InitializeAttributes();

                return _isExperimental;
            }
        }

        [NonSerialized] 
        private int _inputCount;
        public virtual int InputCount
        {
            get
            {
                if (!_attributesInitialized)
                    InitializeAttributes();

                return _inputCount;
            }
        }

        [NonSerialized] 
        private int _outputCount;
        public virtual int OutputCount
        {
            get
            {
                if (!_attributesInitialized)
                    InitializeAttributes();

                return _outputCount;
            }
        }
        
#if UNITY_EDITOR
        
        [NonSerialized] 
        private bool _hasDebugOverride;
        public bool HasDebugOverride
        {
            get
            {
                if (!_attributesInitialized)
                    InitializeAttributes();

                return _hasDebugOverride;
            }
        }
        
        [NonSerialized] 
        private string[] _inputLabels;
        public virtual string[] InputLabels
        {
            get
            {
                if (!_attributesInitialized)
                    InitializeAttributes();
        
                return _inputLabels;
            }
        }

        [NonSerialized] 
        private string[] _outputLabels;
        public virtual string[] OutputLabels
        {
            get
            {
                if (!_attributesInitialized)
                    InitializeAttributes();
        
                return _outputLabels;
            }
        }
        
        [NonSerialized]
        private string _titleSkinId;
        
        public string TitleSkinId
        {
            get
            {
                if (!_attributesInitialized)
                    InitializeAttributes();

                return _titleSkinId;
            }
        }

        [NonSerialized] 
        private string _backgroundSkinId;
        
        public string BackgroundSkinId
        {
            get
            {
                if (!_attributesInitialized)
                    InitializeAttributes();

                return _backgroundSkinId;
            }
        }

        [NonSerialized]
        private Vector2 _size;
        
        public virtual Vector2 Size
        {
            get
            {
                if (!_attributesInitialized)
                    InitializeAttributes();

                return _size;
            }
        }

        [NonSerialized]
        private bool _baseGUIEnabled;
        
        protected bool BaseGUIEnabled
        {
            get
            {
                if (!_attributesInitialized)
                    InitializeAttributes();

                return _baseGUIEnabled;
            }
        }
#endif
        
        protected virtual void InitializeAttributes()
        {
            Type nodeType = GetType();

            _isObsolete = Attribute.GetCustomAttribute(nodeType, typeof(ObsoleteAttribute)) != null;
            
            _isExperimental = Attribute.GetCustomAttribute(nodeType, typeof(ExperimentalAttribute)) != null;
            
            InputCountAttribute inputCountAttribute = (InputCountAttribute) Attribute.GetCustomAttribute(nodeType, typeof(InputCountAttribute));
            _inputCount = inputCountAttribute == null ? 0 : inputCountAttribute.count;

            OutputCountAttribute outputCountAttribute = (OutputCountAttribute) Attribute.GetCustomAttribute(nodeType, typeof(OutputCountAttribute));
            _outputCount = outputCountAttribute == null ? 0 : outputCountAttribute.count;

#if UNITY_EDITOR
            
            _hasDebugOverride = Attribute.GetCustomAttribute(nodeType, typeof(DebugOverrideAttribute), true) != null;
            
            InputLabelsAttribute inputAttribute = (InputLabelsAttribute) Attribute.GetCustomAttribute(nodeType, typeof(InputLabelsAttribute));
            _inputLabels = inputAttribute == null ? new string[0] : inputAttribute.labels;
            
            OutputLabelsAttribute outputAttribute = (OutputLabelsAttribute) Attribute.GetCustomAttribute(nodeType, typeof(OutputLabelsAttribute));
            _outputLabels = outputAttribute == null ? new string[0] : outputAttribute.labels;
            
            SkinAttribute skinAttribute = (SkinAttribute) Attribute.GetCustomAttribute(nodeType, typeof(SkinAttribute));
            _backgroundSkinId = skinAttribute != null ? skinAttribute.backgroundSkinId : "NodeBodyBg";
            _titleSkinId = skinAttribute != null ? skinAttribute.titleSkinId : "NodeTitleBg";
            
            SizeAttribute sizeAttribute = (SizeAttribute) Attribute.GetCustomAttribute(nodeType, typeof(SizeAttribute));
            _size = sizeAttribute != null ? new Vector2(sizeAttribute.width, sizeAttribute.height) : Vector2.one;
            
            DisableBaseGUIAttribute disableBaseGuiAttribute = (DisableBaseGUIAttribute) Attribute.GetCustomAttribute(nodeType, typeof(DisableBaseGUIAttribute));
            _baseGUIEnabled = disableBaseGuiAttribute == null;

            CategoryAttribute categoryAttribute = (CategoryAttribute) Attribute.GetCustomAttribute(nodeType, typeof(CategoryAttribute));
            Category = categoryAttribute.type;
            
#endif

            _attributesInitialized = true;
        }
        #endregion

        #region EDITOR_CODE
#if UNITY_EDITOR
        
        [field:NonSerialized]
        public NodeCategoryType Category { get; private set; } 
        
        [NonSerialized] 
        public float executeTime = 0;

        public Rect rect;

        public virtual string CustomName => String.Empty;

        public string Name => String.IsNullOrEmpty(CustomName)
            ? GetNodeNameFromType(this.GetType())
            : CustomName;
        
        public static string GetNodeNameFromType(Type p_nodeType)
        {
            string typeString = p_nodeType.ToString();
            int dotIndex = typeString.LastIndexOf(".");
            return typeString.Substring(dotIndex + 1, typeString.Length - (dotIndex + 5));
        }

        internal virtual void Unselect() { }

        public virtual NodeBase Clone(DashGraph p_graph, IExposedPropertyTable p_propertyTable)
        {
            NodeBase node = Create(GetType(), p_graph);
            node._model = _model.Clone(p_propertyTable);
            node.ValidateUniqueId();
            return node;
        }

        public virtual void DrawGUI(Rect p_rect)
        {
            GUISkin skin = DashEditorCore.Skin;
            rect = new Rect(rect.x, rect.y, Size.x, Size.y);

            Rect offsetRect = new Rect(rect.x + Graph.viewOffset.x, rect.y + Graph.viewOffset.y, Size.x, Size.y);
            
            if (!p_rect.Contains(new Vector2(offsetRect.x, offsetRect.y)) &&
                !p_rect.Contains(new Vector2(offsetRect.x + offsetRect.width, offsetRect.y)) &&
                !p_rect.Contains(new Vector2(offsetRect.x, offsetRect.y + offsetRect.height)) &&
                !p_rect.Contains(new Vector2(offsetRect.x + offsetRect.width, offsetRect.y + offsetRect.height)))
                return;

            if (BaseGUIEnabled)
            {
                GUI.color = DashEditorCore.EditorConfig.theme.GetNodeBackgroundColorByCategory(Category);

                if (!IsSynchronous() && Graph.zoom < 2.5 && DashEditorCore.EditorConfig.showNodeAsynchronity)
                {
                    GUI.DrawTexture(
                        new Rect(offsetRect.x + offsetRect.width - 24, offsetRect.y - 20, 20, 20),
                        IconManager.GetIcon("time_icon"));
                }

                GUI.Box(offsetRect, "", skin.GetStyle(BackgroundSkinId));

                DrawTitle(offsetRect);
            }

            if (Graph.zoom < 2.5)
            {
                DrawCustomGUI(offsetRect);
                
                DrawId(offsetRect);
            }

            DrawOutline(offsetRect);
            
            DrawConnectors(p_rect);
        }

        public bool HasComment()
        {
            return _model.comment != null;
        }

        public void CreateComment()
        {
            _model.comment = "Comment";
        }

        public void RemoveComment()
        {
            _model.comment = null;
        }
        
        public void DrawComment(Rect p_rect, bool p_zoomed = true)
        {
            if (_model.comment.IsNullOrWhitespace())
                return;

            Rect offsetRect = p_zoomed
                ? new Rect(rect.x + Graph.viewOffset.x, rect.y + Graph.viewOffset.y, Size.x, Size.y)
                : new Rect((rect.x + Graph.viewOffset.x) / Graph.zoom, (rect.y + Graph.viewOffset.y) / Graph.zoom, Size.x, Size.y);
            
            GUIStyle commentStyle = new GUIStyle();
            commentStyle.font = DashEditorCore.Skin.GetStyle("NodeComment").font;
            commentStyle.fontSize = 14;
            commentStyle.normal.textColor = Color.black;

            string commentText = _model.comment;
            Vector2 size = commentStyle.CalcSize( new GUIContent( commentText ) );

            GUI.color = DashEditorCore.EditorConfig.theme.CommentColor;
            GUI.Box(new Rect(offsetRect.x - 10, offsetRect.y - size.y - 26, size.x < 34 ? 50 : size.x + 16, size.y + 26), "", DashEditorCore.Skin.GetStyle("NodeComment"));
            GUI.color = Color.white;
            string text = GUI.TextArea(new Rect(offsetRect.x - 2, offsetRect.y - size.y - 21, size.x, size.y), commentText, commentStyle);
            _model.comment = text;
        }
        
        void DrawTitle(Rect p_rect)
        {
            GUI.color = DashEditorCore.EditorConfig.theme.GetNodeTitleTextColorByCategory(Category);
            
            Texture iconTexture =  DashEditorCore.EditorConfig.theme.GetNodeIconByCategory(Category);

            GUI.Label(
                new Rect(new Vector2(p_rect.x + (iconTexture != null ? 26 : 6), p_rect.y),
                    new Vector2(100, 20)), Name, DashEditorCore.Skin.GetStyle("NodeTitle"));
                
            if (iconTexture != null)
            {
                GUI.DrawTexture(new Rect(p_rect.x + 6, p_rect.y + 4, 16, 16),
                    iconTexture);
            }

            if (IsExperimental)
            {
                GUI.color = DashEditorCore.EditorConfig.theme.ExperimentalColor;
                GUI.DrawTexture(new Rect(p_rect.x + rect.width - 21, p_rect.y + 4, 16, 16),
                    IconManager.GetIcon("experimental_icon"));
                GUI.color = Color.white;
            }
            
            if (IsObsolete)
            {
                GUI.color = DashEditorCore.EditorConfig.theme.ObsoleteColor;
                GUI.DrawTexture(new Rect(p_rect.x + 4, p_rect.y - 20, 16, 16),
                    IconManager.GetIcon("prohibited_icon"));
                GUI.color = Color.white;
            }

            GUI.color = Color.white;
        }

        void DrawId(Rect p_rect)
        {
            GUI.color = Color.gray;
            if (!String.IsNullOrEmpty(_model.id))
            {
                if (DashEditorCore.EditorConfig.showNodeIds)
                {
                    GUI.Label(
                        new Rect(new Vector2(p_rect.x, p_rect.y + p_rect.height - 4), new Vector2(rect.width - 5, 20)), _model.id);
                }
            }
            GUI.color = Color.white;
        }

        void DrawOutline(Rect p_rect)
        {
            if (SelectionManager.IsSelected(Graph.Nodes.IndexOf(this)))
            {
                GUI.color = Color.green;
                GUI.Box(new Rect(p_rect.x - 2, p_rect.y - 2, p_rect.width + 4, p_rect.height + 4),
                    "",  DashEditorCore.Skin.GetStyle("NodeSelected"));
            }
                
            if (IsExecuting || executeTime > 0)
            {
                if (!IsExecuting)
                {
                    executeTime -= .2f;
                }
                GUI.color = Color.cyan;
                GUI.Box(new Rect(p_rect.x - 2, p_rect.y - 2, p_rect.width + 4, p_rect.height + 4),
                    "",  DashEditorCore.Skin.GetStyle("NodeSelected"));
            }
            
            if (SelectionManager.IsSelecting(Graph.Nodes.IndexOf(this)))
            {
                GUI.color = Color.yellow;
                GUI.Box(new Rect(p_rect.x - 2, p_rect.y - 2, p_rect.width + 4, p_rect.height + 4),
                    "",  DashEditorCore.Skin.GetStyle("NodeSelected"));
            }
                
            if (hasErrorsInExecution)
            {
                GUI.color = Color.red;
                GUI.Box(new Rect(p_rect.x - 2, p_rect.y - 2, p_rect.width + 4, p_rect.height + 4),
                    "",  DashEditorCore.Skin.GetStyle("NodeSelected"));
                GUI.DrawTexture(new Rect(p_rect.x + 2, p_rect.y - 22, 16, 16),
                    IconManager.GetIcon("error_icon"));
            }

            int labelOffset = 0;
            if (this is InputNode)
            {
                InputNode node = this as InputNode;
                if (DashEditorCore.EditorConfig.editingController != null &&
                    DashEditorCore.EditorConfig.editingController.bindStart &&
                    DashEditorCore.EditorConfig.editingController.bindStartInput == node.Model.inputName) 
                {
                    GUI.color = Color.white;
                    GUIStyle style = new GUIStyle();
                    style.normal.textColor = Color.green;
                    style.fontStyle = FontStyle.Bold;
                    style.fontSize = 20;
                    style.alignment = TextAnchor.UpperCenter;
                    GUI.Label(new Rect(p_rect.x, p_rect.y + rect.height, rect.width, 20), "[START]", style);
                    labelOffset++;
                }
            }
            
            if (this is InputNode)
            {
                InputNode node = this as InputNode;
                if (DashEditorCore.EditorConfig.editingController != null &&
                    DashEditorCore.EditorConfig.editingController.bindOnEnable &&
                    DashEditorCore.EditorConfig.editingController.bindOnEnableInput == node.Model.inputName) 
                {
                    GUI.color = Color.white;
                    GUIStyle style = new GUIStyle();
                    style.normal.textColor = Color.green;
                    style.fontStyle = FontStyle.Bold;
                    style.fontSize = 20;
                    style.alignment = TextAnchor.UpperCenter;
                    GUI.Label(new Rect(p_rect.x, p_rect.y + rect.height + labelOffset * 25, rect.width, 20), "[ONENABLE]", style);
                    labelOffset++;
                }
            }

            if (Graph.previewNode == this)
            {
                GUI.color = Color.white;
                GUIStyle style = new GUIStyle();
                style.normal.textColor = new Color(1, .5f, 0);
                style.fontStyle = FontStyle.Bold;
                style.fontSize = 20;
                style.alignment = TextAnchor.UpperCenter;
                GUI.Label(new Rect(p_rect.x, p_rect.y + rect.height + labelOffset * 25, rect.width, 20), "[PREVIEW]", style);
            }

            GUI.color = Color.white;
        }

        public virtual Rect GetConnectorRect(NodeConnectorType p_connectorType, int p_index)
        {
            Rect offsetRect = new Rect(rect.x + Graph.viewOffset.x, rect.y + Graph.viewOffset.y, Size.x,
                Size.y);

            Rect connectorRect;
            if (p_connectorType == NodeConnectorType.INPUT)
            {
                connectorRect = new Rect(offsetRect.x,
                    offsetRect.y + DashEditorCore.EditorConfig.theme.TitleTabHeight +
                    p_index * (DashEditorCore.EditorConfig.theme.ConnectorHeight + DashEditorCore.EditorConfig.theme.ConnectorPadding), 24,
                    DashEditorCore.EditorConfig.theme.ConnectorHeight);
            }
            else
            {
                connectorRect = new Rect(offsetRect.x + offsetRect.width - 24,
                    offsetRect.y + DashEditorCore.EditorConfig.theme.TitleTabHeight +
                    p_index * (DashEditorCore.EditorConfig.theme.ConnectorHeight + DashEditorCore.EditorConfig.theme.ConnectorPadding), 24,
                    DashEditorCore.EditorConfig.theme.ConnectorHeight);
            }

            return connectorRect;
        }

        internal bool HitsConnector(Vector2 p_position, out NodeConnectorType p_connectorType, out int p_connectorIndex)
        {
            p_connectorIndex = -1;

            p_connectorType = NodeConnectorType.OUTPUT;
            for (int i = 0; i < OutputCount; i++)
            {
                var connectorRect = GetConnectorRect(p_connectorType, i);
                if (connectorRect.Contains(p_position))
                {
                    p_connectorIndex = i;
                    return true;
                }
            }
            
            p_connectorType = NodeConnectorType.INPUT;
            for (int i = 0; i < InputCount; i++)
            {
                var connectorRect = GetConnectorRect(p_connectorType, i);
                if (connectorRect.Contains(p_position))
                {
                    p_connectorIndex = i;
                    return true;
                }
            }

            return false;
        }

        protected virtual void DrawConnectors(Rect p_rect)
        {
            GUISkin skin = DashEditorCore.Skin;

            // Inputs
            int count = InputCount;
            for (int i = 0; i < count; i++)
            {
                bool isConnected = Graph.HasInputConnected(this, i);
                GUI.color = isConnected
                    ? DashEditorCore.EditorConfig.theme.ConnectorInputConnectedColor
                    : DashEditorCore.EditorConfig.theme.ConnectorInputDisconnectedColor;

                if (IsExecuting)
                    GUI.color = Color.cyan;

                var connectorRect = GetConnectorRect(NodeConnectorType.INPUT, i);
                
                GUI.Label(connectorRect, "", skin.GetStyle(isConnected ? "NodeConnectorOn" : "NodeConnectorOff"));
            }
            
            // Outputs
            for (int i = 0; i < OutputCount; i++)
            {
                bool isConnected = Graph.HasOutputConnected(this, i);
                GUI.color = isConnected
                    ? DashEditorCore.EditorConfig.theme.ConnectorOutputConnectedColor
                    : DashEditorCore.EditorConfig.theme.ConnectorOutputDisconnectedColor;

                if (SelectionManager.connectingNode == this && SelectionManager.connectingType == NodeConnectorType.OUTPUT && SelectionManager.connectingIndex == i)
                    GUI.color = Color.green;

                var connectorRect = GetConnectorRect(NodeConnectorType.OUTPUT, i);
                
                if (connectorRect.Contains(Event.current.mousePosition - new Vector2(p_rect.x, p_rect.y)))
                    GUI.color = Color.green;

                if (OutputLabels != null && OutputLabels.Length > i && Graph.zoom < 2.5)
                {
                    GUIStyle style = new GUIStyle();
                    style.normal.textColor = Color.white;
                    style.alignment = TextAnchor.MiddleRight;
                    GUI.Label(new Rect(connectorRect.x - 100, connectorRect.y, 100, 20), OutputLabels[i], style);
                }

                GUI.Label(connectorRect, "", skin.GetStyle(isConnected ? "NodeConnectorOn" : "NodeConnectorOff"));
            }

            GUI.color = Color.white;
        }
        
        protected virtual void DrawCustomGUI(Rect p_rect) { }

        internal void GetCustomContextMenu(ref RuntimeGenericMenu p_menu)
        {
            if (this is INodeMigratable)
            {
                p_menu.AddItem(new GUIContent("Migrate to "+((INodeMigratable)this).GetMigrateType().Name), false, () => ((INodeMigratable)this).Migrate());
            }
        }
        
        protected virtual void AddCustomContextMenu(ref RuntimeGenericMenu p_menu) { }

        public virtual void DrawInspector(IViewOwner p_owner)
        {
            bool invalidate = _model.DrawInspector(p_owner);
            
            if (invalidate)
            {
                ValidateUniqueId();
                Invalidate();
                Graph.MarkDirty();
                //DashEditorCore.SetDirty();
            }
        }

        public virtual void DrawInspectorControls(IViewOwner p_owner, Rect p_rect) { }

        public List<string> GetModelExposedGUIDs()
        {
            return _model.GetExposedGUIDs();
        }
        
        public List<string> GetModelExposedNodeIDs(List<PropertyName> p_properties)
        {
            return _model.GetExposedNodeIDs(p_properties);
        }

        public virtual void SelectEditorTarget(DashController p_controller) { }

        internal virtual Transform ResolveNodeRetarget(Transform p_transform, NodeConnection p_connection)
        {
            return p_transform;
        }

        public void DrawSceneGUI()
        {
            Handles.BeginGUI();

            DrawCustomSceneGUI();
            
            Handles.EndGUI();
        } 
        
        internal virtual void DrawCustomSceneGUI() { }

        public byte[] SerializeToBytes(DataFormat p_format, ref List<UnityEngine.Object> p_references)
        {
            byte[] bytes = null;
            
            using (var cachedContext = OdinSerializer.Utilities.Cache<SerializationContext>.Claim())
            {
                cachedContext.Value.Config.SerializationPolicy = SerializationPolicies.Everything;
                bytes = OdinSerializer.SerializationUtility.SerializeValue(this, p_format, out p_references, cachedContext.Value);
            }
            
            return bytes;
        }
        
#endif

        #endregion
    }
    
    [Serializable]
    public abstract class NodeBase<T> : NodeBase where T : NodeModelBase, new()
    {
        public T Model
        {
            get
            {
                if (_model == null || !typeof(T).IsAssignableFrom(_model.GetType()))
                {
                    Debug.LogWarning("Model recreated on node "+this+" possibly due to serialization error.");
                    CreateModel();
                }

                return (T) _model;
            }
        }

        protected override void CreateModel()
        {
            _model = new T();
            ValidateUniqueId();
            Invalidate();
        }
    }
}
