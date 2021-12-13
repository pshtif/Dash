/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    [Attributes.Tooltip("Animate RectTransform scale.")]
    [Category(NodeCategoryType.ANIMATION)]
    [OutputCount(1)]
    [InputCount(1)]
    [Serializable]
    public class AnimateScaleNode : AnimationNodeBase<AnimateScaleNodeModel>, IAnimationNodeBindable
    {
        protected override DashTween AnimateOnTarget(Transform p_target, NodeFlowData p_flowData)
        {
            Transform targetTransform = p_target.transform;

            if (CheckException(targetTransform, "No RectTransform component found on target"))
                return null;

            Vector3 fromScale = GetParameterValue(Model.fromScale, p_flowData);
            
            Vector3 startScale = Model.useFrom 
                ? Model.isFromRelative
                    ? targetTransform.localScale + Model.fromScale.GetValue(ParameterResolver, p_flowData)  
                    : fromScale 
                : targetTransform.localScale;
            
            if (Model.storeToAttribute)
            {
                string attribute = GetParameterValue(Model.storeAttributeName, p_flowData);
                p_flowData.SetAttribute<Vector3>(attribute, targetTransform.localScale);
            }
            
            Vector3 toScale = GetParameterValue(Model.toScale, p_flowData);
            
            float time = GetParameterValue(Model.time, p_flowData);
            float delay = GetParameterValue(Model.delay, p_flowData);
            EaseType easeType = GetParameterValue(Model.easeType, p_flowData);
            
            if (time == 0)
            {
                UpdateTween(targetTransform, 1, p_flowData, startScale, toScale, easeType);
                
                return null;
            }
            else
            {
                // Virtual tween to update from directly
                return DashTween.To(targetTransform, 0, 1, time)
                    .OnUpdate(f => UpdateTween(targetTransform, f, p_flowData, startScale, toScale, easeType))
                    .SetDelay(delay);
            }
        }

        protected void UpdateTween(Transform p_target, float p_delta, NodeFlowData p_flowData, Vector3 p_startScale, Vector3 p_toScale, EaseType p_easeType)
        {
            if (p_target == null)
            {
                if (Model.killOnNullEncounter)
                    Stop_Internal();
                return;
            }

            if (Model.isToRelative)
            {
                p_target.localScale = p_startScale + new Vector3(DashTween.EaseValue(0, p_toScale.x, p_delta, p_easeType),
                    DashTween.EaseValue(0, p_toScale.y, p_delta, p_easeType),
                    DashTween.EaseValue(0, p_toScale.z, p_delta, p_easeType));
            }
            else
            {
                p_toScale -= p_startScale;
                p_target.localScale = p_startScale + new Vector3(DashTween.EaseValue(0, p_toScale.x, p_delta, p_easeType),
                    DashTween.EaseValue(0, p_toScale.y, p_delta, p_easeType),
                    DashTween.EaseValue(0, p_toScale.z, p_delta, p_easeType));
            }
        }
        
        #if UNITY_EDITOR
        bool IAnimationNodeBindable.IsFromEnabled()
        {
            return !Model.fromScale.isExpression && Model.useFrom;
        }
        
        void IAnimationNodeBindable.SetTargetTo(object p_target)
        {
            ((RectTransform)p_target).localScale = Model.toScale.GetValue(null);
        }
        
        void IAnimationNodeBindable.GetTargetTo(object p_target)
        {
            Model.isToRelative = false;
            Model.toScale.SetValue(((RectTransform)p_target).localScale);
        }
        
        bool IAnimationNodeBindable.IsToEnabled()
        {
            return !Model.toScale.isExpression;
        }
        
        void IAnimationNodeBindable.SetTargetFrom(object p_target)
        {
            ((RectTransform)p_target).localScale = Model.fromScale.GetValue(null);
        }

        void IAnimationNodeBindable.GetTargetFrom(object p_target)
        {
            Model.useFrom = true;
            Model.isFromRelative = false;
            Model.fromScale.SetValue(((RectTransform)p_target).anchoredPosition);
        }
        #endif
    }
}
