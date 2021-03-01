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
            RectTransform rectTransform = p_target.GetComponent<RectTransform>();

            if (CheckException(rectTransform, "No RectTransform component found on target"))
                return;

            Vector3 fromRotation = GetParameterValue(Model.fromRotation, p_flowData);
            
            Quaternion startRotation = Model.useFrom
                ? Model.isFromRelative
                    ? rectTransform.rotation * Quaternion.Euler(Model.fromRotation.GetValue(ParameterResolver, p_flowData))
                    : Quaternion.Euler(fromRotation) 
                : rectTransform.rotation;

            Vector3 toRotation = GetParameterValue<Vector3>(Model.toRotation, p_flowData);

            if (Model.time == 0)
            {
                UpdateTween(rectTransform, 1, p_flowData, startRotation, toRotation);
                ExecuteEnd(p_flowData);
            }
            else
            {
                // Virtual tween to update from directly
                Tween tween = DOTween
                    .To((f) => UpdateTween(rectTransform, f, p_flowData, startRotation, toRotation), 0,
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

        protected void UpdateTween(RectTransform p_target, float p_delta, NodeFlowData p_flowData, Quaternion p_startRotation, Vector3 p_toRotation)
        {
            if (p_target == null)
                return;

            Quaternion rotation = Quaternion.Euler(p_toRotation);
            if (Model.isToRelative) rotation = rotation * Quaternion.Inverse(p_startRotation);
            Vector3 finalRotation = rotation.eulerAngles;
            Vector3 easedRotation = new Vector3(DOVirtual.EasedValue(0, finalRotation.x, p_delta, Model.easing),
                DOVirtual.EasedValue(0, finalRotation.y, p_delta, Model.easing),
                DOVirtual.EasedValue(0, finalRotation.z, p_delta, Model.easing));

            
            p_target.localRotation = p_startRotation * Quaternion.Euler(easedRotation);
        }
    }
}
