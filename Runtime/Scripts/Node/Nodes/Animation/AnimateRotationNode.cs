/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    [Documentation("Nodes.md#animaterotation")]
    [Attributes.Tooltip("Animate RectTransform rotation.")]
    [Category(NodeCategoryType.ANIMATION)]
    [OutputCount(1)]
    [InputCount(1)]
    [Size(200,85)]
    [Serializable]
    public class AnimateRotationNode : AnimationNodeBase<AnimateRotationNodeModel>, IAnimationNodeBindable
    {
        protected override DashTween AnimateOnTarget(Transform p_target, NodeFlowData p_flowData)
        {
            Transform targetTransform = p_target.transform;

            if (CheckException(targetTransform, "No RectTransform component found on target"))
                return null;

            // TODO skip this if not using from rotation
            Vector3 fromRotation = GetParameterValue(Model.fromRotation, p_flowData);
            fromRotation.x = fromRotation.x > 180 ? fromRotation.x - 360 : fromRotation.x; 
            fromRotation.y = fromRotation.y > 180 ? fromRotation.y - 360 : fromRotation.y; 
            fromRotation.z = fromRotation.z > 180 ? fromRotation.z - 360 : fromRotation.z; 
            
            Quaternion startRotation = Model.useFrom
                ? Model.isFromRelative
                    ? targetTransform.rotation * Quaternion.Euler(fromRotation)
                    : Quaternion.Euler(fromRotation) 
                : targetTransform.rotation;

            if (Model.storeToAttribute)
            {
                string attribute = GetParameterValue(Model.storeAttributeName, p_flowData);
                p_flowData.SetAttribute<Quaternion>(attribute, targetTransform.rotation);
            }
            
            Vector3 toRotation = GetParameterValue<Vector3>(Model.toRotation, p_flowData);

            float time = GetParameterValue(Model.time, p_flowData);
            float delay = GetParameterValue(Model.delay, p_flowData);
            EaseType easing = GetParameterValue(Model.easeType, p_flowData);
            
            if (time == 0)
            {
                UpdateTween(targetTransform, 1, p_flowData, startRotation, toRotation, easing);
                return null;
            }
            else
            {
                return DashTween.To(targetTransform, 0, 1, time)
                    .SetDelay(delay)
                    .OnUpdate(f => UpdateTween(targetTransform, f, p_flowData, startRotation, toRotation, easing));

            }
        }
        
        public Vector3 LerpEulerAngles(Vector3 p_from, Vector3 p_to, float p_t)
        {
            Vector3 result = new Vector3(
                Mathf.LerpAngle(p_from.x, p_to.x, p_t),
                Mathf.LerpAngle(p_from.y, p_to.y, p_t),
                Mathf.LerpAngle(p_from.z, p_to.z, p_t)
            );
            return result;
        }

        protected void UpdateTween(Transform p_target, float p_delta, NodeFlowData p_flowData, Quaternion p_startRotation, Vector3 p_toRotation, EaseType p_easeType)
        {
            if (p_target == null)
            {
                if (Model.killOnNullEncounter)
                    Stop_Internal();
                return;
            }

            float t = DashTween.EaseValue(0, 1, p_delta, p_easeType);
    
            if (Model.isToRelative)
            {
                bool needsMultiRotation = Mathf.Abs(p_toRotation.x) > 180 || 
                                          Mathf.Abs(p_toRotation.y) > 180 || 
                                          Mathf.Abs(p_toRotation.z) > 180;
                
                Vector3 eulers;
                if (needsMultiRotation)
                {
                    eulers = Vector3.Lerp(Vector3.zero, p_toRotation, t);
                }
                else
                {
                    eulers = LerpEulerAngles(Vector3.zero, p_toRotation, t);
                }
                
                Quaternion relativeRotation = Quaternion.Euler(eulers);
                p_target.localRotation = p_startRotation * relativeRotation;
            }
            else
            {
                Vector3 startEulers = p_startRotation.eulerAngles;
                
                startEulers.x = startEulers.x > 180 ? startEulers.x - 360 : startEulers.x;
                startEulers.y = startEulers.y > 180 ? startEulers.y - 360 : startEulers.y;
                startEulers.z = startEulers.z > 180 ? startEulers.z - 360 : startEulers.z;
                
                Vector3 delta = p_toRotation - startEulers;
                
                bool needsMultiRotation = Mathf.Abs(delta.x) > 180 || 
                                          Mathf.Abs(delta.y) > 180 || 
                                          Mathf.Abs(delta.z) > 180;
                
                Vector3 eulers;
                if (needsMultiRotation)
                {
                    eulers = Vector3.Lerp(startEulers, p_toRotation, t);
                }
                else
                {
                    eulers = LerpEulerAngles(startEulers, p_toRotation, t);
                }
                
                p_target.localRotation = Quaternion.Euler(eulers);
            }

            // Vector3 eulers = Vector3.Lerp(Vector3.zero, p_toRotation, DashTween.EaseValue(0, 1, p_delta, p_easeType));

            // Quaternion rotation = Quaternion.Euler(eulers);
            // if (Model.isToRelative) rotation = rotation * p_startRotation;
            // p_target.localRotation = rotation;
            // //Quaternion.Lerp(p_startRotation, rotation, DashTween.EaseValue(0, 1, p_delta, p_easeType));
        }
        
        #if UNITY_EDITOR
        bool IAnimationNodeBindable.IsFromEnabled()
        {
            return !Model.fromRotation.isExpression && Model.useFrom && !Model.isFromRelative;
        }
        
        void IAnimationNodeBindable.SetTargetFrom(object p_target)
        {
            ((RectTransform)p_target).localRotation = Quaternion.Euler(Model.fromRotation.GetValue(null));
        }
        
        void IAnimationNodeBindable.GetTargetFrom(object p_target)
        {
            Model.useFrom = true;
            Model.fromRotation.isExpression = false;
            Model.isFromRelative = false;
            Model.fromRotation.SetValue(((RectTransform)p_target).localRotation.eulerAngles);
        }
        
        void IAnimationNodeBindable.SetTargetTo(object p_target)
        {
            ((RectTransform)p_target).anchoredPosition = Model.toRotation.GetValue(null);
        }
        
        bool IAnimationNodeBindable.IsToEnabled()
        {
            return !Model.toRotation.isExpression && !Model.isToRelative;
        }
        
        void IAnimationNodeBindable.GetTargetTo(object p_target)
        {
            Model.toRotation.isExpression = false;
            Model.isToRelative = false;
            Model.toRotation.SetValue(((RectTransform)p_target).anchoredPosition);
        }
        #endif 
    }
}
