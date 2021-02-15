/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Dash
{
    [Category(NodeCategoryType.SPAWN)]
    [OutputCount(1)]
    [InputCount(1)]
    public class SpawnUIPrefabNode : NodeBase<SpawnUIPrefabNodeModel>
    {
        protected override void OnExecuteStart(NodeFlowData p_flowData)
        {
            Transform target = p_flowData.GetAttribute<Transform>(NodeFlowDataReservedAttributes.TARGET);
            
            if (Model.prefab == null)
            {
                OnExecuteEnd();
                OnExecuteOutput(0, p_flowData);
                return;
            }

            RectTransform spawned = GameObject.Instantiate(Model.prefab);
            if (Model.setTargetAsParent)
            {
                spawned.transform.parent = target;
            }
            
            spawned.anchoredPosition = Model.position.GetValue(ParameterResolver, p_flowData);

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
            if (Model.prefab == null)
                return;

            var style = new GUIStyle();
            style.alignment = TextAnchor.MiddleCenter;
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = new Color(0.1f, .7f, 0.1f);
            
            Rect labelRect = new Rect(p_rect.x + 24, p_rect.y + DashEditorCore.TITLE_TAB_HEIGHT + 12, p_rect.width-48, 20);
            GUI.Label(labelRect, Model.prefab.name, style);
            // Texture2D texture = AssetPreview.GetMiniThumbnail(Model.prefab);
            // GUI.DrawTexture(new Rect(p_rect.x + p_rect.width / 2 - 16, p_rect.y + 35, 32, 32), texture);
        }
#endif
    }
}