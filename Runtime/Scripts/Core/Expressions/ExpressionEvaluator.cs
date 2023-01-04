/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Dash.Extensions;
using Dash.NCalc;
using Machina.Attributes;
using UnityEngine;

namespace Dash
{
    public class ExpressionEvaluator
    {
        protected static Dictionary<string,Expression> _cachedExpressions;
        protected static Dictionary<string, MethodInfo> _cachedMethods = new Dictionary<string, MethodInfo>();
        
        static public bool hasErrorInEvaluation { get; protected set; } = false;
        static public string errorMessage;

        public static void ClearExpressionCache()
        {
            _cachedExpressions = new Dictionary<string, Expression>();
        }

        public static object EvaluateTypedExpression(string p_expression, Type p_returnType, IParameterResolver p_resolver, IAttributeDataCollection p_collection = null)
        {
            MethodInfo method = typeof(ExpressionEvaluator).GetMethod("EvaluateExpression", BindingFlags.Public | BindingFlags.Static);
            MethodInfo generic = method.MakeGenericMethod(p_returnType);
            return generic.Invoke(null, new object[] { p_expression, p_resolver, p_collection, false });
        }
        
        public static object EvaluateUntypedExpression(string p_expression, IParameterResolver p_resolver, IAttributeDataCollection p_collection, bool p_referenced)
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
            EvaluateParameterHandler evalParam = (name, args) => EvaluateParameter(name, args, p_resolver, p_collection, p_referenced);
            cachedExpression.EvaluateParameter += evalParam;
            
            object obj = null;
            try
            {
                obj = cachedExpression.Evaluate();
            }
            catch (Exception e)
            {
                // Only set if we didn't already encounter error in evaluation otherwise this may be unspecified exception as a result of the already logged error so we don't want to overwrite it
                if (!hasErrorInEvaluation)
                {
                    errorMessage = e.Message;
                    hasErrorInEvaluation = true;
                }
            }
            
            cachedExpression.EvaluateFunction -= evalFunction;
            cachedExpression.EvaluateParameter -= evalParam;

            return obj;
        }

        public static T EvaluateExpression<T>(string p_expression, IParameterResolver p_resolver, IAttributeDataCollection p_collection, bool p_referenced)
        {
            hasErrorInEvaluation = false;
            if (_cachedExpressions == null) _cachedExpressions = new Dictionary<string, Expression>();

            Expression cachedExpression;
            if (!_cachedExpressions.ContainsKey(p_expression))
            {
                // We cache after macro replacement so runtime macro changes are not possible for performance reasons
                cachedExpression = new Expression(ReplaceMacros(p_expression));
                _cachedExpressions.Add(p_expression, cachedExpression);
            }
            else
            {
                cachedExpression = _cachedExpressions[p_expression];
            }
            
            EvaluateFunctionHandler evalFunction = (name, args) => EvaluateFunction<T>(name, args);
            cachedExpression.EvaluateFunction += evalFunction; 
            EvaluateParameterHandler evalParam = (name, args) => EvaluateParameter(name, args, p_resolver, p_collection, p_referenced);
            cachedExpression.EvaluateParameter += evalParam;
            
            object obj = null;
            try
            {
                obj = cachedExpression.Evaluate();
            }
            catch (Exception e)
            {
                // Only set if we didn't already encounter error in evaluation otherwise this may be unspecified exception as a result of the already logged error so we don't want to overwrite it
                if (!hasErrorInEvaluation)
                {
                    errorMessage = e.Message;
                    hasErrorInEvaluation = true;
                }
            }

            cachedExpression.EvaluateFunction -= evalFunction;
            cachedExpression.EvaluateParameter -= evalParam;
            
            if (obj != null)
            {
                Type returnType = obj.GetType();
                if (typeof(T).IsAssignableFrom(returnType))
                {
                    return (T) obj;    
                }

                // Explicit numeric type casting at cost of precision/overflow 
                if (typeof(T).IsNumericType() && returnType.IsNumericType())
                {
                    return (T) Convert.ChangeType(obj, typeof(T));
                }
                
                if (typeof(T).IsImplicitlyAssignableFrom(returnType))
                {
                    return (T) Convert.ChangeType(obj, typeof(T));
                }

                if (typeof(T) == typeof(string))
                {
                    return (T) (object) obj.ToString();
                }

                Debug.LogWarning("Invalid expression casting " + obj.GetType() + " and " + typeof(T));
            }

            return default(T);
        }

