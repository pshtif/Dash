/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    [Attributes.Tooltip("Animate RectTransform size delta.")]
    [Category(NodeCategoryType.ANIMATION)]
    [OutputCount(1)]
    [InputCount(1)]
    [Size(180,85)]
    [Serializable]
    public class AnimateSizeDeltaNode : AnimationNodeBase<AnimateSizeDeltaNodeModel>
    {
        protected override DashTween AnimateOnTarget(Transform p_target, NodeFlowData p_flowData)
        {
            RectTransform rectTransform = p_target.GetComponent<RectTransform>();

            if (CheckException(rectTransform, "Target doesn't contain RectTransform"))
                return null;

            Vector2 fromSizeDelta = GetParameterValue(Model.fromSizeDelta, p_flowData);
            
            Vector2 startSizeDelta = Model.useFrom 
                ? Model.isFromRelative 
                    ? rectTransform.sizeDelta + fromSizeDelta
                    : fromSizeDelta 
                : rectTransform.sizeDelta;
            
            if (Model.storeToAttribute)
            {
                string attribute = GetParameterValue(Model.storeAttributeName, p_flowData);
                p_flowData.SetAttribute<Vector2>(attribute, rectTransform.sizeDelta);
            }
            
            Vector2 toSizeDelta = GetParameterValue(Model.toSizeDelta, p_flowData);

            float time = GetParameterValue(Model.time, p_flowData);
            float delay = GetParameterValue(Model.delay, p_flowData);
            EaseType easeType = GetParameterValue(Model.easeType, p_flowData);
            
            if (time == 0)
            {
                UpdateTween(rectTransform, 1, p_flowData, startSizeDelta, toSizeDelta, easeType);

                return null;
            }
            else
            {
                // Virtual tween to update from directly
                return DashTween.To(rectTransform, 0, 1, time).SetDelay(delay)
                    .OnUpdate(f => UpdateTween(rectTransform, f, p_flowData, startSizeDelta, toSizeDelta, easeType));
            }
        }

        protected void UpdateTween(RectTransform p_target, float p_delta, NodeFlowData p_flowData, Vector2 p_startSizeDelta, Vector2 p_toSizeDelta, EaseType p_easeType)
        {
            if (p_target == null)
            {
                if (Model.killOnNullEncounter)
                    Stop_Internal();
                return;
            }

            if (Model.isToRelative)
            {
                p_target.sizeDelta =
                    p_startSizeDelta + new Vector2(DashTween.EaseValue(0, p_toSizeDelta.x, p_delta, p_easeType),
                        DashTween.EaseValue(0, p_toSizeDelta.y, p_delta, p_easeType));
            }
            else
            {
                p_target.sizeDelta = new Vector2(DashTween.EaseValue(p_startSizeDelta.x, p_toSizeDelta.x, p_delta, p_easeType),
                    DashTween.EaseValue(p_startSizeDelta.y, p_toSizeDelta.y, p_delta, p_easeType));
            }
        }
    }
}
