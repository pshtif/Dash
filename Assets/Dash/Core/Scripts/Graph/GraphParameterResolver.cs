/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using OdinSerializer.Utilities;
using UnityEngine;

namespace Dash
{
    public class GraphParameterResolver : IParameterResolver
    {
        protected DashGraph _graph;
        public bool hasErrorInExecution { get; private set; } = false;
        
        public GraphParameterResolver(DashGraph p_graph)
        {
            _graph = p_graph;
        }

        public object Resolve(string p_name, IAttributeDataCollection p_collection)
        {
            object result;
            if (ResolveReservedVariable(p_name, out result))
            {
                return result;
            }

            if (p_name.StartsWith("g_"))
            {
                string name = p_name.Substring(2);
                
                if (_graph.variables.HasVariable(name))
                {
                    Variable variable = _graph.variables.GetVariable(name);
                    return variable.value;
                }
            }
            
            if (p_collection != null)
            {
                if (p_collection.HasAttribute(p_name))
                {
                    return p_collection.GetAttribute(p_name);
                }
            }

            Debug.LogWarning("Variable "+ p_name +" not found.");
            hasErrorInExecution = true;
            return null;
        }

        protected bool ResolveReservedVariable(string p_name, out object p_result)
        {
            if (p_name == "mousePosition")
            {
                p_result = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                return true;
            }
            
            p_result = null;
            return false;
        }

        public T Resolve<T>(string p_name, IAttributeDataCollection p_collection)
        {
            if (p_name.StartsWith("g_"))
            {
                string name = p_name.Substring(2);
                
                if (_graph.variables.HasVariable(name))
                {
                    Variable<T> variable = _graph.variables.GetVariable<T>(name);
                    return variable.value;
                }
            }
            
            if (p_collection != null)
            {
                if (p_collection.HasAttribute(p_name))
                {
                    return p_collection.GetAttribute<T>(p_name);
                }
            }

            Debug.LogWarning("Variable "+ p_name +" not found.");
            hasErrorInExecution = true;
            return default(T);
        }
    }
}