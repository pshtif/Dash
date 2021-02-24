/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    [Help("Changes an active state of target.")]
    [Category(NodeCategoryType.MODIFIERS)]
    [OutputCount(1)]
    [InputCount(1)]
    public class SetActiveNode : RetargetNodeBase<SetActiveNodeModel>
    {
        protected override void ExecuteOnTarget(Transform p_target, NodeFlowData p_flowData)
        {
            p_target.gameObject.SetActive(Model.active);
            
            OnExecuteEnd();
            OnExecuteOutput(0, p_flowData);
        }
    }
}