/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace Dash
{
    [Attributes.Tooltip("Spawn GameObject with Image component.")]
    [Category(NodeCategoryType.CREATION)]
    [OutputCount(1)]
    [InputCount(1)]
    public class SpawnImageNode : NodeBase<SpawnImageNodeModel>
    {
        private RectTransform _imagePrefab;

        public RectTransform ImagePrefab
        {
            get
            {
                if (_imagePrefab == null)
                {
                    var go = new GameObject();
                    Image image = go.AddComponent<Image>();
                    _imagePrefab = go.transform as RectTransform;
                }

                return _imagePrefab;
            }
        }
        
        private PrefabPool _prefabPool;
        
        protected override void OnExecuteStart(NodeFlowData p_flowData)
        {
            Transform target = p_flowData.GetAttribute<Transform>(NodeFlowDataReservedAttributes.TARGET);

            RectTransform spawned = null;
            bool usePooling = GetParameterValue(Model.usePooling, p_flowData);
            if (usePooling)
            {
                if (_prefabPool == null) _prefabPool = DashCore.Instance.GetOrCreatePrefabPool(GetParameterValue(Model.poolId, p_flowData), ImagePrefab);
                spawned = _prefabPool.Get() as RectTransform;
                
                if (spawned == null)
                {
                    SetError("Prefab instance is not a RectTransform");
                }
            }
            else
            {
                spawned = GameObject.Instantiate(ImagePrefab);
            }
            
            spawned.name = Model.spawnedName;
            if (Model.setTargetAsParent)
            {
                spawned.transform.SetParent(target, false);
            }
            
            Image image = spawned.GetComponent<Image>();
            image.sprite = GetParameterValue(Model.sprite, p_flowData);
            spawned.anchoredPosition = GetParameterValue(Model.position, p_flowData);

            if (GetParameterValue(Model.setNativeSize, p_flowData))
            {
                image.SetNativeSize();
            }

            image.maskable = GetParameterValue(Model.isMaskable, p_flowData);
            image.raycastTarget = GetParameterValue(Model.isRaycastTarget, p_flowData);

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
                
                p_flowData.SetAttribute<RectTransform>(Model.spawnedAttributeName, spawned);
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