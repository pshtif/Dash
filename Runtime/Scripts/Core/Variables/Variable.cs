/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

namespace Dash
{
    [Serializable]
    public abstract class Variable
    {
        public VariableType Type { get; protected set; } = VariableType.VALUE;
        
        public bool IsBound => Type == VariableType.BOUND;

        public bool IsLookup => Type == VariableType.LOOKUP;
        
        abstract protected object objectValue { get; set; }

        public object value
        {
            get { return objectValue; }
            set { objectValue = value; }
        }

        public abstract Type GetVariableType();

        [SerializeField, HideInInspector]
        protected string _name;

        public string Name => _name;

        internal void Rename(string p_newName)
        {
            _name = p_newName;
        }
        
        abstract public void BindProperty(PropertyInfo p_property, Component p_component, IVariableBindable p_bindable);
        
        abstract public void BindField(FieldInfo p_field, Component p_component, IVariableBindable p_bindable);

        abstract public void UnbindProperty();

        abstract public bool InitializeBinding(IVariableBindable p_bindable);

        public void SetAsLookup(bool p_lookup)
        {
            Type = p_lookup ? VariableType.LOOKUP : VariableType.VALUE;
        }

        abstract public void InitializeLookup(IVariableBindable p_bindable);

        public abstract Variable Clone();

#if UNITY_EDITOR
        public abstract bool ValueField(float p_maxWidth, IVariableBindable p_bindable);

        static public string ConvertToTypeName(Type p_type)
        {
            string typeString = p_type.ToString();
            switch (typeString)
            {
                case "System.Single":
                    return "float";
                case "System.Int32":
                    return "int";
            }

            typeString = typeString.Substring(typeString.LastIndexOf(".") + 1);
            
            if (p_type.IsGenericType && p_type.GetGenericTypeDefinition() == typeof(ExposedReference<>))
            {
                return typeString.Substring(0, typeString.Length - 1);
            }

            return typeString;
        }
        
#endif
    }

    [Serializable]
    public class Variable<T> : Variable
    {
        protected T _value;

        [NonSerialized]
        private Func<T> _getter;
        [NonSerialized]
        private Action<T> _setter;
        
        // Tried to use Type/MethodInfo directly but it is not good for serialization so using string
        private VariableBindType _boundType;
        private string _boundName;
        private string _boundComponentName;

        public new T value
        {
            get
            {
                return _getter != null ? _getter() : _value; }
            set
            {
                if (_setter != null) _setter(value);
                else _value = value;
            }
        }

        public override Type GetVariableType()
        {
            return typeof(T);
        }

        public string GetVariableTypeShortName()
        {
            string name = typeof(T).ToString();
            name = name.IndexOf(".") >= 0 ? name.Substring(name.LastIndexOf(".") + 1) : name;
            return name;
        }
        
        protected override object objectValue
        {
            get { return value; }
            set { this.value = (T)value; }
        }

        public Variable(string p_name, [CanBeNull] T p_value)
        {
            _name = p_name;
            value = p_value == null ? default(T) : p_value;
        }
        
        public override Variable Clone()
        {
            Variable<T> v = new Variable<T>(Name, value);
            v._boundType = _boundType;
            v._boundName = _boundName;
            v._boundComponentName = _boundComponentName;
            return v;
        }
        
        public override void BindProperty(PropertyInfo p_property, Component p_component, IVariableBindable p_bindable)
        {
            Type = VariableType.BOUND;
            _boundType = VariableBindType.PROPERTY;
            _boundName = p_property.Name;
            _boundComponentName = p_component.GetType().FullName;

            InitializeBinding(p_bindable);
        }
        
        public override void BindField(FieldInfo p_field, Component p_component, IVariableBindable p_bindable)
        {
            Type = VariableType.BOUND;
            _boundType = VariableBindType.FIELD;
            _boundName = p_field.Name;
            _boundComponentName = p_component.GetType().FullName;

            InitializeBinding(p_bindable);
        }

        public void RebindProperty(IVariableBindable p_bindable)
        {
            if (Type == VariableType.BOUND)
            {
                InitializeBinding(p_bindable);
            }
        }

        public override void UnbindProperty()
        {
            _boundName = String.Empty;
            _boundComponentName = String.Empty;
            _getter = null;
            _setter = null;
            Type = VariableType.VALUE;
        }

