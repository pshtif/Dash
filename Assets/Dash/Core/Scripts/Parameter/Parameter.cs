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

        [NonSerialized] 
        private Expression _cachedExpression;
        
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
                
                Debug.Log(EvaluateExpression(p_resolver, p_collection));
                return EvaluateExpression(p_resolver, p_collection);
            }
            
            return _value;
        }

        private T EvaluateExpression(IParameterResolver p_resolver, IAttributeDataCollection p_collection = null)
        {
            // Debug.Log("Parameter<"+typeof(T)+">.EvaluateExpression");
            // Debug.Log(expression);

            if (_previousExpression != expression)
            {
                _cachedExpression = new Expression(expression);
                _previousExpression = expression;
            }
            
            _cachedExpression.EvaluateFunction += EvaluateFunction;
            EvaluateParameterHandler evalParam = (name, args) => EvaluateParameter(name, args, p_resolver, p_collection);
            _cachedExpression.EvaluateParameter += evalParam;

            object obj = null;
            try
            {
                obj = _cachedExpression.Evaluate();
            }
            catch (Exception e)
            {
                //Debug.Log(e);
                hasError = true;
            }
            
            _cachedExpression.EvaluateFunction -= EvaluateFunction;
            _cachedExpression.EvaluateParameter -= evalParam;
            
            if (obj != null && (typeof(T).IsAssignableFrom(obj.GetType()) || typeof(T).IsImplicitlyAssignableFrom(obj.GetType())))
            {
                return (T) Convert.ChangeType(obj, typeof(T));
            }

            return default(T);
        }

        void EvaluateParameter(string p_name, ParameterArgs p_args, IParameterResolver p_resolver, IAttributeDataCollection p_collection)
        {
            p_args.Result = p_resolver.Resolve(p_name, p_collection);
        }

        void EvaluateFunction(string p_name, FunctionArgs p_args)
        {
            MethodInfo methodInfo = typeof(ExpressionFunctions).GetMethod(p_name, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public);

            if (methodInfo != null)
            {
                // Can be generic using parameter type
                if (methodInfo.IsGenericMethod)
                {
                    methodInfo = methodInfo.MakeGenericMethod(typeof(T));
                }
                
                // TODO maybe typize the args but it would probably just slow things down
                bool success = (bool)methodInfo.Invoke(null, new object[] {p_args});
                hasError = !success;
            }
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