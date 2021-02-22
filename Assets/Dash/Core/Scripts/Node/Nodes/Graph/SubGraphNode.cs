/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System.Collections.Generic;
using Dash.Attributes;
using OdinSerializer;
using UnityEngine;

namespace Dash
{
    [Experimental]
    [Category(NodeCategoryType.GRAPH)]
    [OutputCount(1)]
    [InputCount(1)]
    public class SubGraphNode : NodeBase<SubGraphNodeModel>
    {
        private DashGraph _instancedSubGraph;
        
        private int _selfReferenceIndex = -1;
        private byte[] _boundSubGraphData;
        private List<Object> _boundSubGraphReferences;

        protected override void Initialize()
        {
            GetSubGraphInstance();
            if (_instancedSubGraph != null)
            {
                _instancedSubGraph.Initialize(Graph.Controller);
                _instancedSubGraph.OnOutput += (node, data) => ExecuteEnd(Graph.GetOutputIndex(node), data);
            }
        }

        protected override void OnExecuteStart(NodeFlowData p_flowData)
        {
            if (CheckException(_instancedSubGraph, "There is no graph defined"))
                return;
            
            _instancedSubGraph.GetNodeByType<InputNode>()?.Execute(p_flowData);
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
            if (_instancedSubGraph == null)
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
            
            return _instancedSubGraph;
        }
        
        void InstanceAssetGraph()
        {
            if (Model.graphAsset == null)
                return;
            
            _instancedSubGraph = Model.graphAsset.Clone();
        }
        
        void InstanceBoundGraph()
        {
            _instancedSubGraph = ScriptableObject.CreateInstance<DashGraph>();
            
            // Empty graphs don't self reference
            if (_selfReferenceIndex != -1)
            {
                _boundSubGraphReferences[_selfReferenceIndex] = _instancedSubGraph;
                _instancedSubGraph.DeserializeFromBytes(_boundSubGraphData, DataFormat.Binary, ref _boundSubGraphReferences);
            }

            ((IInternalGraphAccess)_instancedSubGraph).parentGraph = Graph;
            _instancedSubGraph.name = Graph.name+"/"+Model.id+"[Bound]";
        }
        
#if UNITY_EDITOR
        public override void DrawInspector()
        {
            GUI.color = new Color(1, 0.75f, 0.5f);
            if (GUILayout.Button("Open Editor", GUILayout.Height(40)))
            {
                DashEditorCore.EditController(DashEditorCore.Config.editingGraph.Controller, GraphUtils.AddChildPath(DashEditorCore.Config.editingGraphPath, Model.id));
            }

            GUI.color = Color.white;

            base.DrawInspector();
        }
#endif
    }
}