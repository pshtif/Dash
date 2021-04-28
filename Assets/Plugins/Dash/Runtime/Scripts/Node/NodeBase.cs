/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Dash.Attributes;
using OdinSerializer;
using OdinSerializer.Utilities;
using UnityEngine;
using Object = System.Object;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Dash
{
    [Serializable]
    [Size(150,85)]
    [Category(NodeCategoryType.OTHER)]
    public abstract class NodeBase : INodeAccess
    {
        static public NodeBase Create(Type p_nodeType, DashGraph p_graph)
        {
            NodeBase node = (NodeBase)Activator.CreateInstance(p_nodeType);
            node._graph = p_graph;
            node.CreateModel();

            return node;
        }
        
        [SerializeField]
        protected NodeModelBase _model;

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
        protected DashGraph _graph;

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

        public virtual int ExecutionCount { get; protected set; } = 0;
        

        public virtual bool IsExecuting => ExecutionCount > 0;
        
        

        [NonSerialized]
        private bool _attributesInitialized = false;
        
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

        protected virtual void Initialize() { }
        
        void INodeAccess.Initialize() => Initialize();

        protected virtual void Remove() { }
        
        void INodeAccess.Remove() => Remove();

        public void Execute(NodeFlowData p_flowData)
        {
            ExecutionCount++;
            
#if UNITY_EDITOR
            executeTime = 1;
#endif

            OnExecuteStart(p_flowData == null ? new NodeFlowData() : p_flowData.Clone());
        }

        protected abstract void OnExecuteStart(NodeFlowData p_flowData);

        protected void OnExecuteOutput(int p_index, NodeFlowData p_flowData)
        {
            if (!hasErrorsInExecution)
            {
                Graph.ExecuteNodeOutputs(this, p_index, p_flowData);
            }

            //_outputConnections[p_index].ForEach(c => c.inputNode.Execute(p_flowData));
        }
        
        protected void OnExecuteEnd()
        {
            if (!hasErrorsInExecution)
            {
                ExecutionCount--;
            }
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
            if (p_object == null)
            {
                SetError(p_warning);
                
                return true;
            }

            return false;
        }

        protected void SetError(string p_warning = null)
        {
            if (!string.IsNullOrEmpty(p_warning))
            {
                Debug.LogWarning(p_warning+" on node " + _model.id);
            }
            hasErrorsInExecution = true;
        }
        
        protected T GetParameterValue<T>(Parameter<T> p_parameter, NodeFlowData p_flowData)
        {
            T value = p_parameter.GetValue(ParameterResolver, p_flowData);
            if (!hasErrorsInExecution && p_parameter.hasErrorInEvaluation)
            {
                SetError(p_parameter.errorMessage);
            }
            hasErrorsInExecution = hasErrorsInExecution || p_parameter.hasErrorInEvaluation;
            return value;
        }
        
        private void InitializeAttributes()
        {
            _isExperimental = Attribute.GetCustomAttribute(GetType(), typeof(ExperimentalAttribute)) != null;
            
            InputCountAttribute inputCountAttribute = (InputCountAttribute) Attribute.GetCustomAttribute(GetType(), typeof(InputCountAttribute));
            _inputCount = inputCountAttribute == null ? 0 : inputCountAttribute.count;
            
            InputLabelsAttribute inputAttribute = (InputLabelsAttribute) Attribute.GetCustomAttribute(GetType(), typeof(InputLabelsAttribute));
            _inputLabels = inputAttribute == null ? new string[0] : inputAttribute.labels;
            
            OutputCountAttribute outputCountAttribute = (OutputCountAttribute) Attribute.GetCustomAttribute(GetType(), typeof(OutputCountAttribute));
            _outputCount = outputCountAttribute == null ? 0 : outputCountAttribute.count;
            
            OutputLabelsAttribute outputAttribute = (OutputLabelsAttribute) Attribute.GetCustomAttribute(GetType(), typeof(OutputLabelsAttribute));
            _outputLabels = outputAttribute == null ? new string[0] : outputAttribute.labels;
            
            #if UNITY_EDITOR
            
            SkinAttribute skinAttribute = (SkinAttribute) Attribute.GetCustomAttribute(GetType(), typeof(SkinAttribute));
            _backgroundSkinId = skinAttribute != null ? skinAttribute.backgroundSkinId : "NodeBodyBg";
            _titleSkinId = skinAttribute != null ? skinAttribute.titleSkinId : "NodeTitleBg";
            
            SizeAttribute sizeAttribute = (SizeAttribute) Attribute.GetCustomAttribute(GetType(), typeof(SizeAttribute));
            _size = sizeAttribute != null ? new Vector2(sizeAttribute.width, sizeAttribute.height) : Vector2.one;
            
            DisableBaseGUIAttribute disableBaseGuiAttribute = (DisableBaseGUIAttribute) Attribute.GetCustomAttribute(GetType(), typeof(DisableBaseGUIAttribute));
            _baseGUIEnabled = disableBaseGuiAttribute == null;
            
            CategoryAttribute categoryAttribute = (CategoryAttribute) Attribute.GetCustomAttribute(GetType(), typeof(CategoryAttribute));
            
            IconAttribute iconAttribute = (IconAttribute) Attribute.GetCustomAttribute(GetType(), typeof(IconAttribute));
            _iconTexture = iconAttribute != null ? IconManager.GetIcon(iconAttribute.iconId) : DashEditorCore.GetNodeIconByCategory(categoryAttribute.type);
            
            _nodeBackgroundColor = DashEditorCore.GetNodeBackgroundColorByCategory(categoryAttribute.type);
            
            _titleBackgroundColor = DashEditorCore.GetNodeTitleBackgroundColorByCategory(categoryAttribute.type);
            
            _titleTextColor = DashEditorCore.GetNodeTitleTextColorByCategory(categoryAttribute.type);

            #endif

            _attributesInitialized = true;
        }

        protected void ValidateUniqueId()
        {
            string id = _model.id;
            if (string.IsNullOrEmpty(id))
            {
                string type = GetType().ToString();
                id = type.Substring(5, type.Length-9) + "1";
            }

            while (Graph.Nodes.Exists(n => n != this && n.Id == id))
            {
                string number = string.Concat(id.Reverse().TakeWhile(char.IsNumber).Reverse());
                id = id.Substring(0,id.Length-number.Length) + (Int32.Parse(number)+1);
            }

            _model.id = id;
        }

        #region EDITOR_CODE
#if UNITY_EDITOR

        [NonSerialized]
        public float executeTime = 0;

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

        [NonSerialized] 
        private Texture _iconTexture;
        
        protected Texture IconTexture
        {
            get
            {
                if (!_attributesInitialized)
                    InitializeAttributes();

                return _iconTexture;
            }
        }

        [NonSerialized] 
        private Color _nodeBackgroundColor;
        
        // Using virtual getters so you can override it and avoid serialization at the same time and it is not
        // possible to use Attributes for this due to non constant initializer
        protected virtual Color NodeBackgroundColor
        {
            get
            {
                if (!_attributesInitialized)
                    InitializeAttributes();

                return _nodeBackgroundColor;
            }
        }

        [NonSerialized]
        private Color _titleBackgroundColor;

        protected virtual Color TitleBackgroundColor
        {
            get
            {
                if (!_attributesInitialized)
                    InitializeAttributes();

                return _titleBackgroundColor;
            }
        }

        [NonSerialized] 
        private Color _titleTextColor;

        protected virtual Color TitleTextColor
        {
            get
            {
                if (!_attributesInitialized)
                    InitializeAttributes();
                
                return _titleTextColor;
            }
        }
        
        public bool IsSelected => Graph.IsSelected(this);
        
        public bool IsSelecting => Graph.IsSelecting(this);
        
        public Rect rect;

        public virtual string CustomName => String.Empty;

        public string Name => String.IsNullOrEmpty(CustomName)
            ? GetNodeNameFromType(this.GetType())
            : CustomName;
        
        public static string GetNodeNameFromType(Type p_nodeType)
        {
            string typeString = p_nodeType.ToString();
            return typeString.ToString().Substring(5, typeString.ToString().Length - 9);
        }

        protected virtual void Invalidate() { }

        public void ValidateSerialization()
        {
            _model.ValidateSerialization();
        }
        
        public NodeBase Clone(DashGraph p_graph)
        {
            NodeBase node = Create(GetType(), p_graph);
            node._model = _model.Clone();
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
                GUI.color = NodeBackgroundColor;
                GUI.Box(offsetRect, "", skin.GetStyle(BackgroundSkinId));

                DrawTitle(offsetRect);

                DrawId(offsetRect);
            }

            DrawOutline(offsetRect);

            if (DashEditorCore.DetailsVisible) 
                DrawCustomGUI(offsetRect);
            
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
            if (_model.comment == null)
                return;

            Rect offsetRect = p_zoomed
                ? new Rect(rect.x + Graph.viewOffset.x, rect.y + Graph.viewOffset.y, Size.x, Size.y)
                : new Rect((rect.x + Graph.viewOffset.x) / DashEditorCore.Config.zoom, (rect.y + Graph.viewOffset.y) / DashEditorCore.Config.zoom, Size.x, Size.y);
            
            GUIStyle commentStyle = new GUIStyle();
            commentStyle.font = DashEditorCore.Skin.GetStyle("NodeComment").font;
            commentStyle.fontSize = 14;
            commentStyle.normal.textColor = Color.black;

            string commentText = _model.comment;
            Vector2 size = commentStyle.CalcSize( new GUIContent( commentText ) );
            
            GUI.color = new Color(1,1,1,.6f);
            GUI.Box(new Rect(offsetRect.x - 10, offsetRect.y - size.y - 26, size.x < 34 ? 50 : size.x + 16, size.y + 26), "", DashEditorCore.Skin.GetStyle("NodeComment"));
            GUI.color = Color.white;
            string text = GUI.TextArea(new Rect(offsetRect.x - 2, offsetRect.y - size.y - 21, size.x, size.y), commentText, commentStyle);
            _model.comment = text;
        }
        
        void DrawTitle(Rect p_rect)
        {
            GUI.color = TitleTextColor;
            
            GUI.Label(
                new Rect(new Vector2(p_rect.x + (IconTexture != null ? 26 : 6), p_rect.y),
                    new Vector2(100, 20)), Name, DashEditorCore.Skin.GetStyle("NodeTitle"));
                
            if (IconTexture != null)
            {
                GUI.color = TitleTextColor;
                GUI.DrawTexture(new Rect(p_rect.x + 6, p_rect.y + 4, 16, 16),
                    IconTexture);
            }

            if (IsExperimental)
            {
                GUI.color = Color.yellow;
                GUI.DrawTexture(new Rect(p_rect.x + rect.width - 21, p_rect.y + 4, 16, 16),
                    IconManager.GetIcon("Experimental_Icon"));
                GUI.color = Color.white;
            }

            GUI.color = Color.white;
        }

        void DrawId(Rect p_rect)
        {
            GUI.color = Color.gray;
            if (!String.IsNullOrEmpty(_model.id))
            {
                if (DashEditorCore.Config.showNodeIds)
                {
                    GUI.Label(
                        new Rect(new Vector2(p_rect.x, p_rect.y - 20), new Vector2(rect.width - 5, 20)), _model.id);
                }
            }
            else
            {
                ValidateUniqueId();
            }
            GUI.color = Color.white;
        }

        void DrawOutline(Rect p_rect)
        {
            if (IsSelected)
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
            
            if (IsSelecting)
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
                    IconManager.GetIcon("Error_Icon"));
            }

            if (Graph.previewNode == this)
            {
                GUI.color = Color.white;
                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.magenta;
                style.fontStyle = FontStyle.Bold;
                style.fontSize = 20;
                style.alignment = TextAnchor.UpperCenter;
                GUI.Label(new Rect(p_rect.x, p_rect.y + rect.height, rect.width, 20), "[PREVIEW]", style);
            }
            
            GUI.color = Color.white;
        }

        public Rect GetConnectorRect(bool p_input, int p_index)
        {
            Rect offsetRect = new Rect(rect.x + Graph.viewOffset.x, rect.y + Graph.viewOffset.y, Size.x,
                Size.y);

            Rect connectorRect;
            if (p_input)
            {
                connectorRect = new Rect(offsetRect.x,
                    offsetRect.y + DashEditorCore.TITLE_TAB_HEIGHT +
                    p_index * (DashEditorCore.CONNECTOR_HEIGHT + DashEditorCore.CONNECTOR_PADDING), 24,
                    DashEditorCore.CONNECTOR_HEIGHT);
            }
            else
            {
                connectorRect = new Rect(offsetRect.x + offsetRect.width - 24,
                    offsetRect.y + DashEditorCore.TITLE_TAB_HEIGHT +
                    p_index * (DashEditorCore.CONNECTOR_HEIGHT + DashEditorCore.CONNECTOR_PADDING), 24,
                    DashEditorCore.CONNECTOR_HEIGHT);
            }

            return connectorRect;
        }
        
        private void DrawConnectors(Rect p_rect)
        {
            GUISkin skin = DashEditorCore.Skin;

            // Inputs
            for (int i = 0; i < InputCount; i++)
            {
                bool isConnected = Graph.HasInputConnected(this, i);
                GUI.color = isConnected ? DashEditorCore.CONNECTOR_INPUT_CONNECTED_COLOR
                    : DashEditorCore.CONNECTOR_INPUT_DISCONNECTED_COLOR;

                if (IsExecuting)
                    GUI.color = Color.cyan;

                var connectorRect = GetConnectorRect(true, i);
                
                if (GUI.Button(connectorRect, "", skin.GetStyle(isConnected ? "NodeConnectorOn" : "NodeConnectorOff")))
                {
                    if (Event.current.button == 0)
                    {
                        if (Graph.connectingNode != null && Graph.connectingNode != this)
                        {
                            Undo.RegisterCompleteObjectUndo(_graph, "Connect node");
                            Graph.Connect(this, i, Graph.connectingNode, Graph.connectingOutputIndex);
                            Graph.connectingNode = null;
                        }
                    }
                }
            }
            
            // Outputs
            for (int i = 0; i < OutputCount; i++)
            {
                bool isConnected = Graph.HasOutputConnected(this, i); 
                GUI.color = isConnected ? DashEditorCore.CONNECTOR_OUTPUT_CONNECTED_COLOR
                    : DashEditorCore.CONNECTOR_OUTPUT_DISCONNECTED_COLOR;

                if (Graph.connectingNode == this && Graph.connectingOutputIndex == i)
                    GUI.color = Color.green;

                var connectorRect = GetConnectorRect(false, i);
                
                if (connectorRect.Contains(Event.current.mousePosition - new Vector2(p_rect.x, p_rect.y)))
                    GUI.color = Color.green;

                if (OutputLabels != null && OutputLabels.Length > i && DashEditorCore.DetailsVisible)
                {
                    GUIStyle style = new GUIStyle();
                    style.normal.textColor = Color.white;
                    style.alignment = TextAnchor.MiddleRight;
                    GUI.Label(new Rect(connectorRect.x - 100, connectorRect.y, 100, 20), OutputLabels[i], style);
                }

                if (GUI.Button(connectorRect, "", skin.GetStyle(isConnected ? "NodeConnectorOn" : "NodeConnectorOff")))
                {
                    Graph.connectingOutputIndex = i;
                    Graph.connectingNode = this;
                }
            }

            GUI.color = Color.white;
        }
        
        protected virtual void DrawCustomGUI(Rect p_rect) { }

        public virtual void DrawInspector()
        {
            bool invalidate = _model.DrawInspector();
            
            if (invalidate)
            {
                ValidateUniqueId();
                Invalidate();
                DashEditorCore.SetDirty();
            }
        }

        public List<string> GetModelExposedGUIDs()
        {
            return _model.GetExposedGUIDs();
        }

        public bool IsInsideRect(Rect p_rect)
        {
            if (p_rect.Contains(new Vector2((rect.x + Graph.viewOffset.x)/DashEditorCore.Config.zoom,
                    (rect.y + Graph.viewOffset.y)/DashEditorCore.Config.zoom)) ||
                p_rect.Contains(new Vector2((rect.x + rect.width + Graph.viewOffset.x)/DashEditorCore.Config.zoom,
                    (rect.y + Graph.viewOffset.y)/DashEditorCore.Config.zoom)) ||
                p_rect.Contains(new Vector2((rect.x + Graph.viewOffset.x)/DashEditorCore.Config.zoom,
                    (rect.y + rect.height + Graph.viewOffset.y)/DashEditorCore.Config.zoom)) ||
                p_rect.Contains(new Vector2((rect.x + rect.width + Graph.viewOffset.x)/DashEditorCore.Config.zoom,
                    (rect.y + rect.height + Graph.viewOffset.y)/DashEditorCore.Config.zoom)))
            {
                return true;
            }

            return false;
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
        }
    }
}
