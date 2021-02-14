/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using Dash.Attributes;
using DG.Tweening;
using UnityEngine;

namespace Dash
{
    [Category(NodeCategoryType.ANIMATION)]
    [OutputCount(1)]
    [InputCount(1)]
    [Size(180,85)]
    [Serializable]
    public class AnimateSizeDeltaNode : AnimationNodeBase<AnimateSizeDeltaNodeModel>
    {
        protected override void ExecuteOnTarget(Transform p_target, NodeFlowData p_flowData)
        {
            RectTransform rectTransform = p_target.GetComponent<RectTransform>();

            if (CheckException(rectTransform, "Target doesn't contain RectTransform at node "+_model.id))
                return;

            Vector2 fromSizeDelta = GetParameterValue(Model.fromSizeDelta, p_flowData);
            
            Vector2 startSizeDelta = Model.useFrom 
                ? Model.isFromRelative 
                    ? rectTransform.sizeDelta + fromSizeDelta
                    : fromSizeDelta 
                : rectTransform.sizeDelta;
            
            Vector2 toSizeDelta = GetParameterValue(Model.toSizeDelta, p_flowData);

            if (Model.time == 0)
            {
                UpdateTween(rectTransform, 1, p_flowData, startSizeDelta, toSizeDelta);
                ExecuteEnd(p_flowData);
            }
            else
            {
                // Virtual tween to update from directly
                Tween tween = DOTween
                    .To((f) => UpdateTween(rectTransform, f, p_flowData, startSizeDelta, toSizeDelta), 0,
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

        protected void UpdateTween(RectTransform p_target, float p_delta, NodeFlowData p_flowData, Vector2 p_startSizeDelta, Vector2 p_toSizeDelta)
        {
            if (p_target == null)
                return;

            if (Model.isToRelative)
            {
                p_target.sizeDelta =
                    p_startSizeDelta + new Vector2(DOVirtual.EasedValue(0, p_toSizeDelta.x, p_delta, Model.easing),
                        DOVirtual.EasedValue(0, p_toSizeDelta.y, p_delta, Model.easing));
            }
            else
            {
                p_target.sizeDelta = new Vector2(DOVirtual.EasedValue(p_startSizeDelta.x, p_toSizeDelta.x, p_delta, Model.easing),
                    DOVirtual.EasedValue(p_startSizeDelta.y, p_toSizeDelta.y, p_delta, Model.easing));
            }
        }
    }
}
