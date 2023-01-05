/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    [Attributes.Tooltip("Animate target (and children) to a specified state.")]
    [Category(NodeCategoryType.ANIMATION)]
    [OutputCount(1)]
    [InputCount(1)]
    [Size(220,85)]
    [Serializable]
    public class AnimateToStateNode : AnimationNodeBase<AnimateToStateNodeModel>
    {
        protected override DashTween AnimateOnTarget(Transform p_target, NodeFlowData p_flowData)
        {
            TransformStorage fromState = GetParameterValue<TransformStorage>(Model.fromState, p_flowData);
            
            EaseType easeType = GetParameterValue(Model.easeType, p_flowData);
            
            float time = GetParameterValue(Model.time, p_flowData);
            float delay = GetParameterValue(Model.delay, p_flowData);
            if (time == 0)
            {
                //UpdateTween(p_target, 1, p_flowData, startPosition, finalPosition, easeType);
                return null;
            }

            return null;
            // DashTween.To(p_target, 0, 1, time).SetDelay(delay).OnUpdate(f => UpdateTween(p_target, f, p_flowData, startPosition, finalPosition, easeType));
        }

        protected void UpdateTween(Transform p_target, float p_delta, NodeFlowData p_flowData, Vector3 p_startPosition, Vector3 p_finalPosition, EaseType p_easeType)
        {
            if (p_target == null)
            {
                if (Model.killOnNullEncounter)
                    Stop_Internal();
                return;
            }

            p_target.position = new Vector3(DashTween.EaseValue(p_startPosition.x, p_finalPosition.x, p_delta, p_easeType),
                DashTween.EaseValue(p_startPosition.y, p_finalPosition.y, p_delta, p_easeType),
                DashTween.EaseValue(p_startPosition.z, p_finalPosition.z, p_delta, p_easeType));
        }
    }
}
