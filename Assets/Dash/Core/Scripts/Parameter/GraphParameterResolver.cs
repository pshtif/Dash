/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System.Reflection;
using OdinSerializer.Utilities;
using UnityEngine;

namespace Dash
{
    public class GraphParameterResolver : IParameterResolver
    {
        protected DashGraph _graph;
        public bool hasErrorInResolving { get; private set; } = false;
        public string errorMessage { get; private set; }
        
        public GraphParameterResolver(DashGraph p_graph)
        {
            _graph = p_graph;
        }

        public object Resolve(string p_name, IAttributeDataCollection p_collection)
        {
            hasErrorInResolving = false;

            if (p_collection != null)
            {
                if (p_collection.HasAttribute(p_name))
                {
                    return p_collection.GetAttribute(p_name);
                }
            }
            
            object result;
            if (ReservedVariables.Resolve(_graph, p_name, out result))
            {
                return result;                
            }
            
            if (ResolveReference(p_name, p_collection, out result))
                return result;
            
            if (_graph.variables.HasVariable(p_name))
            {
                Variable variable = _graph.variables.GetVariable(p_name);
                return variable.value;
            }

            if (DashCore.Instance.globalVariables != null && DashCore.Instance.globalVariables.HasVariable(p_name))
            {
                Variable variable = DashCore.Instance.globalVariables.GetVariable(p_name);
                return variable.value;
            }

            hasErrorInResolving = true;
            errorMessage = "Variable "+ p_name +" not found.";
            return null;
        }

        // public T Resolve<T>(string p_name, IAttributeDataCollection p_collection)
        // {
        //     hasErrorInResolving = false;
        //
        //     if (p_collection != null)
        //     {
        //         if (p_collection.HasAttribute(p_name))
        //         {
        //             return p_collection.GetAttribute<T>(p_name);
        //         }
        //     }
        //     
        //     object result;
        //     if (ResolveReservedVariable(p_name, out result))
        //         return (T)result;
        //
        //     if (ResolveReference(p_name, p_collection, out result))
        //         return (T)result;
        //
        //     if (_graph.variables.HasVariable(p_name))
        //     {
        //         Variable<T> variable = _graph.variables.GetVariable<T>(p_name);
        //         return variable.value;
        //     }
        //
        //     hasErrorInResolving = true;
        //     errorMessage = "Variable/Attribute "+ p_name +" not found.";
        //     return default(T);
        // }

        bool ResolveReference(string p_name, IAttributeDataCollection p_collection, out object p_result)
        {
            if (!p_name.StartsWith("$"))
            {
                p_result = null;
                return false;
            }

            string name = p_name.Substring(1);
            string[] split = name.Split('.');
            
            object value = _graph.GetNodeById(split[0]).GetModel();

            if (value == null || split.Length == 1)
            {
                p_result = value;
                return true;
            }

            for (int i = 1; i < split.Length; i++)
            {
                FieldInfo fieldInfo = value.GetType().GetField(split[i]);
                value = fieldInfo.GetValue(value);
                
                if (typeof(Parameter).IsAssignableFrom(value.GetType()))
                {
                    value = value.GetType().GetMethod("GetValue").Invoke(value, new object[] {this, p_collection});
                }
            }
            
            if (value.GetType().IsGenericType && value.GetType().GetGenericTypeDefinition() == typeof(ExposedReference<>))
            {
                value = value.GetType().GetMethod("Resolve").Invoke(value, new object[] { _graph.Controller });
            }
    
            p_result = value;
            return true;
        }
    }
}