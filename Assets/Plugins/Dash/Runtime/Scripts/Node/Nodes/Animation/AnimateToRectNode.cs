/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    [Help("Animate RectTransform towards another RectTransform.")]
    [Category(NodeCategoryType.ANIMATION)]
    [OutputCount(1)]
    [InputCount(1)]
    [Size(200,85)]
    [Serializable]
    public class AnimateToRectNode : AnimationNodeBase<AnimateToRectNodeModel>
    {
        protected override DashTween AnimateOnTarget(Transform p_target, NodeFlowData p_flowData)
        {
            RectTransform rectTransform = (RectTransform)p_target;

            if (CheckException(rectTransform, "No RectTransform component found on target"))
                return null;

            Transform towards = Model.toTarget.Resolve(Controller);

            if (CheckException(towards, "Towards not defined in node "+_model.id))
                return null;

            RectTransform rectTowards = towards.GetComponent<RectTransform>();

            if (CheckException(rectTowards, "No RectTransform component found on towards"))
                return null;

            Vector2 startPosition = rectTransform.anchoredPosition; 
            Quaternion startRotation = rectTransform.localRotation;
            Vector3 startScale = rectTransform.localScale;

            float time = GetParameterValue(Model.time, p_flowData);
            float delay = GetParameterValue(Model.delay, p_flowData);
            EaseType easeType = GetParameterValue(Model.easeType, p_flowData);
            
            if (time == 0)
            {
                UpdateTween(rectTransform, 1, p_flowData, startPosition, startRotation, startScale, rectTowards, easeType);
                return null;
            }
            else
            {
                // Virtual tween to update from directly
                return DashTween.To(rectTransform, 0, 1, time)
                    .OnUpdate(f => UpdateTween(rectTransform, f, p_flowData, startPosition, startRotation, startScale,
                        rectTowards, easeType))
                    .SetDelay(delay);
            }
        }

        protected void UpdateTween(RectTransform p_target, float p_delta, NodeFlowData p_flowData, Vector2 p_startPosition, Quaternion p_startRotation, Vector3 p_startScale, RectTransform p_towards, EaseType p_easeType)
        {
            if (p_target == null)
                return;

            if (Model.useToPosition)
            {
                Vector2 towardsPosition = TransformExtensions.FromToRectTransform(p_towards, p_target);
                //towardsPosition = towardsPosition - p_target.anchoredPosition;
                
                towardsPosition = new Vector2(DashTween.EaseValue(p_startPosition.x, towardsPosition.x, p_delta, p_easeType),
                        DashTween.EaseValue(p_startPosition.y, towardsPosition.y, p_delta, p_easeType));

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
        
#if UNITY_EDITOR
        protected override void DrawCustomGUI(Rect p_rect)
        {
            base.DrawCustomGUI(p_rect);
                
            Transform towards = Model.toTarget.Resolve(Controller);

            var style = new GUIStyle(DashEditorCore.Skin.GetStyle("NodeText"));
            style.alignment = TextAnchor.MiddleRight;
            
            GUI.Label(new Rect(p_rect.x + p_rect.width - 125, p_rect.y + DashEditorCore.TITLE_TAB_HEIGHT, 100, 20),
                towards == null ? "null" : towards.name, style);
        }
#endif
    }
}
