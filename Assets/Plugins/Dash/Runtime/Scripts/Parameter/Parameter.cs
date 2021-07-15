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
        [NonSerialized]
        public bool hasErrorInEvaluation = false;
        [NonSerialized]
        public string errorMessage;

        public abstract FieldInfo GetValueFieldInfo();
    }

    [Serializable]
    public class Parameter<T> : Parameter
    {
        protected T _value;
        
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
                if (string.IsNullOrWhiteSpace(expression))
                {
                    Debug.LogWarning("Expression cannot be empty returning default value.");
                    return default(T);
                }
                
                T value = ExpressionEvaluator.EvaluateExpression<T>(expression, p_resolver, p_collection);
                if (ExpressionEvaluator.hasErrorInEvaluation)
                {
                    errorMessage = ExpressionEvaluator.errorMessage;
                    hasErrorInEvaluation = true;
                }
                else
                {
                    hasErrorInEvaluation = false;
                }

                return value;
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