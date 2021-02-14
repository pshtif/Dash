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
    [Serializable]
    public class AnimateScaleNode : AnimationNodeBase<AnimateScaleNodeModel>
    {
        protected override void ExecuteOnTarget(Transform p_target, NodeFlowData p_flowData)
        {
            RectTransform rectTransform = p_target.GetComponent<RectTransform>();

            if (CheckException(rectTransform, "No RectTransform component found on target in node "+_model.id))
                return;

            Vector3 startScale = Model.useFrom 
                ? Model.isFromRelative
                    ? rectTransform.localScale + Model.fromScale.GetValue(ParameterResolver, p_flowData)  
                    : Model.fromScale.GetValue(ParameterResolver, p_flowData) 
                : rectTransform.localScale;
            
            Vector3 toScale = Model.toScale.GetValue(ParameterResolver, p_flowData);
            
            if (Model.time == 0)
            {
                UpdateTween(rectTransform, 1, p_flowData, startScale, toScale);
                ExecuteEnd(p_flowData);
            }
            else
            {
                // Virtual tween to update from directly
                Tween tween = DOTween
                    .To((f) => UpdateTween(rectTransform, f, p_flowData, startScale, toScale), 0,
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

        protected void UpdateTween(RectTransform p_target, float p_delta, NodeFlowData p_flowData, Vector3 p_startScale, Vector3 p_toScale)
        {
            if (p_target == null)
                return;

            if (Model.isToRelative)
            {
                p_target.localScale = p_startScale + new Vector3(DOVirtual.EasedValue(0, p_toScale.x, p_delta, Model.easing),
                    DOVirtual.EasedValue(0, p_toScale.y, p_delta, Model.easing),
                    DOVirtual.EasedValue(0, p_toScale.z, p_delta, Model.easing));
            }
            else
            {
                p_toScale -= p_startScale;
                p_target.localScale = p_startScale + new Vector3(DOVirtual.EasedValue(0, p_toScale.x, p_delta, Model.easing),
                    DOVirtual.EasedValue(0, p_toScale.y, p_delta, Model.easing),
                    DOVirtual.EasedValue(0, p_toScale.z, p_delta, Model.easing));
            }
        }
    }
}
