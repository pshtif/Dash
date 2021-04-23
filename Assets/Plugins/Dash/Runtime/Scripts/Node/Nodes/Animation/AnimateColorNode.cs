/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using Dash.Attributes;
using Dash.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR

#endif

namespace Dash
{
    [Help("Animate Image or TextMeshPro color or CanvasGroup alpha.")]
    [Category(NodeCategoryType.ANIMATION)]
    [OutputCount(1)]
    [InputCount(1)]
    [Serializable]
    public class AnimateColorNode : AnimationNodeBase<AnimateColorNodeModel>
    {
        override protected DashTween AnimateOnTarget(Transform p_target, NodeFlowData p_flowData)
        {
            switch (Model.targetType)
            {
                case AlphaTargetType.IMAGE:
                    Image image = p_target.GetComponent<Image>();
                    if (!CheckException(image, "No Image component found on target")) 
                    {
                        return ExecuteAs(image, p_flowData);
                    }

                    break;
                case AlphaTargetType.TEXTMESHPRO:
                    TMP_Text text = p_target.GetComponent<TMP_Text>();
                    if (!CheckException(text, "No TMP_Text component found on target"))
                    {
                        return ExecuteAs(text, p_flowData);
                    }

                    break;
            }
            
            return null;
        }

        DashTween ExecuteAs(Image p_target, NodeFlowData p_flowData)
        {
            Color startColor = p_target.color;
            Color toColor = GetParameterValue<Color>(Model.toColor, p_flowData);

            float time = GetParameterValue(Model.time, p_flowData);
            float delay = GetParameterValue(Model.delay, p_flowData);
            EaseType easing = GetParameterValue(Model.easeType, p_flowData);
            
            if (time == 0)
            {
                UpdateTween(p_target, 1, p_flowData, startColor, toColor);

                return null;
            }
            else
            {
                return DashTween.To(p_target, 0, 1, time)
                    .OnUpdate(f => UpdateTween(p_target, f, p_flowData, startColor, toColor))
                    .SetDelay(delay);
            }
        }
        
        DashTween ExecuteAs(TMP_Text p_target, NodeFlowData p_flowData)
        {
            float time = GetParameterValue(Model.time, p_flowData);
            float delay = GetParameterValue(Model.delay, p_flowData);
            EaseType easeType = GetParameterValue(Model.easeType, p_flowData);
            Color startColor = p_target.color;
            Color toColor = GetParameterValue<Color>(Model.toColor, p_flowData);

            if (time == 0)
            {
                UpdateTween(p_target, 1, p_flowData, startColor, toColor);
                return null;
            }
            else
            {
                DashTween tween = DashTween.To(p_target, 0, 1, time)
                    .OnUpdate(f => UpdateTween(p_target, f, p_flowData, startColor, toColor))
                    .SetDelay(delay);

                return tween;
            }
        }

        // void ExecuteAsCanvasGroup(CanvasGroup p_target, NodeFlowData p_flowData)
        // {
        //     float toAlpha = Model.toAlpha.GetValue(ParameterResolver, p_flowData);
        //     // Virtual tween to update from directly
        //     Tween tween = DOTween.To(f => UpdateTween(p_target, f, p_flowData, startAlpha, toAlpha), 0, 1, Model.time)
        //         .SetDelay(Model.delay)
        //         .SetEase(Model.easing)
        //         .OnComplete(() => {
        //             OnExecuteOutput(0, p_flowData);
        //             OnExecuteEnd();
        //         });
        // }
        
        protected void UpdateTween(CanvasGroup p_target, float p_delta, NodeFlowData p_flowData, float p_startAlpha, float p_toAlpha)
        {
            if (Model.isToRelative)
            {
                p_target.alpha = p_startAlpha + p_toAlpha * p_delta;
            }
            else
            {
                p_target.alpha = Mathf.Lerp(p_startAlpha, p_toAlpha, p_delta);
            }
        }
        
        protected void UpdateTween(Image p_target, float p_delta, NodeFlowData p_flowData, Color p_startColor, Color p_toColor)
        {
            p_target.color = Color.Lerp(p_startColor, p_toColor, p_delta);
        }
        
        protected void UpdateTween(TMP_Text p_target, float p_delta, NodeFlowData p_flowData, Color p_startColor, Color p_toColor)
        {
            p_target.color = Color.Lerp(p_startColor, p_toColor, p_delta);
        }
    }
}
