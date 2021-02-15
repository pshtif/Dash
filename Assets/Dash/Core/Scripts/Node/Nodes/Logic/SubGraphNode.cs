/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    [Category(NodeCategoryType.HIDDEN)]
    [OutputCount(1)]
    [InputCount(1)]
    public class SubGraphNode : NodeBase<SubGraphNodeModel>
    {
        private DashGraph _subGraph;
        
        protected override void Initialize()
        {
            if (Model.graph != null)
            {
                _subGraph = GetInstance(Model.graph);
                _subGraph.Initialize(Graph.Controller);
                _subGraph.OnExit += ExecuteEnd;
            }
        }

        protected override void OnExecuteStart(NodeFlowData p_flowData)
        {
            if (CheckException(_subGraph, "There is no graph defined on node "+_model.id))
                return;
            
            _subGraph.Enter(p_flowData);
        }

        protected void ExecuteEnd(NodeFlowData p_flowData)
        {
            Debug.Log("EndSubgraph: "+_graph);
            OnExecuteEnd();
            OnExecuteOutput(0, p_flowData);
        }
        
        public DashGraph GetInstance(DashGraph p_original)
        {
#if UNITY_EDITOR
            // If we are in editor mode return the original instance, or if its null obviously
            if ((!Application.isPlaying && !DashEditorCore.Previewer.IsPreviewing) || p_original == null)
            {
                return p_original;
            }

            if (!Application.isPlaying)
            {
                return p_original.Clone();
            }
#endif

            // Cache directly into field, maybe use lookup later
            if (_subGraph == null)
            {
                _subGraph = p_original.Clone();
            }

            return _subGraph;
        }
    }
}