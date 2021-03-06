﻿/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using Dash.Attributes;
using DG.Tweening;
using UnityEngine;

namespace Dash
{
    [Help("Animate RectTransform position on current target.")]
    [Category(NodeCategoryType.ANIMATION)]
    [OutputCount(1)]
    [InputCount(1)]
    [Size(220,85)]
    [Serializable]
    public class AnimateAnchoredPositionNode : AnimationNodeBase<AnimateAnchoredPositionNodeModel>
    {
        protected override void ExecuteOnTarget(Transform p_target, NodeFlowData p_flowData)
        {
            RectTransform rectTransform = p_target.GetComponent<RectTransform>();

            if (CheckException(rectTransform, "No RectTransform component found on target"))
                return;

            Vector2 fromPosition = GetParameterValue<Vector2>(Model.fromPosition, p_flowData);

            Vector2 startPosition = Model.useFrom 
                ? Model.isFromRelative 
                    ? rectTransform.anchoredPosition + fromPosition 
                    : fromPosition 
                : rectTransform.anchoredPosition;

            Vector2 finalPosition = GetParameterValue<Vector2>(Model.toPosition, p_flowData);

            float time = GetParameterValue(Model.time);
            float delay = GetParameterValue(Model.delay);
            if (time == 0)
            {
                UpdateTween(rectTransform, 1, p_flowData, startPosition, finalPosition);
                ExecuteEnd(p_flowData);
            }
            else
            {
                // Virtual tween to update from directly
                Tween tween = DOTween
                    .To((f) => UpdateTween(rectTransform, f, p_flowData, startPosition, finalPosition), 0,
                        1, time)
                    .SetDelay(delay)
                    .SetEase(Ease.Linear)
                    .OnComplete(() => ExecuteEnd(p_flowData));

                DOPreview.StartPreview(tween);
            }
        }
        
        void ExecuteEnd(NodeFlowData p_flowData)
        {
            OnExecuteEnd();
            OnExecuteOutput(0,p_flowData);
        }

        protected void UpdateTween(RectTransform p_target, float p_delta, NodeFlowData p_flowData, Vector2 p_startPosition, Vector2 p_finalPosition)
        {
            if (p_target == null)
                return;

            if (Model.isToRelative)
            {
                p_target.anchoredPosition =
                    p_startPosition + new Vector2(DOVirtual.EasedValue(0, p_finalPosition.x, p_delta, Model.easing),
                        DOVirtual.EasedValue(0, p_finalPosition.y, p_delta, Model.easing));
            }
            else
            {
                p_target.anchoredPosition = new Vector2(DOVirtual.EasedValue(p_startPosition.x, p_finalPosition.x, p_delta, Model.easing),
                    DOVirtual.EasedValue(p_startPosition.y, p_finalPosition.y, p_delta, Model.easing));
            }
        }
    }
}
