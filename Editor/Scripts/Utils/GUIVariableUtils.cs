using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace Dash.Editor
{
    public class GUIVariableUtils
    {
        public static void DrawVariablesInspector(DashVariables p_variables, GameObject p_boundObject)
        {
            var style = new GUIStyle();
            style.normal.textColor = new Color(1, 0.7f, 0);
            style.alignment = TextAnchor.MiddleCenter;
            style.fontStyle = FontStyle.Bold;
            style.normal.background = Texture2D.whiteTexture;
            style.fontSize = 16;
            GUI.backgroundColor = new Color(0, 0, 0, .5f);
            GUILayout.Label("Variables", style, GUILayout.Height(28));
            GUI.backgroundColor = Color.white;

            int index = 0;
            p_variables.variables?.ForEach(variable =>
            {
                VariableField(p_variables, variable.Name, p_boundObject,
                    EditorGUIUtility.currentViewWidth - 20);
                GUILayout.Space(4);
                index++;
            });

            if (GUILayout.Button("Add Variable"))
            {
                TypesMenu.Show((type) => OnAddNewVariable(p_variables, type));
            }
        }
        
        static void OnAddNewVariable(DashVariables p_variables, Type p_type)
        {
            p_variables.AddNewVariable(p_type);
            // EditorUtility.SetDirty(target);
            // PrefabUtility.RecordPrefabInstancePropertyModifications(target);
        }
        
        public static void VariableField(DashVariables p_variables, string p_name, GameObject p_boundObject, float p_maxWidth)
        {
            var variable = p_variables.GetVariable(p_name);
            GUILayout.BeginHorizontal();
            string newName = EditorGUILayout.TextField(p_name, GUILayout.Width(120));
            GUILayout.Space(2);
            if (newName != p_name) 
            {
                p_variables.RenameVariable(p_name, newName);
            }
            
            variable.ValueField(p_maxWidth-170);

            var oldColor = GUI.color;
            GUI.color = variable.IsBound || variable.IsLookup ? Color.yellow : Color.gray;
            
            GUILayout.BeginVertical(GUILayout.Width(16));
            GUILayout.Space(2);
            if (GUILayout.Button(IconManager.GetIcon("Bind_Icon"), GUIStyle.none, GUILayout.Height(16), GUILayout.Width(16)))
            {
                var menu = GetVariableMenu(p_variables, p_name, p_boundObject);
                GenericMenuPopup.Show(menu, "", Event.current.mousePosition, 240, 300, false, false);
            }
            GUILayout.EndVertical();

            GUI.color = oldColor;

            GUILayout.EndHorizontal();
        }

        static RuntimeGenericMenu GetVariableMenu(DashVariables p_variables, string p_name, GameObject p_boundObject)
        {
            RuntimeGenericMenu menu = new RuntimeGenericMenu();

            var variable = p_variables.GetVariable(p_name);
            if (variable.IsBound)
            {
                menu.AddItem(new GUIContent("Unbind"), false, () => variable.UnbindProperty());
            } 
            else
            {
                if (p_boundObject != null && !variable.IsLookup)
                {
                    Dictionary<Component, List<PropertyInfo>> bindableProperties =
                        GetBindableProperties(p_variables, p_name, p_boundObject);
                    foreach (var infoKeys in bindableProperties)
                    {
                        foreach (PropertyInfo property in infoKeys.Value)
                        {
                            menu.AddItem(new GUIContent("Bind (Controller)/" + infoKeys.Key.name + "/" + property.Name),
                                false,
                                () => OnBindVariable(variable, property, infoKeys.Key));
                        }
                    }
                    
                    Dictionary<Component, List<FieldInfo>> bindableFields =
                        GetBindableFields(p_variables, p_name, p_boundObject);
                    foreach (var infoKeys in bindableFields)
                    {
                        foreach (FieldInfo field in infoKeys.Value)
                        {
                            menu.AddItem(new GUIContent("Bind (Controller)/" + infoKeys.Key.name + "/" + field.Name),
                                false, 
                                () => OnBindVariable(variable, field, infoKeys.Key));
                        }
                    }
                }

                if (variable.IsLookup)
                {
                    menu.AddItem(new GUIContent("Unset as Lookup"), false, () => OnLookupVariable(p_variables, p_name));
                }
                else
                {
                    menu.AddItem(new GUIContent("Set as Lookup"), false, () => OnLookupVariable(p_variables, p_name));
                }
            }
            
            menu.AddItem(new GUIContent("Delete Variable"), false, () => OnDeleteVariable(p_variables, p_name, p_boundObject));
            menu.AddItem(new GUIContent("Copy Variable"), false, () => OnCopyVariable(p_variables, p_name));

            return menu;
        }
        
        static Dictionary<Component, List<PropertyInfo>> GetBindableProperties(DashVariables p_variables, string p_name, GameObject p_boundObject)
        {
            Dictionary<Component, List<PropertyInfo>> bindableProperties = new Dictionary<Component, List<PropertyInfo>>();
            var variable = p_variables.GetVariable(p_name);
            if (p_boundObject != null)
            {
                Component[] components = p_boundObject.GetComponents<Component>();
                foreach (Component component in components)
                {
                    Type componentType = component.GetType();
                    PropertyInfo[] properties = componentType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                    foreach (PropertyInfo property in properties)
                    {
                        if (IsPropertyBindable(variable, property))
                        {
                            if (!bindableProperties.ContainsKey(component))
                            {
                                bindableProperties.Add(component, new List<PropertyInfo>());
                            }
                            bindableProperties[component].Add(property);
                        }
                    }
                }
            }

            return bindableProperties;
        }
        
        static bool IsPropertyBindable(Variable p_variable, PropertyInfo p_propertyInfo)
        {
            if (p_variable.GetVariableType().IsAssignableFrom(p_propertyInfo.PropertyType))
                return true;

            return false;
        }

        static Dictionary<Component, List<FieldInfo>> GetBindableFields(DashVariables p_variables, string p_name, GameObject p_boundObject)
        {
            Dictionary<Component, List<FieldInfo>> bindableFields = new Dictionary<Component, List<FieldInfo>>();
            var variable = p_variables.GetVariable(p_name);
            if (p_boundObject != null)
            {
                Component[] components = p_boundObject.GetComponents<Component>();
                foreach (Component component in components)
                {
                    Type componentType = component.GetType();
                    FieldInfo[] fields = componentType.GetFields(BindingFlags.Instance | BindingFlags.Public);
                    foreach (FieldInfo field in fields)
                    {
                        if (IsFieldBindable(variable, field))
                        {
                            if (!bindableFields.ContainsKey(component))
                            {
                                bindableFields.Add(component, new List<FieldInfo>());
                            }
                            bindableFields[component].Add(field);
                        }
                    }
                }
            }

            return bindableFields;
        }
        
        static bool IsFieldBindable(Variable p_variable, FieldInfo p_fieldInfo)
        {
            if (p_variable.GetVariableType().IsAssignableFrom(p_fieldInfo.FieldType))
                return true;

            return false;
        }
        
        static void OnBindVariable(Variable p_variable, PropertyInfo p_property, Component p_boundComponent)
        {
            p_variable.BindProperty(p_property, p_boundComponent);
        }
        
        static void OnBindVariable(Variable p_variable, FieldInfo p_field, Component p_boundComponent)
        {
            p_variable.BindField(p_field, p_boundComponent);
        }
        
        static void OnDeleteVariable(DashVariables p_variables, string p_name, GameObject p_boundObject)
        {
            p_variables.RemoveVariable(p_name);
            EditorUtility.SetDirty(p_boundObject);
        }
        
        static void OnLookupVariable(DashVariables p_variables, string p_name)
        {
            var variable = p_variables.GetVariable(p_name);
            variable.SetAsLookup(!variable.IsLookup);
        }

        static void OnCopyVariable(DashVariables p_variables, string p_name)
        {
            VariableUtils.CopyVariable(p_variables.GetVariable(p_name));
        }
    }
}