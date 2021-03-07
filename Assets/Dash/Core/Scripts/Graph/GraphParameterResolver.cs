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

            object result;
            if (ResolveReservedVariable(p_name, out result))
                return result;

            if (ResolveReference(p_name, p_collection, out result))
                return result;

            if (_graph.variables.HasVariable(p_name))
            {
                Variable variable = _graph.variables.GetVariable(p_name);
                return variable.value;
            }

            if (p_collection != null)
            {
                if (p_collection.HasAttribute(p_name))
                {
                    return p_collection.GetAttribute(p_name);
                }
            }

            hasErrorInResolving = true;
            errorMessage = "Variable "+ p_name +" not found.";
            return null;
        }

        public T Resolve<T>(string p_name, IAttributeDataCollection p_collection)
        {
            hasErrorInResolving = false;

            object result;
            if (ResolveReservedVariable(p_name, out result))
                return (T)result;

            if (ResolveReference(p_name, p_collection, out result))
                return (T)result;
            
            if (_graph.variables.HasVariable(p_name))
            {
                Variable<T> variable = _graph.variables.GetVariable<T>(p_name);
                return variable.value;
            }

            if (p_collection != null)
            {
                if (p_collection.HasAttribute(p_name))
                {
                    return p_collection.GetAttribute<T>(p_name);
                }
            }

            hasErrorInResolving = true;
            errorMessage = "Variable/Attribute "+ p_name +" not found.";
            return default(T);
        }

        protected bool ResolveReservedVariable(string p_name, out object p_result)
        {
            if (p_name == "controller")
            {
                p_result = _graph.Controller.transform;
                return true;
            }
            
            if (p_name == "mousePosition")
            {
                p_result = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                return true;
            }
            
            p_result = null;
            return false;
        }
        
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

            p_result = value;
            return true;
        }
    }
}