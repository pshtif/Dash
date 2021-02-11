/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dash.Extensions;
using NCalc;
using UnityEngine;

namespace Dash
{
    public class ExpressionEvaluator
    {
        protected static Dictionary<string,Expression> _cachedExpressions;
        static public bool hasErrorInExecution { get; protected set; } = false;

        public static T EvaluateExpression<T>(string p_expression, IParameterResolver p_resolver, IAttributeDataCollection p_collection = null)
        {
            hasErrorInExecution = false;
            if (_cachedExpressions == null) _cachedExpressions = new Dictionary<string, Expression>();

            Expression cachedExpression;
            if (!_cachedExpressions.ContainsKey(p_expression))
            {
                cachedExpression = new Expression(p_expression);
                _cachedExpressions.Add(p_expression, cachedExpression);
            }
            else
            {
                cachedExpression = _cachedExpressions[p_expression];
            }
            
            EvaluateFunctionHandler evalFunction = (name, args) => EvaluateFunction<T>(name, args);
            cachedExpression.EvaluateFunction += evalFunction; 
            EvaluateParameterHandler evalParam = (name, args) => EvaluateParameter(name, args, p_resolver, p_collection);
            cachedExpression.EvaluateParameter += evalParam;

            bool error = false;
            object obj = null;
            try
            {
                obj = cachedExpression.Evaluate();
            }
            catch (Exception e)
            {
                //Debug.Log(e);
                hasErrorInExecution = true;
            }

            cachedExpression.EvaluateFunction -= evalFunction;
            cachedExpression.EvaluateParameter -= evalParam;
            
            if (obj != null && (typeof(T).IsAssignableFrom(obj.GetType()) || typeof(T).IsImplicitlyAssignableFrom(obj.GetType())))
            {
                return (T) Convert.ChangeType(obj, typeof(T));
            }

            return default(T);
        }

        static void EvaluateParameter(string p_name, ParameterArgs p_args, IParameterResolver p_resolver, IAttributeDataCollection p_collection)
        {
            p_args.Result = p_resolver.Resolve(p_name, p_collection);
            hasErrorInExecution = hasErrorInExecution || p_resolver.hasErrorInExecution;
        }

        static void EvaluateFunction<T>(string p_name, FunctionArgs p_args)
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
                hasErrorInExecution = hasErrorInExecution || !success;
            }
        }
    }
}