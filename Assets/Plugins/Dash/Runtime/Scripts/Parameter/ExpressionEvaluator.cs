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
        static public bool hasErrorInEvaluation { get; protected set; } = false;
        static public string errorMessage;

        public static object EvaluateTypedExpression(string p_expression, Type p_returnType, IParameterResolver p_resolver, IAttributeDataCollection p_collection = null)
        {
            MethodInfo method = typeof(ExpressionEvaluator).GetMethod("EvaluateExpression", BindingFlags.Public | BindingFlags.Static);
            MethodInfo generic = method.MakeGenericMethod(p_returnType);
            return generic.Invoke(null, new object[] { p_expression, p_resolver, p_collection });
        }
        
        public static object EvaluateUntypedExpression(string p_expression, IParameterResolver p_resolver, IAttributeDataCollection p_collection = null)
        {
            hasErrorInEvaluation = false;
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
            
            EvaluateFunctionHandler evalFunction = (name, args) => EvaluateFunction(name, args);
            cachedExpression.EvaluateFunction += evalFunction; 
            EvaluateParameterHandler evalParam = (name, args) => EvaluateParameter(name, args, p_resolver, p_collection);
            cachedExpression.EvaluateParameter += evalParam;
            
            object obj = null;
            try
            {
                obj = cachedExpression.Evaluate();
            }
            catch //(Exception e)
            {
                //Debug.Log(e);
                hasErrorInEvaluation = true;
            }
            
            cachedExpression.EvaluateFunction -= evalFunction;
            cachedExpression.EvaluateParameter -= evalParam;

            return obj;
        }
        
        public static T EvaluateExpression<T>(string p_expression, IParameterResolver p_resolver, IAttributeDataCollection p_collection = null)
        {
            hasErrorInEvaluation = false;
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
            
            object obj = null;
            try
            {
                obj = cachedExpression.Evaluate();
            }
            catch (Exception e)
            {
                errorMessage = e.Message; 
                hasErrorInEvaluation = true;
            }

            cachedExpression.EvaluateFunction -= evalFunction;
            cachedExpression.EvaluateParameter -= evalParam;
            
            if (obj != null)
            {
                if (typeof(T).IsAssignableFrom(obj.GetType()))
                {
                    return (T) obj;    
                }
                
                if (typeof(T).IsImplicitlyAssignableFrom(obj.GetType()))
                {
                    return (T) Convert.ChangeType(obj, typeof(T));
                }

                Debug.LogWarning("Invalid expression casting " + obj.GetType() + " and " + typeof(T));
            }

            return default(T);
        }

        static void EvaluateParameter(string p_name, ParameterArgs p_args, IParameterResolver p_resolver, IAttributeDataCollection p_collection)
        {
            p_args.Result = p_resolver.Resolve(p_name, p_collection);
            // Only log first error
            if (!hasErrorInEvaluation && p_resolver.hasErrorInResolving)
            {
                errorMessage = p_resolver.errorMessage;
            }
            hasErrorInEvaluation = hasErrorInEvaluation || p_resolver.hasErrorInResolving;
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
                if (!hasErrorInEvaluation && !success)
                {
                    errorMessage = ExpressionFunctions.errorMessage;
                }
                hasErrorInEvaluation = hasErrorInEvaluation || !success;
            }
            else
            {
                errorMessage = "Function " + p_name + " not found";
                hasErrorInEvaluation = true;
            }
        }
        
        static void EvaluateFunction(string p_name, FunctionArgs p_args)
        {
            MethodInfo methodInfo = typeof(ExpressionFunctions).GetMethod(p_name, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public);

            if (methodInfo != null)
            {
                // TODO maybe typize the args but it would probably just slow things down
                bool success = (bool)methodInfo.Invoke(null, new object[] {p_args});
                if (!hasErrorInEvaluation && !success)
                {
                    errorMessage = ExpressionFunctions.errorMessage;
                }
                hasErrorInEvaluation = hasErrorInEvaluation || !success;
            }
            else
            {
                errorMessage = "Function " + p_name + " not found";
                hasErrorInEvaluation = true;
            }
        }
    }
}