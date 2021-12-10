/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace Dash
{
    [Help("Set properties of Image component on target.")]
    [Category(NodeCategoryType.MODIFIER)]
    [OutputCount(1)]
    [InputCount(1)]
    public class SetImageNode : RetargetNodeBase<SetImageNodeModel>
    {
        protected override void ExecuteOnTarget(Transform p_target, NodeFlowData p_flowData)
        {
            Image image = p_target.GetComponent<Image>();
            
            if (CheckException(image, "Target doesn't contain Image component"))
                return;

            if (Model.setSprite)
            {
                image.sprite = GetParameterValue(Model.sprite, p_flowData);
            }

            if (GetParameterValue(Model.setNativeSize, p_flowData))
            {
                image.SetNativeSize();
            }

            if (Model.setIsMaskable)
            {
                image.maskable = GetParameterValue(Model.isMaskable, p_flowData);
            }

            if (Model.setIsRaycastTarget)
            {
                image.raycastTarget = GetParameterValue(Model.isRaycastTarget, p_flowData);
            }
        }
    }
}