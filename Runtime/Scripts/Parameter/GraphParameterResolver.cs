/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Reflection;
using Dash.Editor;
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

        public object Resolve(string p_name, IAttributeDataCollection p_collection, bool p_referenced)
        {
            hasErrorInResolving = false;

            string name = p_name;
            string[] nameSplit = null;
            if (p_name.IndexOf('.') > 0)
            {
                nameSplit = p_name.Split('.');
                name = nameSplit[0];
            }
            
            if (p_collection != null)
            {
                if (p_collection.HasAttribute(name))
                {
                    return ResolveNested(nameSplit, 0, p_collection.GetAttribute(name), p_collection, p_referenced);
                }
            }

            object result;
            if (ReservedParameters.Resolve(_graph, name, out result))
                return ResolveNested(nameSplit, 0, result, p_collection, p_referenced);

            if (ResolveReference(name, p_collection, out result))
                return ResolveNested(nameSplit, 0, result, p_collection, true);

            DashController controller = _graph.Controller;

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                controller = DashEditorCore.EditorConfig.editingController;
            }
#endif

            if (controller != null && controller.Variables != null && controller.Variables.HasVariable(name))
            {
                Variable variable = controller.Variables.GetVariable(name);
                return ResolveNested(nameSplit, 0, variable.value, p_collection, p_referenced);
            }

            if (_graph.variables.HasVariable(name))
            {
                Variable variable = _graph.variables.GetVariable(name);
                return ResolveNested(nameSplit, 0, variable.value, p_collection, p_referenced);
            }

            if (DashCore.Instance.GlobalVariables != null && DashCore.Instance.GlobalVariables.HasVariable(name))
            {
                Variable variable = DashCore.Instance.GlobalVariables.GetVariable(name);
                return ResolveNested(nameSplit, 0, variable.value, p_collection, p_referenced);
            }

            hasErrorInResolving = true;
            errorMessage = "Variable "+ name +" not found.";
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

        object ResolveNested(string[] p_properties, int p_index, object p_result, IAttributeDataCollection p_collection, bool p_referenced)
        {
            if (p_properties == null || p_result == null || p_index >= p_properties.Length-1)
                return p_result;
            
            p_index++;
            string property = p_properties[p_index];
            FieldInfo fieldInfo = p_result.GetType().GetField(property);
            
            if (fieldInfo == null)
            {
                PropertyInfo propertyInfo = p_result.GetType().GetProperty(property);

                if (propertyInfo == null)
                {
                    hasErrorInResolving = true;
                    errorMessage = "Nested property lookup " + String.Join(".", p_properties) + " not found.";
                    return null;
                }

                p_result = propertyInfo.GetValue(p_result);
            }
            else
            {
                p_result = fieldInfo.GetValue(p_result);
            }
            
            if (typeof(Parameter).IsAssignableFrom(p_result.GetType()))
            {
                Parameter parameter = p_result as Parameter;
                
                if (!parameter.IsInReferenceChain(parameter))
                {
                    p_result = p_result.GetType().GetMethod("GetValue")
                        .Invoke(p_result, new object[] { this, p_collection, p_referenced });
                }
                else
                {
                    hasErrorInResolving = true;
                    errorMessage = "Reference chain encountered.";
                    p_result = null;
                    return p_result;
                }
            }
            
            return ResolveNested(p_properties, p_index, p_result, p_collection, p_referenced);
        }

        bool ResolveReference(string p_name, IAttributeDataCollection p_collection, out object p_result)
        {
            if (!p_name.StartsWith("$"))
            {
                p_result = null;
                return false;
            }

            string name = p_name.Substring(1);

            object value = _graph.GetNodeById(name).GetModel();

            if (value != null)
            {
                p_result = value;
                return true;
            }
            
            hasErrorInResolving = true;
            errorMessage = "Invalid node reference " + name;
            
            p_result = null;
            return false;
        }
    }
}