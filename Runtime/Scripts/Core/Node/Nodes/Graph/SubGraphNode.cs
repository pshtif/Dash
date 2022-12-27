/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using Dash.Attributes;
using OdinSerializer;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Dash
{
    [Experimental]
    [Category(NodeCategoryType.GRAPH)]
    public class SubGraphNode : NodeBase<SubGraphNodeModel>
    {
        [NonSerialized]
        private DashGraph _subGraphInstance;

        private int _selfReferenceIndex = -1;
        private byte[] _boundSubGraphData;
        private List<Object> _boundSubGraphReferences;

        public override int ExecutionCount
        {
            get
            {
                return SubGraph != null ? SubGraph.CurrentExecutionCount : 0;
            }
        }

        public override int InputCount
        {
            get
            {
                return SubGraph != null ? SubGraph.GetNodesByType<InputNode>().Count : 0;
            }
        }
        
        public override int OutputCount
        {
            get
            {
                return SubGraph != null ? SubGraph.GetNodesByType<OutputNode>().Count : 0;
            }
        }
        
        public DashGraph SubGraph => GetSubGraphInstance();

        protected override void Initialize()
        {
            if (SubGraph != null)
            {
                SubGraph.Initialize(Graph.Controller);
                SubGraph.OnOutput += (node, data) => ExecuteEnd(SubGraph.GetOutputIndex(node), data);
            }
        }

        protected override void Stop_Internal()
        {
            SubGraph.Stop();
        }

        protected override void OnExecuteStart(NodeFlowData p_flowData)
        {
            if (CheckException(SubGraph, "There is no graph defined"))
                return;
            
            var inputs = SubGraph.GetNodesByType<InputNode>();
            if (inputs.Count > p_flowData.inputIndex)
            {
                AddAttributes(p_flowData);
                inputs[p_flowData.inputIndex].Execute(p_flowData);
            }
        }

        protected void AddAttributes(NodeFlowData p_flowData)
        {
            if (Model.attributes != null)
            {
                foreach (var attribute in Model.attributes)
                {
                    string attributeName = GetParameterValue(attribute.name, p_flowData);
                    if (!p_flowData.HasAttribute(attributeName) ||
                        !attribute.specifyType ||
                        p_flowData.GetAttributeType(attributeName) == attribute.type ||
                        DashCore.Instance.Config.allowAttributeTypeChange) 
                    {
                        var expression = GetParameterValue(attribute.expression, p_flowData);
                        object value;
                        if (attribute.specifyType)
                        {
                            value = ExpressionEvaluator.EvaluateTypedExpression(expression, attribute.type,
                                ParameterResolver, p_flowData);
                        }
                        else
                        {
                            value = ExpressionEvaluator.EvaluateUntypedExpression(expression, ParameterResolver,
                                p_flowData, false);
                        }
                        Debug.Log(attributeName+" : "+value);
                        if (ExpressionEvaluator.hasErrorInEvaluation)
                        {
                            Debug.LogError(ExpressionEvaluator.errorMessage);
                        }
                    
                        p_flowData.SetAttribute(attributeName, value);
                    }
                    else
                    {
                        Debug.LogWarning("Changing flow data attribute type at runtime not allowed for attribute: "+attributeName);
                    }
                }
            }
        }

        protected void ExecuteEnd(int p_outputIndex, NodeFlowData p_flowData)
        {
            OnExecuteEnd();
            OnExecuteOutput(p_outputIndex, p_flowData);
        }
        
        public DashGraph GetSubGraphInstance()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying && !DashEditorCore.Previewer.IsPreviewing && Model.graphAsset != null)
            {
                return Model.graphAsset;
            }
#endif
            if (_subGraphInstance == null)
            {
                if (Model.useAsset && Model.graphAsset != null)
                {
                    InstanceAssetGraph();
                }
                else 
                {
                    InstanceBoundGraph();
                }
            }
            
            return _subGraphInstance;
        }
        
        void InstanceAssetGraph()
        {
            if (Model.graphAsset == null)
                return;
            
            _subGraphInstance = Model.graphAsset.Clone();
            _subGraphInstance.SetParentGraph(Graph);
            _subGraphInstance.name = Model.id;
        }
        
        void InstanceBoundGraph()
        {
            _subGraphInstance = ScriptableObject.CreateInstance<DashGraph>();
            
            // Empty graphs don't self reference
            if (_selfReferenceIndex != -1)
            {
                _boundSubGraphReferences[_selfReferenceIndex] = _subGraphInstance;
                _subGraphInstance.DeserializeFromBytes(_boundSubGraphData, DataFormat.Binary, ref _boundSubGraphReferences);
            }

            _subGraphInstance.SetParentGraph(Graph);
            //_subGraphInstance.isBound = true;
            _subGraphInstance.name = Model.id;
        }
        
        public void ReserializeBound()
        {
            if (_subGraphInstance != null)
            {
                _boundSubGraphData = _subGraphInstance.SerializeToBytes(DataFormat.Binary, ref _boundSubGraphReferences);
                _selfReferenceIndex = _boundSubGraphReferences.FindIndex(r => r == _subGraphInstance);
            }
        }