        public override bool InitializeBinding(IVariableBindable p_bindable)
        {
            if (!IsBound)
                return false;
            
            Type componentType = ReflectionUtils.GetTypeByName(_boundComponentName);
            Component component = p_bindable.gameObject.GetComponent(componentType);
            if (component == null)
            {
                Debug.LogWarning("Cannot find component " + _boundComponentName + " for variable " + Name);
                return false;
            }

            if (_boundType == VariableBindType.PROPERTY)
            {
                PropertyInfo property = componentType.GetProperty(_boundName);
                if (property == null)
                {
                    Debug.LogWarning("Cannot find property " + _boundName + " on component " + component.name);
                    return false;
                }

                var method = property.GetGetMethod();
                var delegateGetType = typeof(Func<>).MakeGenericType(method.ReturnType);

                _getter = ConvertDelegate<Func<T>>(Delegate.CreateDelegate(delegateGetType, component, method, true));

                var setMethod = property.GetSetMethod();
                if (setMethod != null)
                {
                    var delegateSetType = typeof(Action<>).MakeGenericType(method.ReturnType);
                    _setter = ConvertDelegate<Action<T>>(Delegate.CreateDelegate(delegateSetType, component, setMethod,
                        true));
                }
            }
            
            if (_boundType == VariableBindType.FIELD)
            {
                FieldInfo field = componentType.GetField(_boundName);
                if (field == null)
                {
                    Debug.LogWarning("Cannot find field " + _boundName + " on component " + component.name);
                    return false;
                }

                _getter = () => (T)field.GetValue(component);

                _setter = (T t) => field.SetValue(component, t);
            }

            return true;
        }
        
        public override void InitializeLookup(IVariableBindable p_bindable)
        {
            if (!IsLookup)
                return;
            
            var transform = p_bindable.gameObject.transform.DeepFind(Name);

            bool found = false;
            if (transform != null)
            {
                if (typeof(T).IsAssignableFrom(transform.GetType()))
                {
                    objectValue = transform;
                    found = true;
                }
                else
                {
                    var component = transform.GetComponent<T>();
                    if (component == null)
                    {
                        objectValue = component;
                        found = true;
                    }
                }
            }

            if (!found) 
            {
                Debug.LogWarning("Lookup variable "+Name+" wasn't able to find object of same name!");
                objectValue = null;
            }
        }

        private K ConvertDelegate<K>(Delegate p_delegate)
        {
            return (K)(object)p_delegate;
        }
        
#if UNITY_EDITOR
        public override bool ValueField(float p_maxWidth, IVariableBindable p_bindable)
        {
            bool invalidate = false;
            FieldInfo valueField = GetType().GetField("_value", BindingFlags.Instance | BindingFlags.NonPublic);
            if (IsBound)
            {
                GUILayout.Label(ReflectionUtils.GetTypeNameWithoutAssembly(_boundComponentName) + "." + _boundName, GUILayout.Width(p_maxWidth));
            }
            else if (IsLookup)
            {
                GUILayout.Label(GetVariableTypeShortName()+".LOOKUP", GUILayout.Width(p_maxWidth));
            }
            else
            {
                if (IsEnumProperty(valueField))
                {
                    invalidate = EnumProperty(valueField);
                } 
                else if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(ExposedReference<>))
                {
                    if (p_bindable != null)
                    {
                        invalidate = ExposedProperty(valueField, p_bindable);
                    }
                    else
                    {
                        GUILayout.Label("NOT ASSIGNALBE ON ASSET");
                    }
                }
                else if (typeof(UnityEngine.Object).IsAssignableFrom(typeof(T)))
                {
                    EditorGUI.BeginChangeCheck();
                    // Hack to work with EditorGUILayout instead of EditorGUI where ObjectField always show large preview that we don't want
                    objectValue = EditorGUILayout.ObjectField(value as UnityEngine.Object, typeof(T), false);
                    if (EditorGUI.EndChangeCheck())
                    {
                        invalidate = true;
                    }
                } 
                else
                {
                    string type = typeof(T).ToString();
                    switch (type)
                    {
                        case "System.String":
                            EditorGUI.BeginChangeCheck();
                            var stringValue = EditorGUILayout.TextField((string)valueField.GetValue(this), GUILayout.Width(p_maxWidth), GUILayout.ExpandWidth(true));
                            if (EditorGUI.EndChangeCheck())
                            {
                                invalidate = true;
                                valueField.SetValue(this, stringValue);
                            }
                            break;
                        case "System.Boolean":
                            EditorGUI.BeginChangeCheck();
                            EditorGUILayout.Space(0, true);
                            var boolValue = EditorGUILayout.Toggle((bool)valueField.GetValue(this), GUILayout.Width(16));
                            if (EditorGUI.EndChangeCheck())
                            {
                                invalidate = true;
                                valueField.SetValue(this, boolValue);
                            }
                            break;
                        case "System.Int32":
                            EditorGUI.BeginChangeCheck();
                            var intValue = EditorGUILayout.IntField((int)valueField.GetValue(this), GUILayout.Width(p_maxWidth), GUILayout.ExpandWidth(true));
                            if (EditorGUI.EndChangeCheck())
                            {
                                invalidate = true;
                                valueField.SetValue(this, intValue);
                            }
                            break;
                        case "System.Single":
                            // value = (T) Convert.ChangeType(EditorGUILayout.FloatField(Convert.ToSingle(value)),
                            //     typeof(T));
                            EditorGUI.BeginChangeCheck();
                            var floatValue = EditorGUILayout.FloatField((float)valueField.GetValue(this), GUILayout.Width(p_maxWidth), GUILayout.ExpandWidth(true));
                            if (EditorGUI.EndChangeCheck())
                            {
                                invalidate = true;
                                valueField.SetValue(this, floatValue);
                            }
                            break;
                        case "UnityEngine.Vector2":
                            // value = (T) Convert.ChangeType(
                            //     EditorGUILayout.Vector2Field("",
                            //         (Vector2) Convert.ChangeType(value, typeof(Vector2))),
                            //     typeof(T));
                            EditorGUI.BeginChangeCheck();
                            var vector2Value = EditorGUILayout.Vector2Field("", (Vector2) valueField.GetValue(this), GUILayout.Width(p_maxWidth), GUILayout.ExpandWidth(true));
                            if (EditorGUI.EndChangeCheck())
                            {
                                invalidate = true;
                                valueField.SetValue(this, vector2Value);
                            }
                            break;
                        case "UnityEngine.Vector3":
                            EditorGUI.BeginChangeCheck();
                            var vector3Value = EditorGUILayout.Vector3Field("", (Vector3) valueField.GetValue(this), GUILayout.Width(p_maxWidth), GUILayout.ExpandWidth(true));
                            if (EditorGUI.EndChangeCheck())
                            {
                                invalidate = true;
                                valueField.SetValue(this, vector3Value);
                            }
                            break;
                        case "UnityEngine.Vector4":
                            EditorGUI.BeginChangeCheck();
                            var vector4Value = EditorGUILayout.Vector4Field("", (Vector4) valueField.GetValue(this), GUILayout.Width(p_maxWidth), GUILayout.ExpandWidth(true));
                            if (EditorGUI.EndChangeCheck())
                            {
                                invalidate = true;
                                valueField.SetValue(this, vector4Value);
                            }
                            break;
                        case "UnityEngine.Quaternion":
                            EditorGUI.BeginChangeCheck();
                            Quaternion q = (Quaternion) valueField.GetValue(this);
                            Vector4 v4 = new Vector4(q.x, q.y, q.z, q.w);
                            v4 = EditorGUILayout.Vector4Field("", v4, GUILayout.Width(p_maxWidth), GUILayout.ExpandWidth(true));
                            if (EditorGUI.EndChangeCheck())
                            {
                                valueField.SetValue(this, new Quaternion(v4.x, v4.y, v4.z, v4.w));
                            }
                            break;
                        case "UnityEngine.Color":
                            EditorGUI.BeginChangeCheck();
                            var colorValue = EditorGUILayout.ColorField((Color)valueField.GetValue(this));
                            if (EditorGUI.EndChangeCheck())
                            {
                                invalidate = true;
                                valueField.SetValue(this, colorValue);
                            }
                            break;
                        default:
                            GUILayout.Label("UI not supported.");
                            break;
                    }
                }
            }

