/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    [Attributes.Tooltip("Animate transform on current target.")]
    [Category(NodeCategoryType.ANIMATION)]
    [OutputCount(1)]
    [InputCount(1)]
    [Size(220,85)]
    [Serializable]
    public class AnimateTransformNode : AnimationNodeBase<AnimateTransformNodeModel>
    {
        protected override DashTween AnimateOnTarget(Transform p_target, NodeFlowData p_flowData)
        {
            // Position
            
            bool usePosition = GetParameterValue(Model.usePosition, p_flowData);
            bool useFromPosition = GetParameterValue(Model.useFromPosition, p_flowData);
            Vector3 fromPosition = GetParameterValue(Model.fromPosition, p_flowData);
            bool isFromPositionRelative = GetParameterValue(Model.isFromPositionRelative, p_flowData);
            bool storePositionToAttribute = GetParameterValue(Model.storePositionToAttribute, p_flowData);

            Vector3 startPosition = useFromPosition
                ? isFromPositionRelative
                    ? p_target.position + fromPosition
                    : fromPosition
                : p_target.position;

            if (storePositionToAttribute)
            {
                string attribute = GetParameterValue(Model.storePositionAttributeName, p_flowData);
                p_flowData.SetAttribute(attribute, p_target.position);
            }

            Vector3 finalPosition = GetParameterValue(Model.toPosition, p_flowData);
            bool isToPositionRelative = GetParameterValue(Model.isToPositionRelative, p_flowData);
            EaseType easeType = GetParameterValue(Model.easeType, p_flowData);
            
            // Rotation
            bool useRotation = GetParameterValue(Model.useRotation, p_flowData);
            bool useFromRotation = GetParameterValue(Model.useFromRotation, p_flowData);
            
            Vector3 fromRotation = GetParameterValue(Model.fromRotation, p_flowData);
            fromRotation.x = fromRotation.x > 180 ? fromRotation.x - 360 : fromRotation.x; 
            fromRotation.y = fromRotation.y > 180 ? fromRotation.y - 360 : fromRotation.y; 
            fromRotation.z = fromRotation.z > 180 ? fromRotation.z - 360 : fromRotation.z;

            bool isFromRotationRelative = GetParameterValue(Model.isFromRotationRelative, p_flowData);
            
            Quaternion startRotation = useFromPosition
                ? isFromPositionRelative
                    ? p_target.rotation * Quaternion.Euler(fromRotation)
                    : Quaternion.Euler(fromRotation) 
                : p_target.rotation;

            bool storeRotationToAttribute = GetParameterValue(Model.storeRotationToAttribute, p_flowData);
            
            if (storeRotationToAttribute)
            {
                string attribute = GetParameterValue(Model.storeRotationAttributeName, p_flowData);
                p_flowData.SetAttribute<Quaternion>(attribute, p_target.rotation);
            }
            
            Vector3 toRotation = GetParameterValue<Vector3>(Model.toRotation, p_flowData);
            
            // Scale
            bool useScale = GetParameterValue(Model.useScale, p_flowData);
            bool useFromScale = GetParameterValue(Model.useFromScale, p_flowData);
            Vector3 fromScale = GetParameterValue(Model.fromScale, p_flowData);
            bool isFromScaleRelative = GetParameterValue(Model.isFromScaleRelative, p_flowData);
            
            Vector3 startScale = useFromScale 
                ? isFromScaleRelative
                    ? p_target.localScale + Model.fromScale.GetValue(ParameterResolver, p_flowData)  
                    : fromScale 
                : p_target.localScale;

            bool storeScaleToAttribute = GetParameterValue(Model.storeScaleToAttribute, p_flowData);
            
            if (storeScaleToAttribute)
            {
                string attribute = GetParameterValue(Model.storeScaleAttributeName, p_flowData);
                p_flowData.SetAttribute<Vector3>(attribute, p_target.localScale);
            }
            
            Vector3 toScale = GetParameterValue(Model.toScale, p_flowData);
            bool isToScaleRelative = GetParameterValue(Model.isToScaleRelative, p_flowData);
            
            // Animation
            
            float time = GetParameterValue(Model.time, p_flowData);
            float delay = GetParameterValue(Model.delay, p_flowData);
            
            if (time == 0)
            {
                if (usePosition)
                    UpdatePositionTween(p_target, 1, p_flowData, startPosition, finalPosition, isToPositionRelative, easeType);
                if (useRotation)
                    UpdateRotationTween(p_target, 1, p_flowData, startRotation, toRotation, isToPositionRelative, easeType);
                if (useScale)
                    UpdateScaleTween(p_target, 1, p_flowData, startScale, toScale, isToScaleRelative, easeType);
                return null;
            }
            
            return DashTween.To(p_target, 0, 1, time).SetDelay(delay)
                .OnUpdate(f =>
                {
                    if (usePosition)
                        UpdatePositionTween(p_target, f, p_flowData, startPosition, finalPosition, isToPositionRelative,
                                easeType);
                    if (useRotation)
                        UpdateRotationTween(p_target, f, p_flowData, startRotation, toRotation, isToPositionRelative,
                            easeType);
                    if (useScale)
                        UpdateScaleTween(p_target, f, p_flowData, startScale, toScale, isToScaleRelative, easeType);
                });
        }

        protected void UpdatePositionTween(Transform p_target, float p_delta, NodeFlowData p_flowData, Vector3 p_startPosition, Vector3 p_finalPosition, bool p_relative, EaseType p_easeType)
        {
            if (p_target == null)
            {
                if (Model.killOnNullEncounter)
                    Stop_Internal();
                return;
            }

            if (p_relative)
            {
                p_target.position =
                    p_startPosition + new Vector3(DashTween.EaseValue(0, p_finalPosition.x, p_delta, p_easeType),
                        DashTween.EaseValue(0, p_finalPosition.y, p_delta, p_easeType),
                        DashTween.EaseValue(0, p_finalPosition.z, p_delta, p_easeType));
            }
            else
            {
                p_target.position = new Vector3(DashTween.EaseValue(p_startPosition.x, p_finalPosition.x, p_delta, p_easeType),
                    DashTween.EaseValue(p_startPosition.y, p_finalPosition.y, p_delta, p_easeType),
                    DashTween.EaseValue(p_startPosition.z, p_finalPosition.z, p_delta, p_easeType));
            }
        }
        
        protected void UpdateRotationTween(Transform p_target, float p_delta, NodeFlowData p_flowData, Quaternion p_startRotation, Vector3 p_toRotation, bool p_relative, EaseType p_easeType)
        {
            if (p_target == null)
            {
                if (Model.killOnNullEncounter)
                    Stop_Internal();
                return;
            }

            Quaternion rotation = Quaternion.Euler(p_toRotation);
            if (p_relative) rotation = rotation * p_startRotation;
            p_target.localRotation = Quaternion.Lerp(p_startRotation, rotation, DashTween.EaseValue(0,1, p_delta, p_easeType));
        }
        
        protected void UpdateScaleTween(Transform p_target, float p_delta, NodeFlowData p_flowData, Vector3 p_startScale, Vector3 p_toScale, bool p_relative, EaseType p_easeType)
        {
            if (p_target == null)
            {
                if (Model.killOnNullEncounter)
                    Stop_Internal();
                return;
            }

            if (p_relative)
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
    }
}
