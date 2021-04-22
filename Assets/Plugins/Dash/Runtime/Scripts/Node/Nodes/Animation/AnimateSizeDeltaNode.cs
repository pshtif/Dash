/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using Dash.Attributes;
using DG.Tweening;
using UnityEngine;

namespace Dash
{
    [Help("Animate RectTransform size delta.")]
    [Category(NodeCategoryType.ANIMATION)]
    [OutputCount(1)]
    [InputCount(1)]
    [Size(180,85)]
    [Serializable]
    public class AnimateSizeDeltaNode : AnimationNodeBase<AnimateSizeDeltaNodeModel>
    {
        protected override Tween AnimateOnTarget(Transform p_target, NodeFlowData p_flowData)
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
            
            Vector2 toSizeDelta = GetParameterValue(Model.toSizeDelta, p_flowData);

            float time = GetParameterValue(Model.time, p_flowData);
            float delay = GetParameterValue(Model.delay, p_flowData);
            Ease easing = GetParameterValue(Model.easing, p_flowData);
            
            if (time == 0)
            {
                UpdateTween(rectTransform, 1, p_flowData, startSizeDelta, toSizeDelta, easing);

                return null;
            }
            else
            {
                // Virtual tween to update from directly
                Tween tween = DOTween
                    .To((f) => UpdateTween(rectTransform, f, p_flowData, startSizeDelta, toSizeDelta, easing), 0,
                        1, time)
                    .SetDelay(delay)
                    .SetEase(Ease.Linear);

                return tween;
            }
        }

        protected void UpdateTween(RectTransform p_target, float p_delta, NodeFlowData p_flowData, Vector2 p_startSizeDelta, Vector2 p_toSizeDelta, Ease p_easing)
        {
            if (p_target == null)
                return;

            if (Model.isToRelative)
            {
                p_target.sizeDelta =
                    p_startSizeDelta + new Vector2(DOVirtual.EasedValue(0, p_toSizeDelta.x, p_delta, p_easing),
                        DOVirtual.EasedValue(0, p_toSizeDelta.y, p_delta, p_easing));
            }
            else
            {
                p_target.sizeDelta = new Vector2(DOVirtual.EasedValue(p_startSizeDelta.x, p_toSizeDelta.x, p_delta, p_easing),
                    DOVirtual.EasedValue(p_startSizeDelta.y, p_toSizeDelta.y, p_delta, p_easing));
            }
        }
    }
}
