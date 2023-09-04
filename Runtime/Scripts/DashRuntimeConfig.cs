/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using OdinSerializer;
using OdinSerializer.Utilities;
using UnityEditor;
using UnityEngine;

namespace Dash
{
    [Serializable]
    public class DashRuntimeConfig : ScriptableObject, ISerializationCallbackReceiver
    {
        #if UNITY_EDITOR
        public static DashRuntimeConfig Create()
        {
            DashRuntimeConfig config = (DashRuntimeConfig) AssetDatabase.LoadAssetAtPath("Assets/Resources/DashRuntimeConfig.asset",
                typeof(DashRuntimeConfig));
            
            if (config == null)
            {
                config = CreateInstance<DashRuntimeConfig>();
                if (config != null)
                {
                    if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                    {
                        AssetDatabase.CreateFolder("Assets", "Resources");
                    }
                    AssetDatabase.CreateAsset(config, "Assets/Resources/DashRuntimeConfig.asset");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }

            return config;
        }
        #endif
        
        public bool enableCustomExpressionClasses = false;
        
        public bool allowAttributeTypeChange = false;
        
        public string packageVersion;

        [OdinSerialize]
        public Dictionary<string, string> expressionMacros = new Dictionary<string, string>();

        [OdinSerialize] 
        public Dictionary<PrefabInfo, GameObject> prefabs = new Dictionary<PrefabInfo, GameObject>();

        #region SERIALIZATION
        
        [SerializeField, HideInInspector] 
        private SerializationData _serializationData;
        
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            // Bug in some versions of Unity
            if (this == null)
                return;
            
            using (var cachedContext = Cache<DeserializationContext>.Claim())
            {
                cachedContext.Value.Config.SerializationPolicy = SerializationPolicies.Everything;
                UnitySerializationUtility.DeserializeUnityObject(this, ref _serializationData, cachedContext.Value);
            }
        }
        
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            // Bug in some versions of Unity
            if (this == null)
                return;
            
            using (var cachedContext = Cache<SerializationContext>.Claim())
            {
                cachedContext.Value.Config.SerializationPolicy = SerializationPolicies.Everything;
                UnitySerializationUtility.SerializeUnityObject(this, ref _serializationData, serializeUnityFields: true, context: cachedContext.Value);
            }
        }
#endregion
    }
}