/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    [Help("Stop all or specific target animations.")]
    [Category(NodeCategoryType.ANIMATION)]
    [OutputCount(1)]
    [InputCount(1)]
    [Size(200,85)]
    [Serializable]
    public class StopAnimationsNode : RetargetNodeBase<StopAnimationsNodeModel>
    {
        protected override void ExecuteOnTarget(Transform p_target, NodeFlowData p_flowData)
        {
            if (Model.allAnimations)
            {
                ((IInternalGraphAccess)Graph).StopActiveTweens(null);
            }
            else
            {
                ((IInternalGraphAccess) Graph).StopActiveTweens(p_target);
            }

            OnExecuteEnd();
            OnExecuteOutput(0, p_flowData);
        }
    }
}