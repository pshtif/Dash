/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    [Help("Changes a current target within NodeFlowData.")]
    [Category(NodeCategoryType.MODIFIERS)]
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
    }
}