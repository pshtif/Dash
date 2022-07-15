/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using Dash.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace Dash
{
    [Attributes.Tooltip("Animate Image fill amount.")]
    [Category(NodeCategoryType.ANIMATION)]
    [OutputCount(1)]
    [InputCount(1)]
    [Size(180,85)]
    [Serializable]
    public class AnimateImageFillNode : AnimationNodeBase<AnimateImageFillNodeModel>
    {
        protected override DashTween AnimateOnTarget(Transform p_target, NodeFlowData p_flowData)
        {
            Image image = p_target.GetComponent<Image>();
            if (CheckException(image, "No Image component found on target"))
                return null;

            if (image.type != Image.Type.Filled)
            {
                Debug.LogWarning("Image not of type Filled!");
                return null;
            }

            float fromFillAmount = GetParameterValue(Model.fromFillAmount, p_flowData);
            
            float startFillAmount = Model.useFrom 
                ? Model.isFromRelative 
                    ? image.fillAmount + fromFillAmount
                    : fromFillAmount 
                : image.fillAmount;
            
            if (Model.storeToAttribute)
            {
                string attribute = GetParameterValue(Model.storeAttributeName, p_flowData);
                p_flowData.SetAttribute<float>(attribute, image.fillAmount);
            }
            
            float toFillAmount = GetParameterValue(Model.toFillAmount, p_flowData);

            float time = GetParameterValue(Model.time, p_flowData);
            float delay = GetParameterValue(Model.delay, p_flowData);
            EaseType easeType = GetParameterValue(Model.easeType, p_flowData);
            
            if (time == 0)
            {
                UpdateTween(image, 1, p_flowData, startFillAmount, toFillAmount, easeType);

                return null;
            }
            else
            {
                // Virtual tween to update from directly
                return DashTween.To(image, 0, 1, time).SetDelay(delay)
                    .OnUpdate(f => UpdateTween(image, f, p_flowData, startFillAmount, toFillAmount, easeType));
            }
        }

        protected void UpdateTween(Image p_image, float p_delta, NodeFlowData p_flowData, float p_startFillAmount, float p_toFillAmount, EaseType p_easeType)
        {
            if (p_image == null)
            {
                if (Model.killOnNullEncounter)
                    Stop_Internal();
                return;
            }

            if (Model.isToRelative)
            {
                p_image.fillAmount =
                    p_startFillAmount + DashTween.EaseValue(0, p_toFillAmount, p_delta, p_easeType);
            }
            else
            {
                p_image.fillAmount = DashTween.EaseValue(p_startFillAmount, p_toFillAmount, p_delta, p_easeType);
            }
            
#if UNITY_EDITOR
            // Unity doesn't update filled images in editor time unless explicitly marked dirty
            if (DashEditorCore.Previewer.IsPreviewing)
            {
                UnityEditor.EditorUtility.SetDirty(p_image);
            }
#endif
        }
    }
}
