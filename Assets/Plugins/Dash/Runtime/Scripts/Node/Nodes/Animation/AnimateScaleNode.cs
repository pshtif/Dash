/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using Dash.Attributes;
using DG.Tweening;
using UnityEngine;

namespace Dash
{
    [Help("Animate RectTransform scale.")]
    [Category(NodeCategoryType.ANIMATION)]
    [OutputCount(1)]
    [InputCount(1)]
    [Serializable]
    public class AnimateScaleNode : AnimationNodeBase<AnimateScaleNodeModel>
    {
        protected override void ExecuteOnTarget(Transform p_target, NodeFlowData p_flowData)
        {
            Transform targetTransform = p_target.transform;

            if (CheckException(targetTransform, "No RectTransform component found on target"))
                return;

            Vector3 fromScale = GetParameterValue(Model.fromScale, p_flowData);
            
            Vector3 startScale = Model.useFrom 
                ? Model.isFromRelative
                    ? targetTransform.localScale + Model.fromScale.GetValue(ParameterResolver, p_flowData)  
                    : fromScale 
                : targetTransform.localScale;
            
            Vector3 toScale = GetParameterValue(Model.toScale, p_flowData);
            
            float time = GetParameterValue(Model.time, p_flowData);
            float delay = GetParameterValue(Model.delay, p_flowData);
            Ease easing = GetParameterValue(Model.easing, p_flowData);
            
            if (time == 0)
            {
                UpdateTween(targetTransform, 1, p_flowData, startScale, toScale, easing);
                ExecuteEnd(p_flowData);
            }
            else
            {
                // Virtual tween to update from directly
                Tween tween = DOTween
                    .To((f) => UpdateTween(targetTransform, f, p_flowData, startScale, toScale, easing), 0,
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

        protected void UpdateTween(Transform p_target, float p_delta, NodeFlowData p_flowData, Vector3 p_startScale, Vector3 p_toScale, Ease p_easing)
        {
            if (p_target == null)
                return;

            if (Model.isToRelative)
            {
                p_target.localScale = p_startScale + new Vector3(DOVirtual.EasedValue(0, p_toScale.x, p_delta, p_easing),
                    DOVirtual.EasedValue(0, p_toScale.y, p_delta, p_easing),
                    DOVirtual.EasedValue(0, p_toScale.z, p_delta, p_easing));
            }
            else
            {
                p_toScale -= p_startScale;
                p_target.localScale = p_startScale + new Vector3(DOVirtual.EasedValue(0, p_toScale.x, p_delta, p_easing),
                    DOVirtual.EasedValue(0, p_toScale.y, p_delta, p_easing),
                    DOVirtual.EasedValue(0, p_toScale.z, p_delta, p_easing));
            }
        }
    }
}
