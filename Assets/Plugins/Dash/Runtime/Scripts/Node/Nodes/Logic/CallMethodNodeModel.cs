/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using Dash.Attributes;
using OdinSerializer.Utilities;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Dash
{
    [CustomInspector]
    public class CallMethodNodeModel : NodeModelBase
    {
        [HideInInspector]
        public string componentName;
        [HideInInspector]
        public string methodName;
        
#if UNITY_EDITOR

        protected override void DrawCustomInspector()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Method", GUILayout.Width(120));
            if (componentName.IsNullOrWhitespace())
            {
                if (GUILayout.Button("Bind"))
                {
                    GetMethodMenu(DashEditorCore.Config.editingGraph.Controller.gameObject).ShowAsContext();
                }
            }
            else
            {
                if (GUILayout.Button("Unbind"))
                {
                    OnUnbind();
                }
            }

            GUILayout.EndHorizontal();
        }

        void OnBindMethod(MethodInfo p_method, Component p_component)
        {
            componentName = p_component.GetType().FullName;
            methodName = p_method.Name;
        }

        void OnUnbind()
        {
            componentName = methodName = "";
        }
        
        GenericMenu GetMethodMenu(GameObject p_boundObject)
        {
            GenericMenu menu = new GenericMenu();
            
            if (p_boundObject != null)
            {
                Dictionary<Component, List<MethodInfo>> bindableMethods =
                    GetBindableMethods(p_boundObject, true);
                foreach (var infoKeys in bindableMethods)
                {
                    foreach (MethodInfo method in infoKeys.Value)
                    {
                        menu.AddItem(new GUIContent(infoKeys.Key.GetType() + "/" + method.Name),
                            false,
                            () => OnBindMethod(method, infoKeys.Key));
                    }
                }
            }

            return menu;
        }
        
        static Dictionary<Component, List<MethodInfo>> GetBindableMethods(GameObject p_boundObject, bool p_declaredOnly)
        {
            Dictionary<Component, List<MethodInfo>> bindableMethods = new Dictionary<Component, List<MethodInfo>>();

            if (p_boundObject != null)
            {
                Component[] components = p_boundObject.GetComponents<Component>();
                foreach (Component component in components)
                {
                    Type componentType = component.GetType();
                    var flags = BindingFlags.Instance | BindingFlags.Public;
                    if (p_declaredOnly) flags = flags | BindingFlags.DeclaredOnly;
                    MethodInfo[] methods = componentType.GetMethods(flags);
                    foreach (MethodInfo method in methods)
                    {
                        if (method.GetParameters().Length > 0 || method.IsGetAccessor())
                            continue;

                        if (!bindableMethods.ContainsKey(component))
                        {
                            bindableMethods.Add(component, new List<MethodInfo>());
                        }
                        bindableMethods[component].Add(method);
                    }
                }
            }

            return bindableMethods;
        }
    }
#endif
}