            return invalidate;
        }
        
        bool IsEnumProperty(FieldInfo p_fieldInfo)
        {
            return p_fieldInfo.FieldType.IsEnum;
        }

        bool EnumProperty(FieldInfo p_fieldInfo)
        {
            if (!IsEnumProperty(p_fieldInfo))
                return false;
            
            EditorGUI.BeginChangeCheck();
            
            var newValue = EditorGUILayout.EnumPopup((Enum) p_fieldInfo.GetValue(this));

            if (EditorGUI.EndChangeCheck())
            {
                p_fieldInfo.SetValue(this, newValue);
                return true;
            }

            return false;
        }
        
        bool ExposedProperty(FieldInfo p_fieldInfo, IVariableBindable p_bindable)
        {
            IExposedPropertyTable propertyTable = p_bindable;
            var exposedReference = p_fieldInfo.GetValue(this);
            
            PropertyName exposedName = (PropertyName)exposedReference.GetType().GetField("exposedName").GetValue(exposedReference);
            bool isDefault = PropertyName.IsNullOrEmpty(exposedName);
            
            EditorGUI.BeginChangeCheck();
            
            UnityEngine.Object exposedValue = (UnityEngine.Object)exposedReference.GetType().GetMethod("Resolve")
                .Invoke(exposedReference, new object[] {propertyTable});
            var newValue = EditorGUILayout.ObjectField(exposedValue, p_fieldInfo.FieldType.GetGenericArguments()[0], true);

            if (EditorGUI.EndChangeCheck())
            {
                if (propertyTable != null)
                {
                    Undo.RegisterCompleteObjectUndo(propertyTable as UnityEngine.Object, "Set Exposed Property");
                }

                if (!isDefault)
                {
                    if (newValue == null)
                    {
                        propertyTable.ClearReferenceValue(exposedName);   
                        exposedReference.GetType().GetField("exposedName").SetValue(exposedReference, null);
                        p_fieldInfo.SetValue(this, exposedReference);
                    }
                    else
                    {
                        propertyTable.SetReferenceValue(exposedName, newValue);
                    }
                }
                else
                {
                    if (newValue != null)
                    {
                        PropertyName newExposedName = new PropertyName(GUID.Generate().ToString());
                        exposedReference.GetType().GetField("exposedName")
                            .SetValue(exposedReference, newExposedName);
                        propertyTable.SetReferenceValue(newExposedName, newValue);
                        p_fieldInfo.SetValue(this, exposedReference);
                    }
                }

                return true;
            }

            return false;
        }
#endif
    }
}