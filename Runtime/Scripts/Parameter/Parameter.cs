/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Dash
{

    [Serializable]
    public abstract class Parameter
    {
        abstract protected object objectValue { get; set; }
        
        public object value
        {
            get { return objectValue; }
            set { objectValue = value; }
        }
        
        [HideInInspector]
        public bool isExpression = false;
        
        [HideInInspector]
        public string expression;

        // True if last evaluation was erroneous
        [NonSerialized]
        public bool hasErrorInEvaluation = false;
        [NonSerialized]
        public string errorMessage;

        // [SerializeField]
        // protected bool _debug = false;
        // [SerializeField]
        // protected string _debugName;
        // [SerializeField]
        // protected string _debugId;

        public bool IsDebug()
        {
            return false;//_debug;
        }

        public void SetDebug(bool p_enable, string p_debugName = "", string p_debugId = "")
        {
            // _debug = p_enable;
            // _debugName = p_debugName;
            // _debugId = p_debugId;
        }

        public abstract bool IsDefault();

        public abstract FieldInfo GetValueFieldInfo();

        public abstract Type GetValueType();

        public abstract void SetValueToDefault();

        static protected List<Parameter> _referenceChain = new List<Parameter>();
        
        public bool IsInReferenceChain(Parameter p_parameter)
        {
            return _referenceChain.Contains(p_parameter);
        }
        
        public static Parameter Create(Type p_type)
        {
            return (Parameter)Activator.CreateInstance(typeof(Parameter<>).MakeGenericType(p_type),
                new object[] { p_type.GetDefaultValue() });
        }

        public static Parameter<T> Create<T>()
        {
            return new Parameter<T>(default(T));
        }
        
        public object GetUntypedValue(IParameterResolver p_resolver, IAttributeDataCollection p_collection = null, int p_index = 0, bool p_referenced = false)
        {
            if (!p_referenced)
            {
                _referenceChain.Clear();
            }
            else
            {
                _referenceChain.Add(this);
            }
            
            if (isExpression)
            {
                expression = expression.RemoveWhitespace();
                if (string.IsNullOrEmpty(expression))
                {
                    errorMessage = "Expression cannot be empty returning default value.";
                    return null;
                }
                
                object expressionValue = ExpressionEvaluator.EvaluateUntypedExpression(expression, p_resolver, p_collection, p_referenced);
                if (ExpressionEvaluator.hasErrorInEvaluation)
                {
                    errorMessage = ExpressionEvaluator.errorMessage;
                    hasErrorInEvaluation = true;
                }
                else
                {
                    hasErrorInEvaluation = false;
                }
        
                return expressionValue;
            }
        
            hasErrorInEvaluation = false;
            return value;
        }
    }

    [Serializable]
    public class Parameter<T> : Parameter
    {
        internal T _value;
        
        public new T value
        {
            get
            {
                return _value; 
            }
            set
            {
                _value = value;
            }
        }
        
        protected override object objectValue
        {
            get { return value; }
            set { this.value = (T)value; }
        }
        
        public Parameter(T p_value)
        {
            _value = p_value;
        }

        public override bool IsDefault()
        {
            return EqualityComparer<T>.Default.Equals(_value, default(T));
        } 

        public override FieldInfo GetValueFieldInfo()
        {
            return GetType().GetField("_value", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public override Type GetValueType()
        {
            return typeof(T);
        }
        
        public T GetValue(IParameterResolver p_resolver, IAttributeDataCollection p_collection = null, bool p_referenced = false)
        {
            if (!p_referenced)
            {
                _referenceChain.Clear();
            }
            else
            {
                _referenceChain.Add(this);
            }
            
            if (isExpression)
            {
                if (string.IsNullOrWhiteSpace(expression))
                {
                    Debug.LogWarning("Expression cannot be empty returning default value.");
                    return default(T);
                }

                T value = ExpressionEvaluator.EvaluateExpression<T>(expression, p_resolver, p_collection, p_referenced);
                if (ExpressionEvaluator.hasErrorInEvaluation)
                {
                    errorMessage = ExpressionEvaluator.errorMessage;
                    hasErrorInEvaluation = true;
                }
                else
                {
                    hasErrorInEvaluation = false;
                }

//                 if (_debug)
//                 {
// #if UNITY_EDITOR
//                     DashEditorDebug.Debug(new CustomDebugItem("[Parameter debug] Name: "+_debugName+" Value: "+value));
// #endif
//                     Debug.Log("["+_debugId+"] "+_debugName+" = "+value);
//                 }

                return value;
            }
            
	        hasErrorInEvaluation = false;
            return _value;
        }

        public void SetValue(T p_value)
        {
            _value = p_value;
        }

        public override void SetValueToDefault()
        {
            _value = default(T);
        }
        
        public override string ToString()
        {
            return isExpression ? expression : _value != null ? _value.ToString() : "NULL";
        }
    }
}