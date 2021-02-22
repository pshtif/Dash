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
    [Category(NodeCategoryType.LOGIC)]
    [OutputCount(1)]
    [InputCount(1)]
    public class SubGraphNode : NodeBase<SubGraphNodeModel>
    {
        private DashGraph _instancedGraph;
        
        private int _selfReferenceIndex = -1;
        private byte[] _boundGraphData;
        private List<Object> _boundGraphReferences;

        protected override void Initialize()
        {
            GetGraphInstance();
            if (_instancedGraph != null)
            {
                _instancedGraph.Initialize(Graph.Controller);
                _instancedGraph.OnExit += ExecuteEnd;
            }
        }

        protected override void OnExecuteStart(NodeFlowData p_flowData)
        {
            if (CheckException(_instancedGraph, "There is no graph defined"))
                return;
            
            _instancedGraph.Enter(p_flowData);
        }

        protected void ExecuteEnd(NodeFlowData p_flowData)
        {
            Debug.Log("EndSubgraph: "+_graph);
            OnExecuteEnd();
            OnExecuteOutput(0, p_flowData);
        }
        
        private DashGraph GetGraphInstance()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying && !DashEditorCore.Previewer.IsPreviewing && Model.graphAsset != null)
            {
                return Model.graphAsset;
            }
#endif
            if (_instancedGraph == null)
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
            
            return _instancedGraph;
        }
        
        void InstanceAssetGraph()
        {
            if (Model.graphAsset == null)
                return;
            
            _instancedGraph = Model.graphAsset.Clone();
        }
        
        void InstanceBoundGraph()
        {
            _instancedGraph = ScriptableObject.CreateInstance<DashGraph>();
            
            // Empty graphs don't self reference
            if (_selfReferenceIndex != -1)
            {
                _boundGraphReferences[_selfReferenceIndex] = _instancedGraph;
                _instancedGraph.DeserializeFromBytes(_boundGraphData, DataFormat.Binary, ref _boundGraphReferences);
            }

            ((IInternalGraphAccess)_instancedGraph).parentGraph = Graph;
            _instancedGraph.name = Controller.gameObject.name+"[Bound]";
        }
        
#if UNITY_EDITOR
        public override void DrawInspector()
        {
            GUI.color = new Color(1, 0.75f, 0.5f);
            if (GUILayout.Button("Open Editor", GUILayout.Height(40)))
            {
                Debug.Log("here"+_instancedGraph);
                DashEditorCore.EditController(DashEditorCore.Config.editingGraph.Controller, GetGraphInstance());
            }

            GUI.color = Color.white;

            base.DrawInspector();
        }
#endif
    }
}