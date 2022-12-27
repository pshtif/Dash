using System.Reflection;
using OdinSerializer;
using OdinSerializer.Utilities;
using UnityEngine;

namespace Dash
{
    public class DashVariablesController : MonoBehaviour
        , ISerializationCallbackReceiver, ISupportsPrefabSerialization, IVariables, IVariableBindable
    {
        public DashGraph Graph => null;
        
        public bool makeGlobal = false;
        
        [HideInInspector]
        [SerializeField] 
        [OdinSerialize]
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
            Initialize(this);
        }
        
        public void Initialize(IVariableBindable p_bindable)
        {
            if (GetType() != typeof(DashVariablesController))
            {
                FetchFieldsToVariables();
            }
            
            Variables.Initialize(p_bindable);

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

        public void SetVariable<T>(string p_name, T p_value)
        {
            Variables?.SetVariable<T>(p_name, p_value);
        }
        
        [SerializeField, HideInInspector]
        private SerializationData _serializationData;
        
        SerializationData ISupportsPrefabSerialization.SerializationData { get { return this._serializationData; } set { this._serializationData = value; } }

        [SerializeField]
        private SerializedValue[] _serializedVariables;
        
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            // Debug.Log("OnAfterDeserialize: "+_serializedVariables);
            using (var cachedContext = Cache<DeserializationContext>.Claim())
            {
                cachedContext.Value.Config.SerializationPolicy = SerializationPolicies.Everything;
                UnitySerializationUtility.DeserializeUnityObject(this, ref _serializationData, cachedContext.Value);
            }
            
            if (_serializedVariables != null)
            {
                Variables.ClearVariables();
                for (int i = 0; i < _serializedVariables.Length; i++)
                {
                    Variable variable = SerializationUtility.DeserializeValue<Variable>(_serializedVariables[i].bytes,
                        DataFormat.Binary, _serializedVariables[i].references);
                    if (variable != null)
                    {
                        Variables.AddVariable(variable);
                    }
            
                    // Debug.Log(variable+" : "+variable.Name+" , "+variable.value);
                }
                Variables.InvalidateLookup();
            }
        }
        
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            // Debug.Log("OnBeforeSerialize");
            
            using (var cachedContext = Cache<SerializationContext>.Claim())
            {
                cachedContext.Value.Config.SerializationPolicy = SerializationPolicies.Everything;
                UnitySerializationUtility.SerializeUnityObject(this, ref _serializationData, context: cachedContext.Value);
            }

            if (_variables == null)
                return;
            
            _serializedVariables = new SerializedValue[_variables.Count];
            int index = 0;
            foreach (var variable in _variables)
            {
                SerializedValue serializedValue = new SerializedValue();
                serializedValue.bytes = SerializationUtility.SerializeValue(variable, DataFormat.Binary, out serializedValue.references);
                
                //Variable vars = SerializationUtility.DeserializeValue<Variable>(serializedValue.bytes, DataFormat.JSON, serializedValue.references);
                // Debug.Log(variable+" : "+vars.Name+" , "+vars.value);
                // Debug.Log(System.Text.Encoding.UTF8.GetString(serializedValue.bytes));
                _serializedVariables[index++] = serializedValue;
            }
        }
    }
}