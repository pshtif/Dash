/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace Dash
{
    [Help("Spawn GameObject with Image component.")]
    [Category(NodeCategoryType.CREATION)]
    [OutputCount(1)]
    [InputCount(1)]
    public class SpawnImageNode : NodeBase<SpawnImageNodeModel>
    {
        protected override void OnExecuteStart(NodeFlowData p_flowData)
        {
            Transform target = p_flowData.GetAttribute<Transform>(NodeFlowDataReservedAttributes.TARGET);
            
            GameObject spawned = new GameObject();
            spawned.name = Model.spawnedName;
            if (Model.setTargetAsParent)
            {
                spawned.transform.SetParent(target, false);
            }
            
            Image image = spawned.AddComponent<Image>();
            image.sprite = GetParameterValue(Model.sprite, p_flowData);
            RectTransform rectTransform = image.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = GetParameterValue(Model.position, p_flowData);

            if (Model.retargetToSpawned)
            {
                p_flowData.SetAttribute(NodeFlowDataReservedAttributes.TARGET, spawned.transform);
            }
            else if (Model.createSpawnedAttribute)
            {
                if (string.IsNullOrEmpty(Model.spawnedAttributeName))
                {
                    SetError("Attribute name cannot be empty");
                }
                
                p_flowData.SetAttribute<Transform>(Model.spawnedAttributeName, rectTransform);
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