using System;
using OdinSerializer;
using OdinSerializer.Utilities;
using UnityEngine;

namespace Dash
{
    public class DashGlobalVariables : MonoBehaviour, ISerializationCallbackReceiver
    {
        [SerializeField]
        protected DashVariables _variables;

        public DashVariables variables
        {
            get
            {
                if (_variables == null) _variables = new DashVariables();
                return _variables;
            }
        }

        private void Awake()
        {
            variables.Initialize(gameObject);
            
            DashCore.Instance.SetGlobalVariables(this);
        }

        private void OnDestroy()
        {
            DashCore.Instance.SetGlobalVariables(null);
        }

        [SerializeField, HideInInspector]
        private SerializationData _serializationData;
        
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            //Debug.Log("OnAfterDeserialize");
            using (var cachedContext = Cache<DeserializationContext>.Claim())
            {
                cachedContext.Value.Config.SerializationPolicy = SerializationPolicies.Everything;
                UnitySerializationUtility.DeserializeUnityObject(this, ref _serializationData, cachedContext.Value);
            }
        }
        
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            //Debug.Log("OnBeforeSerialize");
            using (var cachedContext = Cache<SerializationContext>.Claim())
            {
                cachedContext.Value.Config.SerializationPolicy = SerializationPolicies.Everything;
                UnitySerializationUtility.SerializeUnityObject(this, ref _serializationData, serializeUnityFields: true, context: cachedContext.Value);
            }
        }
    }
}