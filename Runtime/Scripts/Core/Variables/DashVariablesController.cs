using System;
using OdinSerializer;
using OdinSerializer.Utilities;
using UnityEngine;

namespace Dash
{
    public class DashVariablesController : MonoBehaviour
        , ISerializationCallbackReceiver, ISupportsPrefabSerialization
    {
        [SerializeField] 
        protected DashVariables _variables;

        public DashVariables Variables
        {
            get
            {
                if (_variables == null) _variables = new DashVariables();

                return _variables;
            }
        }


        private void Awake()
        {
            Variables.Initialize(gameObject);
            
            //MachinaCore.Instance.SetGlobalVariables(this);
        }

        private void OnDestroy()
        {
            //MachinaCore.Instance.SetGlobalVariables(null);
        }

        [SerializeField, HideInInspector]
        private SerializationData _serializationData;
        
        SerializationData ISupportsPrefabSerialization.SerializationData { get { return this._serializationData; } set { this._serializationData = value; } }
        
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