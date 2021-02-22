/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Xml;
using DG.Tweening;
using Dash.Attributes;
using TMPro;
using UnityEngine;

namespace Dash
{
    [Category(NodeCategoryType.ANIMATION)]
    [OutputCount(1)]
    [InputCount(1)]
    [Size(200,85)]
    [Serializable]
    public class AnimateToNode : RetargetNodeBase<AnimateToNodeModel>
    {
        protected override void ExecuteOnTarget(Transform p_target, NodeFlowData p_flowData)
        {
            RectTransform rectTransform = (RectTransform)p_target;

            if (CheckException(rectTransform, "No RectTransform component found on target"))
                return;

            Transform towards = Model.toTarget.Resolve(Controller);

            if (CheckException(towards, "Towards not defined in node "+_model.id))
                return;

            RectTransform rectTowards = towards.GetComponent<RectTransform>();

            if (CheckException(rectTowards, "No RectTransform component found on towards"))
                return;

            Vector2 startPosition = rectTransform.anchoredPosition; 
            Quaternion startRotation = rectTransform.localRotation;
            Vector3 startScale = rectTransform.localScale;

            if (Model.time == 0)
            {
                UpdateTween(rectTransform, 1, p_flowData, startPosition, startRotation, startScale, rectTowards);
                ExecuteEnd(p_flowData);
            }
            else
            {
                // Virtual tween to update from directly
                Tween tween = DOTween
                    .To((f) => UpdateTween(rectTransform, f, p_flowData, startPosition, startRotation, startScale, rectTowards), 0,
                        1, Model.time)
                    .SetDelay(Model.delay)
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

        protected void UpdateTween(RectTransform p_target, float p_delta, NodeFlowData p_flowData, Vector2 p_startPosition, Quaternion p_startRotation, Vector3 p_startScale, RectTransform p_towards)
        {
            if (p_target == null)
                return;

            if (Model.useToPosition)
            {
                Vector2 towardsPosition = TransformExtensions.FromToRectTransform(p_towards, p_target);
                //towardsPosition = towardsPosition - p_target.anchoredPosition;
                
                towardsPosition = new Vector2(DOVirtual.EasedValue(p_startPosition.x, towardsPosition.x, p_delta, Model.easing),
                        DOVirtual.EasedValue(p_startPosition.y, towardsPosition.y, p_delta, Model.easing));

                p_target.anchoredPosition = towardsPosition;
            }

            if (Model.useToRotation)
            {
                p_target.localRotation = Quaternion.Lerp(p_startRotation, p_towards.localRotation, p_delta);
            }

            if (Model.useToScale)
            {
                p_target.localScale = Vector3.Lerp(p_startScale, p_towards.localScale, p_delta);
            }
        }
    }
}
