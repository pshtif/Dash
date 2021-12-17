/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    [Attributes.Tooltip("Animate transform towards another transform.")]
    [Category(NodeCategoryType.ANIMATION)]
    [OutputCount(1)]
    [InputCount(1)]
    [Size(200,85)]
    [Serializable]
    public class AnimateToTransformNode : AnimationNodeBase<AnimateToTransformNodeModel>
    {
        protected override DashTween AnimateOnTarget(Transform p_target, NodeFlowData p_flowData)
        {
            bool useRectTransform = GetParameterValue(Model.useRectTransform, p_flowData);
            Transform targetTransform;
            
            if (Model.useToExpression)
            {
                var value = ExpressionEvaluator.EvaluateUntypedExpression(Model.targetToExpression,
                    ParameterResolver, p_flowData, false);

                if (ExpressionEvaluator.hasErrorInEvaluation)
                {
                    SetError(ExpressionEvaluator.errorMessage);
                    return null;
                }

                targetTransform = value as Transform;

                if (targetTransform == null && value.GetType() == typeof(GameObject))
                {
                    targetTransform = (value as GameObject).transform;
                } 
                else if (targetTransform == null && value is Component)
                {
                    targetTransform = (value as Component).transform;
                }
            }
            else
            {
                targetTransform = Model.targetReference.Resolve(Controller);
            }
            
            if (CheckException(targetTransform, "Target transform null."))
                return null;

            Vector2 startPosition = p_target.position; 
            Quaternion startRotation = p_target.localRotation;
            Vector3 startScale = p_target.localScale;
            
            if (useRectTransform)
            {
                RectTransform rectTransform = (RectTransform)p_target;

                if (CheckException(rectTransform, "No RectTransform component found on target."))
                    return null;

                RectTransform targetRectTransform = (RectTransform)targetTransform;
                
                if (CheckException(targetRectTransform, "No RectTransform component found on target transform."))
                    return null;

                startPosition = rectTransform.anchoredPosition;
            }

            float time = GetParameterValue(Model.time, p_flowData);
            float delay = GetParameterValue(Model.delay, p_flowData);
            EaseType easeType = GetParameterValue(Model.easeType, p_flowData);
            
            if (time == 0)
            {
                if (useRectTransform)
                {
                    UpdateRectTransformTween((RectTransform)p_target, 1, p_flowData, startPosition, startRotation,
                        startScale, (RectTransform)targetTransform, easeType);
                }
                else
                {
                    UpdateTransformTween(p_target, 1, p_flowData, startPosition, startRotation, startScale,
                        targetTransform, easeType);
                }

                return null;
            }
            else
            {
                if (useRectTransform)
                {
                    return DashTween.To((RectTransform)p_target, 0, 1, time)
                        .OnUpdate(f => UpdateRectTransformTween((RectTransform)p_target, f, p_flowData,
                            startPosition, startRotation, startScale, (RectTransform)targetTransform, easeType))
                        .SetDelay(delay);
                }
                else
                {
                    return DashTween.To((RectTransform)p_target, 0, 1, time)
                        .OnUpdate(f => UpdateTransformTween(p_target, f, p_flowData, startPosition, startRotation,
                            startScale, targetTransform, easeType))
                        .SetDelay(delay);
                }
            }
        }

        protected void UpdateRectTransformTween(RectTransform p_target, float p_delta, NodeFlowData p_flowData, Vector2 p_startPosition, Quaternion p_startRotation, Vector3 p_startScale, RectTransform p_towards, EaseType p_easeType)
        {
            if (p_target == null)
            {
                if (Model.killOnNullEncounter)
                    Stop_Internal();
                return;
            }

            if (GetParameterValue(Model.useToPosition, p_flowData))
            {
                Vector2 towardsPosition = TransformExtensions.FromToRectTransform(p_towards, p_target);

                towardsPosition = new Vector2(DashTween.EaseValue(p_startPosition.x, towardsPosition.x, p_delta, p_easeType),
                        DashTween.EaseValue(p_startPosition.y, towardsPosition.y, p_delta, p_easeType));

                p_target.anchoredPosition = towardsPosition;
            }

            if (GetParameterValue(Model.useToRotation, p_flowData))
            {
                p_target.localRotation = Quaternion.Lerp(p_startRotation, p_towards.localRotation, p_delta);
            }

            if (GetParameterValue(Model.useToScale, p_flowData))
            {
                p_target.localScale = Vector3.Lerp(p_startScale, p_towards.localScale, p_delta);
            }
        }
        
        protected void UpdateTransformTween(Transform p_target, float p_delta, NodeFlowData p_flowData, Vector2 p_startPosition, Quaternion p_startRotation, Vector3 p_startScale, Transform p_towards, EaseType p_easeType)
        {
            if (p_target == null)
            {
                if (Model.killOnNullEncounter)
                    Stop_Internal();
                
                return;
            }

            if (GetParameterValue(Model.useToPosition, p_flowData))
            {
                Vector2 towardsPosition = p_towards.position;

                towardsPosition = new Vector2(DashTween.EaseValue(p_startPosition.x, towardsPosition.x, p_delta, p_easeType),
                    DashTween.EaseValue(p_startPosition.y, towardsPosition.y, p_delta, p_easeType));

                p_target.position = towardsPosition;
            }

            if (GetParameterValue(Model.useToRotation, p_flowData))
            {
                p_target.localRotation = Quaternion.Lerp(p_startRotation, p_towards.localRotation, p_delta);
            }

            if (GetParameterValue(Model.useToScale, p_flowData))
            {
                p_target.localScale = Vector3.Lerp(p_startScale, p_towards.localScale, p_delta);
            }
        }
        
#if UNITY_EDITOR
        protected override void DrawCustomGUI(Rect p_rect)
        {
            base.DrawCustomGUI(p_rect);
                
            // Transform towards = Model.toTarget.Resolve(Controller);
            //
            // var style = new GUIStyle(DashEditorCore.Skin.GetStyle("NodeText"));
            // style.alignment = TextAnchor.MiddleRight;
            //
            // GUI.Label(new Rect(p_rect.x + p_rect.width - 125, p_rect.y + DashEditorCore.EditorConfig.theme.TitleTabHeight, 100, 20),
            //     towards == null ? "null" : towards.name, style);
        }
#endif
    }
}
