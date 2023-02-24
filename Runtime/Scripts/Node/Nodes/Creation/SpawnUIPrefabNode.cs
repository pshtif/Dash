/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;
using Dash.Editor;
using OdinSerializer.Utilities;
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

            RectTransform prefab = GetParameterValue(Model.prefab, p_flowData);
            
            if (prefab == null)
            {
                OnExecuteEnd();
                OnExecuteOutput(0, p_flowData);
                return;
            }

            RectTransform spawned = null;
            bool usePooling = GetParameterValue(Model.usePooling, p_flowData);
            
            if (usePooling)
            {
                if (_prefabPool == null) _prefabPool = DashCore.Instance.GetOrCreatePrefabPool(GetParameterValue(Model.poolId, p_flowData), prefab);
                spawned = _prefabPool.Get() as RectTransform;
                
                if (spawned == null)
                {
                    SetError("Prefab instance is not a RectTransform");
                }
            }
            else
            {
                spawned = GameObject.Instantiate(prefab);
            }

            bool setTargetAsParent = GetParameterValue(Model.setTargetAsParent, p_flowData);
            if (setTargetAsParent)
            {
                spawned.transform.SetParent(target, false);
            }
            
            spawned.anchoredPosition = GetParameterValue(Model.position, p_flowData);

            bool retargetToSpawned = GetParameterValue(Model.retargetToSpawned, p_flowData);
            if (retargetToSpawned)
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
            var style = new GUIStyle();
            style.alignment = TextAnchor.MiddleCenter;
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = new Color(0.1f, .7f, 0.1f);
            
            Rect labelRect = new Rect(p_rect.x + 24, p_rect.y + DashEditorCore.EditorConfig.theme.TitleTabHeight + 12, p_rect.width-48, 20);

            if (Model.prefab == null || Model.prefab.isExpression)
            {
                style.normal.textColor = Color.cyan;
                GUI.Label(labelRect, GUIUtils.ShortenLabel(style, Model.prefab.expression.IsNullOrWhitespace() ? "Empty" : Model.prefab.expression, 85), style);
            }
            else
            {
                style.normal.textColor = Color.white;
                GUI.Label(labelRect, Model.prefab.GetValue(null).name, style);
            }
        }
#endif
    }
}