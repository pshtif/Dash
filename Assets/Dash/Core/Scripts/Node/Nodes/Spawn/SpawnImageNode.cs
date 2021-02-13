/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace Dash
{
    [Category(NodeCategoryType.SPAWN)]
    [OutputCount(1)]
    [InputCount(1)]
    public class SpawnImageNode : NodeBase<SpawnImageNodeModel>
    {
        protected override void OnExecuteStart(NodeFlowData p_flowData)
        {
            Transform target = p_flowData.GetAttribute<Transform>(NodeFlowDataReservedAttributes.TARGET);
            
            GameObject spawned = new GameObject();
            if (Model.setTargetAsParent)
            {
                spawned.transform.parent = target;
            }
            
            Image image = spawned.AddComponent<Image>();
            image.sprite = Model.sprite.GetValue(ParameterResolver, p_flowData);
            RectTransform rectTransform = image.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = Model.position.GetValue(ParameterResolver, p_flowData);

            if (Model.retargetToSpawned)
            {
                p_flowData.SetAttribute(NodeFlowDataReservedAttributes.TARGET, spawned.transform);
            }
            
            OnExecuteEnd();
            OnExecuteOutput(0, p_flowData);
        }
        
        #if UNITY_EDITOR
        protected override void DrawCustomGUI(Rect p_rect)
        {
            if (Model.sprite == null || Model.sprite.isExpression || Model.sprite.GetValue(ParameterResolver) == null)
                return;

            if (!Model.sprite.isExpression)
            {
                GUI.DrawTexture(new Rect(p_rect.x + p_rect.width / 2 - 16, p_rect.y + 35, 32, 32),
                    Model.sprite.GetValue(ParameterResolver).texture);
            }
        }
        #endif
    }
}