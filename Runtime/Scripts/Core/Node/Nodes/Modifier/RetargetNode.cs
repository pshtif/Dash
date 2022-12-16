/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    [Attributes.Tooltip("Changes a current target within NodeFlowData.")]
    [Category(NodeCategoryType.MODIFIER, "Modifier/Retarget")]
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
        internal override Transform ResolveNodeRetarget(Transform p_transform, NodeConnection p_connection)
        {
            // Hack where we resend null connection to may it think it is the last one
            return base.ResolveNodeRetarget(p_transform, null);
        }
        #endif
    }
}