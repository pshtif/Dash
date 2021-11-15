/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Dash
{
    public class TypesMenu
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
            typeof(Canvas)
        };
            
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
        #endif
    }
}