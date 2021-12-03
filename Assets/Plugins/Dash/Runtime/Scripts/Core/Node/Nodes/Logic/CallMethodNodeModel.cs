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
        public string componentName = "";
        [HideInInspector]
        public string methodName = "";

        public bool includeNonPublic = false;

        public bool includeStatic = false;

        public bool includeInherited = false;

        public string shortComponentName => componentName.IndexOf('.') == -1
             ? componentName
             : componentName.Substring(componentName.LastIndexOf('.') + 1);
        
#if UNITY_EDITOR

        protected override void DrawCustomInspector()
        {
            var style = new GUIStyle();
            style.alignment = TextAnchor.UpperCenter;
            style.normal.textColor = Color.white;
            //GUILayout.Label("Method");
            //GUILayout.BeginHorizontal();
            if (componentName.IsNullOrWhitespace())
            {
                GUILayout.Label("No Method Bound", style);
                
                if (GUILayout.Button("Bind Method"))
                {
                    GetMethodMenu(DashEditorCore.EditorConfig.editingGraph.Controller.gameObject).ShowAsContext();
                }
            }
            else
            {
                GUILayout.Label(shortComponentName+"."+methodName, style);
                
                if (GUILayout.Button("Unbind Method"))
                {
                    OnUnbind();
                }
            }

            //GUILayout.EndHorizontal();
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
                    GetBindableMethods(p_boundObject, includeInherited, includeStatic, includeNonPublic);
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
        
        static Dictionary<Component, List<MethodInfo>> GetBindableMethods(GameObject p_boundObject, bool p_includeInherited, bool p_includeStatic, bool p_includeNonPublic)
        {
            Dictionary<Component, List<MethodInfo>> bindableMethods = new Dictionary<Component, List<MethodInfo>>();

            if (p_boundObject != null)
            {
                Component[] components = p_boundObject.GetComponents<Component>();
                foreach (Component component in components)
                {
                    Type componentType = component.GetType();
                    var flags = BindingFlags.Instance | BindingFlags.Public;
                    if (!p_includeInherited) flags = flags | BindingFlags.DeclaredOnly;
                    if (p_includeNonPublic) flags = flags | BindingFlags.NonPublic;
                    if (p_includeStatic) flags = flags | BindingFlags.Static;
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
#endif
    }
}