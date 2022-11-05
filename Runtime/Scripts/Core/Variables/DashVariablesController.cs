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
                        (Variables as IInternalVariablesAccess).AddVariable(variable);
                    }
            
                    // Debug.Log(variable+" : "+variable.Name+" , "+variable.value);
                }
                (Variables as IInternalVariablesAccess).InvalidateLookup();
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
        
//         #region EXPOSE_TABLE
//
//         [HideInInspector] 
//         [SerializeField]
//         protected List<PropertyName> _propertyNames = new List<PropertyName>();
// #if UNITY_EDITOR
//         public List<PropertyName> propertyNames => _propertyNames;
// #endif
//
//         [HideInInspector]
//         [SerializeField]
//         protected List<Object> _references = new List<Object>();
// #if UNITY_EDITOR
//         public List<Object> references => _references;
// #endif
//
//         public void CleanupReferences(List<string> p_existingGUIDs)
//         {
//             for (int i = 0; i < _propertyNames.Count; i++)
//             {
//                 if (p_existingGUIDs.Contains(_propertyNames[i].ToString()))
//                     continue;
//
//                 _propertyNames.RemoveAt(i);
//                 _references.RemoveAt(i);
//                 i--;
//             }
//         }
//
//         public void ClearReferenceValue(PropertyName id)
//         {
//             int index = _propertyNames.IndexOf(id);
//             if (index != -1)
//             {
//                 _references.RemoveAt(index);
//                 _propertyNames.RemoveAt(index);
//             }
//         }
//
//         public Object GetReferenceValue(PropertyName id, out bool idValid)
//         {
//             int index = _propertyNames.IndexOf(id);
//             if (index != -1)
//             {
//                 idValid = true;
//                 return _references[index];
//             }
//
//             idValid = false;
//             return null;
//         }
//
//         public void SetReferenceValue(PropertyName id, Object value)
//         {
//             int index = _propertyNames.IndexOf(id);
//             if (index != -1)
//             {
//                 _references[index] = value;
//             }
//             else
//             {
//                 _propertyNames.Add(id);
//                 _references.Add(value);
//             }
//         }
//         
//         #endregion
    }
}