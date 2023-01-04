/*
 *	Created by:  Peter @sHTiF Stefcek
 */
#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Dash.Editor
{
    public class VariableTypesMenu
    {
        static private Type[] SupportedBasicTypes =
        {
            typeof(bool),
            typeof(string),
            typeof(int),
            typeof(float),
            typeof(Vector4),
            typeof(Vector3),
            typeof(Vector2),
            typeof(Quaternion)
        };
        
        static private Type[] SupportedUnityTypes =
        {
            typeof(EaseType),
            typeof(Sprite),
            typeof(RectTransform),
            typeof(Transform),
            typeof(GameObject),
            typeof(Button),
            typeof(Color),
            typeof(Canvas),
        };

        // static private Type[] SupportedExposedTypes =
        // {
        //     typeof(ExposedReference<RectTransform>),
        //     typeof(ExposedReference<Transform>)
        // };
            
        #if UNITY_EDITOR
        public static void Show(Action<Type> p_callback)
        {
            GenericMenu menu = new GenericMenu();

            foreach (Type type in SupportedBasicTypes)
            {
                menu.AddItem(new GUIContent("Basic/" + Variable.ConvertToTypeName(type)), false, () => p_callback(type));
            }
            
            foreach (Type type in SupportedUnityTypes)
            {
                menu.AddItem(new GUIContent("Unity/" + Variable.ConvertToTypeName(type)), false, () => p_callback(type));
            }
            
            menu.ShowAsContext();
        }
        
        public static void Show(DashVariables p_variables, DashGraph p_graph = null)
        {
            GenericMenu menu = new GenericMenu();

            foreach (Type type in SupportedBasicTypes)
            {
                menu.AddItem(new GUIContent("Basic/" + Variable.ConvertToTypeName(type)), false, () => p_variables.AddNewVariable(type));
            }
            
            foreach (Type type in SupportedUnityTypes)
            {
                menu.AddItem(new GUIContent("Unity/" + Variable.ConvertToTypeName(type)), false, () => p_variables.AddNewVariable(type));
            }

            if (p_graph != null)
            {
                menu.AddSeparator("");
                
                foreach (var variable in p_graph.variables)
                {
                    menu.AddItem(new GUIContent("Graph Overrides/" + variable.Name), false,
                        () => p_variables.AddVariableDirect(variable.Clone()));
                }
                
                menu.AddItem(new GUIContent("All Overrides"), false, () =>
                {
                    foreach (var variable in p_graph.variables)
                    {
                        p_variables.AddVariableDirect(variable.Clone());
                    }
                });
            }

            // foreach (Type type in SupportedExposedTypes)
            // {
            //     menu.AddItem(new GUIContent("Exposed/" + Variable.ConvertToTypeName(type)), false, () => p_callback(type));
            // }

            menu.ShowAsContext();
        }
        #endif
    }
}
#endif