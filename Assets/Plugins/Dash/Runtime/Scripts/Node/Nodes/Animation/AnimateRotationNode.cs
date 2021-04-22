/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using Dash.Attributes;
using DG.Tweening;
using UnityEngine;

namespace Dash
{
    [Help("Animate RectTransform rotation.")]
    [Category(NodeCategoryType.ANIMATION)]
    [OutputCount(1)]
    [InputCount(1)]
    [Size(200,85)]
    [Serializable]
    public class AnimateRotationNode : AnimationNodeBase<AnimateRotationNodeModel>
    {
        protected override void ExecuteOnTarget(Transform p_target, NodeFlowData p_flowData)
        {
            Transform targetTransform = p_target.transform;

            if (CheckException(targetTransform, "No RectTransform component found on target"))
                return;

            Vector3 fromRotation = GetParameterValue(Model.fromRotation, p_flowData);
            fromRotation.x = fromRotation.x > 180 ? fromRotation.x - 360 : fromRotation.x; 
            fromRotation.y = fromRotation.y > 180 ? fromRotation.y - 360 : fromRotation.y; 
            fromRotation.z = fromRotation.z > 180 ? fromRotation.z - 360 : fromRotation.z; 
            
            Quaternion startRotation = Model.useFrom
                ? Model.isFromRelative
                    ? targetTransform.rotation * Quaternion.Euler(fromRotation)
                    : Quaternion.Euler(fromRotation) 
                : targetTransform.rotation;

            Vector3 toRotation = GetParameterValue<Vector3>(Model.toRotation, p_flowData);

            float time = GetParameterValue(Model.time, p_flowData);
            float delay = GetParameterValue(Model.delay, p_flowData);
            Ease easing = GetParameterValue(Model.easing, p_flowData);
            
            if (time == 0)
            {
                UpdateTween(targetTransform, 1, p_flowData, startRotation, toRotation, easing);
                ExecuteEnd(p_flowData);
            }
            else
            {
                // Virtual tween to update from directly
                Tween tween = DOTween
                    .To((f) => UpdateTween(targetTransform, f, p_flowData, startRotation, toRotation, easing), 0,
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

        protected void UpdateTween(Transform p_target, float p_delta, NodeFlowData p_flowData, Quaternion p_startRotation, Vector3 p_toRotation, Ease p_easing)
        {
            if (p_target == null)
                return;

            Quaternion rotation = Quaternion.Euler(p_toRotation);
            if (Model.isToRelative) rotation = rotation * p_startRotation;
            p_target.localRotation = Quaternion.Lerp(p_startRotation, rotation, DOVirtual.EasedValue(0,1, p_delta, p_easing));

            /*
            Debug.Log(rotation.eulerAngles + " : " + p_toRotation + " : " + p_startRotation.eulerAngles);
            Vector3 finalRotation = rotation.eulerAngles;
            finalRotation.z = finalRotation.z > 180 ? finalRotation.z - 360 : finalRotation.z;
            Vector3 easedRotation = new Vector3(DOVirtual.EasedValue(0, finalRotation.x, p_delta, Model.easing),
                DOVirtual.EasedValue(0, finalRotation.y, p_delta, Model.easing),
                DOVirtual.EasedValue(0, finalRotation.z, p_delta, Model.easing));
            
            p_target.localRotation = p_startRotation * Quaternion.Euler(easedRotation);
            */
        }
    }
}
