/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Dash.Attributes;
using OdinSerializer.Utilities;
using UnityEditor;
using UnityEngine;
using Object = System.Object;

namespace Dash.Editor
{ 
    public class AddTypeContextMenu
    {
        static private Vector2 _lastMousePosition;
        static public void Show()
        {
            _lastMousePosition = Event.current.mousePosition;
            
            Get().ShowAsContext();
        }

        static public void ShowAsPopup()
        {
            _lastMousePosition = Event.current.mousePosition;

            GenericMenuPopup.Show(Get(), "Select Type",  _lastMousePosition, 300, 300, true, false);
        }
        
        static public GenericMenu Get()
        {
            GenericMenu menu = new GenericMenu();

            Type[] loadedTypes = ReflectionUtils.GetAllTypes();
            foreach (Type type in loadedTypes)
            {
                string path =
                    (string.IsNullOrEmpty(type.Namespace) ? "Without Namespace" : type.Namespace.Replace(".", "/")) + "/" +
                    type.Name;
                menu.AddItem(new GUIContent(path, ""), false, AddType, type);
            }

            return menu;
        }

        static void AddType(object p_type)
        {
            if (DashEditorCore.Config.explicitAOTTypes == null)
            {
                DashEditorCore.Config.explicitAOTTypes = new List<Type>();
            }

            if (!DashEditorCore.Config.explicitAOTTypes.Contains(p_type))
            {
                DashEditorCore.Config.explicitAOTTypes.Add((Type)p_type);
            }
            
            EditorUtility.SetDirty(DashEditorCore.Config);
        }
    }
}