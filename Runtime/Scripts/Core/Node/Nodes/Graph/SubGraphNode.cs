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
                return SubGraph != null ? SubGraph.GetAllNodesByType<InputNode>().Count : 0;
            }
        }
        
        public override int OutputCount
        {
            get
            {
                return SubGraph != null ? SubGraph.GetAllNodesByType<OutputNode>().Count : 0;
            }
        }
        
        public DashGraph SubGraph => GetSubGraphInstance();

        protected override void Initialize()
        {
            if (SubGraph != null)
            {
                SubGraph.Initialize(Graph.Controller);
                SubGraph.OnOutput += (node, data) => ExecuteEnd(Graph.GetOutputIndex(node), data);
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
            
            SubGraph.GetNodeByType<InputNode>()?.Execute(p_flowData);
        }

        protected void ExecuteEnd(int p_outputIndex, NodeFlowData p_flowData)
        {
            OnExecuteEnd();
            OnExecuteOutput(0, p_flowData);
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
            ((IInternalGraphAccess)_subGraphInstance).SetParentGraph(Graph);
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

            ((IInternalGraphAccess)_subGraphInstance).SetParentGraph(Graph);
            _subGraphInstance.isBound = true;
            _subGraphInstance.name = Model.id;
        }
        
        public void ReserializeBound()
        {
            Debug.Log("SubGraphNode.ReserializeBound");
            if (_subGraphInstance != null)
            {
                _boundSubGraphData = _subGraphInstance.SerializeToBytes(DataFormat.Binary, ref _boundSubGraphReferences);
                _selfReferenceIndex = _boundSubGraphReferences.FindIndex(r => r == _subGraphInstance);
            }
        }

#if UNITY_EDITOR

        public override Vector2 Size => new Vector2(150, 85 + (InputCount > OutputCount
                ? (InputCount > 2 ? (InputCount - 2) * 25 : 0)
                : (OutputCount > 2 ? (OutputCount - 2) * 25 : 0)));
        
        public override void DrawInspector()
        {
            GUI.color = new Color(1, 0.75f, 0.5f);
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

            GUI.color = Color.white;
            
            if (!Model.useAsset)
            {
                if (GUILayout.Button("Save to Asset"))
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
                if (GUILayout.Button("Bind Graph"))
                {
                    DashGraph graph = SubGraph.Clone();
                    _boundSubGraphData = graph.SerializeToBytes(DataFormat.Binary, ref _boundSubGraphReferences);
                    _selfReferenceIndex = _boundSubGraphReferences.FindIndex(r => r == graph);

                    Model.useAsset = false;
                    Model.graphAsset = null;
                }
            }

            GUI.color = Color.white;

            base.DrawInspector();
        }
        
        protected override void DrawCustomGUI(Rect p_rect)
        {
            base.DrawCustomGUI(p_rect);

            if (Model.useAsset)
            {
                var style = new GUIStyle(DashEditorCore.Skin.GetStyle("NodeText"));
                style.alignment = TextAnchor.MiddleCenter;
                style.normal.textColor = Color.cyan;
                string name = Model.graphAsset != null ? Model.graphAsset.name : "NONE"; 
                GUI.Label(new Rect(p_rect.x, p_rect.y + p_rect.height - 32, p_rect.width, 20), name, style);
            }
        }
#endif
    }
}