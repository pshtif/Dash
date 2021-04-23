using System;
using System.Collections.Generic;
using System.Reflection;
using OdinSerializer.Utilities;
using UnityEditor;
using UnityEngine;

namespace Dash
{
    public class GUIVariableUtils
    {
        public static void VariableField(DashVariables p_variables, string p_name, GameObject p_boundObject)
        {
            var variable = p_variables.GetVariable(p_name);
            EditorGUILayout.BeginHorizontal();
            string newName = EditorGUILayout.TextField(p_name, GUILayout.Width(140));
            EditorGUILayout.Space(4, false);
            if (newName != p_name) 
            {
                p_variables.RenameVariable(p_name, newName);
            }
            
            EditorGUI.BeginChangeCheck();
            variable.PropertyField();

            GUI.color = variable.IsBound ? Color.yellow : Color.gray;

            EditorGUILayout.BeginVertical(GUILayout.Width(16));
            EditorGUILayout.Space(2,false);
            if (GUILayout.Button(IconManager.GetIcon("Bind_Icon"), GUIStyle.none, GUILayout.Height(16), GUILayout.Width(16)))
            {
                GetVariableMenu(p_variables, p_name, p_boundObject).ShowAsContext();
            }
            EditorGUILayout.EndVertical();
            
            GUI.color = Color.white;

            EditorGUILayout.EndHorizontal();
        }

        static GenericMenu GetVariableMenu(DashVariables p_variables, string p_name, GameObject p_boundObject)
        {
            GenericMenu menu = new GenericMenu();

            var variable = p_variables.GetVariable(p_name);
            if (variable.IsBound)
            {
                menu.AddItem(new GUIContent("Unbind"), false, () => variable.UnbindProperty());
            } 
            else
            {
                if (p_boundObject != null)
                {
                    Dictionary<Component, List<PropertyInfo>> bindableFields =
                        GetBindableProperties(p_variables, p_name, p_boundObject);
                    foreach (var infoKeys in bindableFields)
                    {
                        foreach (PropertyInfo property in infoKeys.Value)
                        {
                            menu.AddItem(new GUIContent("Bind (Controller)/" + infoKeys.Key.name + "/" + property.Name),
                                false,
                                () => OnBindVariable(variable, property, infoKeys.Key));
                        }
                    }
                }
            }

            menu.AddItem(new GUIContent("Delete Variable"), false, () => OnDeleteVariable(p_variables, p_name));

            return menu;
        }
        
        static Dictionary<Component, List<PropertyInfo>> GetBindableProperties(DashVariables p_variables, string p_name, GameObject p_boundObject)
        {
            Dictionary<Component, List<PropertyInfo>> bindableFields = new Dictionary<Component, List<PropertyInfo>>();
            var variable = p_variables.GetVariable(p_name);
            if (p_boundObject != null)
            {
                Component[] components = p_boundObject.GetComponents<Component>();
                foreach (Component component in components)
                {
                    Type componentType = component.GetType();
                    PropertyInfo[] properties = componentType.GetProperties();
                    foreach (PropertyInfo property in properties)
                    {
                        if (IsPropertyBindable(variable, property))
                        {
                            if (!bindableFields.ContainsKey(component))
                            {
                                bindableFields.Add(component, new List<PropertyInfo>());
                            }
                            bindableFields[component].Add(property);
                        }
                    }
                }
            }

            return bindableFields;
        }
        
        static bool IsPropertyBindable(Variable p_variable, PropertyInfo p_propertyInfo)
        {
            if (p_variable.GetVariableType().IsAssignableFrom(p_propertyInfo.PropertyType))
                return true;

            return false;
        }

        static void OnBindVariable(Variable p_variable, PropertyInfo p_property, Component p_boundComponent)
        {
            p_variable.BindProperty(p_property, p_boundComponent);
        }
        
        static void OnDeleteVariable(DashVariables p_variables, string p_name)
        {
            p_variables.RemoveVariable(p_name);
        }
    }
}