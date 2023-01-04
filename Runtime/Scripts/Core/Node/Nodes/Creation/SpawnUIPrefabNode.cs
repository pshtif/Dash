/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;
using Dash.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Dash
{
    [Attributes.Tooltip("Spawn GameObject with RectTransform from prefab.")]
    [Category(NodeCategoryType.CREATION)]
    [OutputCount(1)]
    [InputCount(1)]
    public class SpawnUIPrefabNode : NodeBase<SpawnUIPrefabNodeModel>
    {
        private PrefabPool _prefabPool;
        
        protected override void OnExecuteStart(NodeFlowData p_flowData)
        {
            Transform target = p_flowData.GetAttribute<Transform>(NodeFlowDataReservedAttributes.TARGET);
            
            if (Model.prefab == null)
            {
                OnExecuteEnd();
                OnExecuteOutput(0, p_flowData);
                return;
            }

            RectTransform spawned = null;
            if (Model.usePooling)
            {
                if (_prefabPool == null) _prefabPool = DashCore.Instance.GetOrCreatePrefabPool(GetParameterValue(Model.poolId, p_flowData), Model.prefab);
                spawned = _prefabPool.Get() as RectTransform;
                
                if (spawned == null)
                {
                    SetError("Prefab instance is not a RectTransform");
                }
            }
            else
            {
                spawned = GameObject.Instantiate(Model.prefab);
            }

            if (Model.setTargetAsParent)
            {
                spawned.transform.SetParent(target, false);
            }
            
            spawned.anchoredPosition = GetParameterValue(Model.position, p_flowData);

            if (Model.retargetToSpawned)
            {
                p_flowData.SetAttribute(NodeFlowDataReservedAttributes.TARGET, spawned.transform);
            }
            
            if (Model.createSpawnedAttribute)
            {
                if (string.IsNullOrEmpty(Model.spawnedAttributeName))
                {
                    SetError("Attribute name cannot be empty");
                }
                
                p_flowData.SetAttribute<Transform>(Model.spawnedAttributeName, spawned);
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
            
            Rect labelRect = new Rect(p_rect.x + 24, p_rect.y + DashEditorCore.EditorConfig.theme.TitleTabHeight + 12, p_rect.width-48, 20);
            GUI.Label(labelRect, Model.prefab.name, style);
            // Texture2D texture = AssetPreview.GetMiniThumbnail(Model.prefab);
            // GUI.DrawTexture(new Rect(p_rect.x + p_rect.width / 2 - 16, p_rect.y + 35, 32, 32), texture);
        }
#endif
    }
}