/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Reflection;
using UnityEngine;
using NCalc;
using Random = UnityEngine.Random;
using Dash.Extensions;

namespace Dash
{

    [Serializable]
    public abstract class Parameter
    {
        [HideInInspector]
        public bool isExpression = false;
        
        [HideInInspector]
        public string expression;

        // True if last evaluation was erroneous
        public bool hasError = false;

        public abstract FieldInfo GetValueFieldInfo();
    }

    [Serializable]
    public class Parameter<T> : Parameter
    {
        protected T _value;

        [NonSerialized] 
        private string _previousExpression;
        
        public Parameter(T p_value)
        {
            _value = p_value;
        }

        public override FieldInfo GetValueFieldInfo()
        {
            return GetType().GetField("_value", BindingFlags.NonPublic | BindingFlags.Instance);
        }
        
        public T GetValue(IParameterResolver p_resolver, IAttributeDataCollection p_collection = null)
        {
            if (isExpression)
            {
                // TODO Maybe do this directly after editing?
                expression = expression.RemoveWhitespace();
                if (string.IsNullOrEmpty(expression))
                {
                    Debug.LogWarning("Expression cannot be empty returning default value.");
                    return default(T);
                }
                
                return ExpressionEvaluator.EvaluateExpression<T>(expression, p_resolver, p_collection);
            }
            
            return _value;
        }

        public void SetValue(T p_value)
        {
            _value = p_value;
        }
        
        public override string ToString()
        {
            return isExpression ? expression : _value != null ? _value.ToString() : "NULL";
        }
    }
}