#if UNITY_EDITOR
        
        public override string CustomName => Model.useAsset && Model.graphAsset != null ? SubGraph.name : "SubGraph";

        public float CustomNameSize => DashEditorCore.Skin.GetStyle("NodeTitle").CalcSize(new GUIContent(CustomName)).x;

        public override Vector2 Size => new Vector2(
            CustomNameSize < 115
                ? 150
                : CustomNameSize + 35, 85 +
            (InputCount > OutputCount
                ? (InputCount > 2 ? (InputCount - 2) * 28 : 0)
                : (OutputCount > 2 ? (OutputCount - 2) * 28 : 0)));
        
        public override void DrawInspector()
        {
            GUILayout.Space(6);
            GUI.color = DashEditorCore.EditorConfig.theme.InspectorButtonColor;
            if (!Model.useAsset || Model.graphAsset != null)
            {
                if (GUILayout.Button("Open Editor", GUILayout.Height(40)))
                {
                    if (DashEditorCore.EditorConfig.editingController != null)
                    {
                        DashEditorCore.EditController(DashEditorCore.EditorConfig.editingController,
                            GraphUtils.AddChildPath(DashEditorCore.EditorConfig.editingGraphPath, Model.id));
                    }
                    else
                    {
                        DashEditorCore.EditGraph(DashEditorCore.EditorConfig.editingRootGraph,
                            GraphUtils.AddChildPath(DashEditorCore.EditorConfig.editingGraphPath, Model.id));
                    }
                }
            }

            GUI.color = Color.white;
            
            if (!Model.useAsset)
            {
                if (Model.graphAsset != null)
                {
                    Model.graphAsset = null;
                }
                
                if (GUILayout.Button("Save to Asset", GUILayout.Height(24)))
                {
                    DashGraph graph = GraphUtils.CreateGraphAsAssetFile(SubGraph);
                    if (graph != null)
                    {
                        Model.useAsset = true;
                        Model.graphAsset = graph;

                        _subGraphInstance = null;
                        _selfReferenceIndex = -1;
                        _boundSubGraphData = null;
                        _boundSubGraphReferences.Clear();
                    }
                }
            }
            else
            {
                if (_boundSubGraphData != null)
                {
                    _subGraphInstance = null;
                    _selfReferenceIndex = -1;
                    _boundSubGraphData = null;
                    _boundSubGraphReferences.Clear();
                }
                
                if (Model.graphAsset != null)
                {
                    if (GUILayout.Button("Bind Graph"))
                    {
                        DashGraph graph = SubGraph.Clone();
                        _boundSubGraphData = graph.SerializeToBytes(DataFormat.Binary, ref _boundSubGraphReferences);
                        _selfReferenceIndex = _boundSubGraphReferences.FindIndex(r => r == graph);

                        Model.useAsset = false;
                        Model.graphAsset = null;
                        
                        InstanceBoundGraph();
                    }
                }
            }

            GUI.color = Color.white;

            base.DrawInspector();
        }
        
        protected override void DrawCustomGUI(Rect p_rect)
        {
            base.DrawCustomGUI(p_rect);

            var inputs = SubGraph.GetNodesByType<InputNode>();
            for (int i = 0; i < inputs.Count; i++)
            {
                GUI.Label(new Rect(p_rect.x + 20, p_rect.y + 26 + 28 * i, p_rect.width-10, 20), inputs[i].Model.inputName);
            }

            var style = new GUIStyle("label");
            style.alignment = TextAnchor.MiddleRight;
            
            var outputs = SubGraph.GetNodesByType<OutputNode>();
            for (int i = 0; i < outputs.Count; i++)
            {
                GUI.Label(new Rect(p_rect.x + 20, p_rect.y + 26 + 28 * i, p_rect.width-40, 20), outputs[i].Model.outputName, style);
            }
        }
#endif
    }
}