        static void EvaluateParameter(string p_name, ParameterArgs p_args, IParameterResolver p_resolver, IAttributeDataCollection p_collection, bool p_referenced)
        {
            //Debug.Log("EvaluateParameter: "+p_name);
            p_args.Result = p_resolver.Resolve(p_name, p_collection, p_referenced);
            // Only log first error
            if (!hasErrorInEvaluation && p_resolver.hasErrorInResolving)
            {
                errorMessage = p_resolver.errorMessage;
            }
            hasErrorInEvaluation = hasErrorInEvaluation || p_resolver.hasErrorInResolving;
        }

        static void EvaluateFunction<T>(string p_name, FunctionArgs p_args)
        {
            //Debug.Log("EvaluateFunction("+p_name+","+p_args+")");
            MethodInfo methodInfo = null;

            if (_cachedMethods.ContainsKey(p_name))
            {
                methodInfo = _cachedMethods[p_name];
            } else {
                methodInfo = typeof(ExpressionFunctions).GetMethod(p_name,
                    BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public);
                
                if (methodInfo == null && DashCore.Instance.Config.enableCustomExpressionClasses)
                {
                    methodInfo = GetCustomFunction(p_name);
                }
                
                if (methodInfo != null) _cachedMethods.Add(p_name, methodInfo);
            }

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
                if (!hasErrorInEvaluation)
                {
                    errorMessage = "Function " + p_name + " not found";
                }

                hasErrorInEvaluation = true;
            }
        }

        static void EvaluateFunction(string p_name, FunctionArgs p_args)
        {
            MethodInfo methodInfo = null;
            if (_cachedMethods.ContainsKey(p_name))
            {
                methodInfo = _cachedMethods[p_name];
            }
            else
            {
                methodInfo = typeof(ExpressionFunctions).GetMethod(p_name,
                    BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public);

                if (methodInfo == null && DashCore.Instance.Config.enableCustomExpressionClasses)
                {
                    methodInfo = GetCustomFunction(p_name);
                }
                
                if (methodInfo != null) _cachedMethods.Add(p_name, methodInfo);
            }

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
                if (!hasErrorInEvaluation)
                {
                    errorMessage = "Function " + p_name + " not found";
                }

                hasErrorInEvaluation = true;
            }
        }

        static MethodInfo GetCustomFunction(string p_name)
        {
            List<Type> classes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a =>
                a.GetTypes().Where(t => t.IsDefined(typeof(ClassAttributes.ExpressionFunctionsAttribute)))).ToList();

            MethodInfo methodInfo = null;
            foreach (Type type in classes)
            {
                methodInfo = type.GetMethod(p_name,
                    BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public);
                
                if (methodInfo != null)
                    break;
            }

            return methodInfo;
        }

        static string ReplaceMacros(string p_expression)
        {
            foreach (var pair in DashCore.Instance.Config.expressionMacros)
            {
                p_expression = p_expression.Replace(pair.Key, pair.Value);
            }
            
            // Check for invalid macros
            Match match = Regex.Match(p_expression, @"\{[A-Z0-9 _]+\}", RegexOptions.None);
            if (match.Success)
            {
                hasErrorInEvaluation = true;
                errorMessage = "Invalid macro found " + match.Value.Substring(1,match.Value.Length-2);
            }

            return p_expression;
        }
    }
}