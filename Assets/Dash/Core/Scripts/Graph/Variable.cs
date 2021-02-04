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
#endif

namespace Dash
{
    [Serializable]
    public abstract class Variable
    {
        abstract protected object objectValue { get; set; }
        
        abstract public bool IsBound { get; }

        public object value
        {
            get { return objectValue; }
            set { objectValue = value; }
        }

        [SerializeField, HideInInspector]
        protected string _name;

        public string Name => _name;

        public void Rename(string p_newName)
        {
            _name = p_newName;
        }
        
        abstract public void BindProperty(PropertyInfo prop, Component p_component);
        
        abstract public void UnbindProperty();

        abstract public void InitializeBinding(GameObject p_target);

        public abstract Variable Clone();

#if UNITY_EDITOR
        public abstract void PropertyField(Rect p_position);
        
        static public Type[] SupportedTypes =
            new Type[]
            {
                typeof(int),
                typeof(float),
                typeof(Vector3),
                typeof(Vector2),
                typeof(Quaternion)
            };

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

            return typeString.Substring(typeString.LastIndexOf(".") + 1);
        }
        
#endif
    }

    [Serializable]
    public class Variable<T> : Variable
    {
        protected T _value;

        public override bool IsBound => !String.IsNullOrEmpty(_boundProperty);

        [NonSerialized]
        private Func<T> _getter;
        [NonSerialized]
        private Action<T> _setter;
        
        // Tried to use Type/MethodInfo directly but it is not good for serialization so using string
        private string _boundProperty;
        private string _boundComponentName;

        new public T value
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
            return v;
        }
        
        public override void BindProperty(PropertyInfo p_property, Component p_component)
        {
            _boundProperty = p_property.Name;
            
            _boundComponentName = p_component.GetType().FullName;

            InitializeBinding(p_component.gameObject);
        }

        public override void UnbindProperty()
        {
            _boundProperty = String.Empty;
            _boundComponentName = String.Empty;
            _getter = null;
            _setter = null;
        }

        public override void InitializeBinding(GameObject p_target)
        {
            if (!IsBound)
                return;

            Type componentType = ReflectionUtils.GetType(_boundComponentName);
            Component component = p_target.GetComponent(componentType);
            if (component == null)
                Debug.LogWarning("Cannot find component " + _boundComponentName + " for variable " + Name);

            PropertyInfo property = componentType.GetProperty(_boundProperty);
            if (property == null)
                Debug.LogWarning("Cannot find property " + _boundProperty+" on component "+component.name);
            
            var method = property.GetGetMethod();
            var delegateGetType = typeof(Func<>).MakeGenericType(method.ReturnType);

            _getter = ConvertDelegate<Func<T>>(Delegate.CreateDelegate(delegateGetType, component, method, true));

            var setMethod = property.GetSetMethod();
            if (setMethod != null)
            {
                var delegateSetType = typeof(Action<>).MakeGenericType(method.ReturnType);
                _setter = ConvertDelegate<Action<T>>(Delegate.CreateDelegate(delegateSetType, component, setMethod, true));
            }
        }

        private K ConvertDelegate<K>(Delegate p_delegate)
        {
            return (K)(object)p_delegate;
        }
        
#if UNITY_EDITOR
        public override void PropertyField(Rect p_rect)
        {
            if (IsBound)
            {
                EditorGUI.LabelField(p_rect, ReflectionUtils.GetTypeNameWithoutAssembly(_boundComponentName) + "." + _boundProperty);
            }
            else
            {
                string type = typeof(T).ToString();
                switch (type)
                {
                    case "System.Int32":
                        value = (T) Convert.ChangeType(EditorGUI.IntField(p_rect, Convert.ToInt32(value)),
                            typeof(T));
                        break;
                    case "System.Single":
                        value = (T) Convert.ChangeType(EditorGUI.FloatField(p_rect, Convert.ToSingle(value)),
                            typeof(T));
                        break;
                    case "UnityEngine.Vector2":
                        value = (T) Convert.ChangeType(
                            EditorGUI.Vector2Field(p_rect, "", (Vector2) Convert.ChangeType(value, typeof(Vector2))),
                            typeof(T));
                        break;
                    case "UnityEngine.Vector3":
                        value = (T) Convert.ChangeType(
                            EditorGUI.Vector3Field(p_rect, "", (Vector3) Convert.ChangeType(value, typeof(Vector3))),
                            typeof(T));
                        break;
                    case "UnityEngine.Quaternion":
                        Quaternion q = (Quaternion) Convert.ChangeType(value, typeof(Quaternion));
                        Vector4 v4 = new Vector4(q.x, q.y, q.z, q.w);
                        v4 = EditorGUI.Vector4Field(p_rect, "", v4);
                        value = (T) Convert.ChangeType(new Quaternion(v4.x, v4.y, v4.z, v4.w), typeof(T));
                        break;
                    case "UnityEngine.Transform":
                        value = (T) Convert.ChangeType(
                            EditorGUILayout.ObjectField((Transform) Convert.ChangeType(value, typeof(Transform)),
                                typeof(Transform), false), typeof(T));
                        break;
                    default:
                        Debug.Log(type);
                        break;
                }
            }
        }
#endif
    }
}