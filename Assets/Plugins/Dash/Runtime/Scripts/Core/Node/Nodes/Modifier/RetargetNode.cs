/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    [Help("Changes a current target within NodeFlowData.")]
    [Category(NodeCategoryType.MODIFIER)]
    [Size(180,85)]
    [OutputCount(1)]
    [InputCount(1)]
    public class RetargetNode : RetargetNodeBase<RetargetNodeModel>
    {
        protected override void ExecuteOnTarget(Transform p_target, NodeFlowData p_flowData)
        {
            p_flowData.SetAttribute("target", p_target);
            
            ExecuteEnd(p_flowData);
        }
        
        void ExecuteEnd(NodeFlowData p_flowData)
        {
            OnExecuteEnd();
            OnExecuteOutput(0,p_flowData);
        }
        
        #if UNITY_EDITOR
        internal override Transform ResolveEditorTarget(string p_path = "", int p_outputIndex = 0)
        {
            // If we don't have controller no point in resolving
            if (DashEditorCore.EditorConfig.editingController == null)
                return null;
            
            object target = ResolveRetargetedEditorTarget(p_path, p_outputIndex, false);
            
            if (target is string)
            {
                var connections = Graph.GetInputConnections(this);
                if (connections.Count > 0)
                {
                    return connections[0].outputNode
                        .ResolveEditorTarget(target + "/" + p_path, connections[0].outputIndex);
                }

                return DashEditorCore.EditorConfig.editingController.transform.ResolvePathWithFind(
                    target + "/" + p_path);
            }

            if (target != null)
            {
                return (target as Transform).ResolvePathWithFind(p_path);
            }

            return null;
        }
        #endif
    }
}