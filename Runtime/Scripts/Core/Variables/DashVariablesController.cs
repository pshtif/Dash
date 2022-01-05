using System;
using System.Reflection;
using OdinSerializer;
using OdinSerializer.Utilities;
using UnityEngine;

namespace Dash
{
    public class DashVariablesController : MonoBehaviour
        , ISerializationCallbackReceiver, ISupportsPrefabSerialization, IVariables
    {
        public bool makeGlobal = false;
        
        [HideInInspector]
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

        void Awake()
        {
            Initialize(gameObject);
        }
        
        public void Initialize(GameObject p_gameObject)
        {
            if (GetType() != typeof(DashVariablesController))
            {
                FetchFieldsToVariables();
            }
            
            Variables.Initialize(p_gameObject);

            if (makeGlobal)
            {
                DashCore.Instance.AddGlobalVariables(Variables);
            }
        }

        void FetchFieldsToVariables()
        {
            FieldInfo[] fields = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

            foreach (var field in fields)
            {
                Variables.AddVariableByType(field.FieldType, field.Name, field.GetValue(this));
            }
        }

        public bool HasVariable(string p_name)
        {
            return Variables != null && Variables.HasVariable(p_name);
        }

        public Variable GetVariable(string p_name)
        {
            return Variables?.GetVariable(p_name);
        }

        public Variable<T> GetVariable<T>(string p_name)
        {
            return Variables?.GetVariable<T>(p_name);
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