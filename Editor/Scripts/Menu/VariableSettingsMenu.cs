/*
 *	Created by:  Peter @sHTiF Stefcek
 */
#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Dash.Editor
{
    public class VariableSettingsMenu
    {
        
        public static RuntimeGenericMenu Get(DashVariables p_variables, string p_name, IVariableOwner p_owner)
        {
            RuntimeGenericMenu menu = new RuntimeGenericMenu();

            var variable = p_variables.GetVariable(p_name);
            if (variable.IsBound)
            {
                menu.AddItem(new GUIContent("Unbind"), false, () => variable.UnbindProperty());
            } 
            else
            {
                if (p_owner.Bindable != null && !variable.IsLookup)
                {
                    Dictionary<Component, List<PropertyInfo>> bindableProperties =
                        GetBindableProperties(p_variables, p_name, p_owner);
                    foreach (var infoKeys in bindableProperties)
                    {
                        foreach (PropertyInfo property in infoKeys.Value)
                        {
                            menu.AddItem(new GUIContent("Bind (Controller)/" + infoKeys.Key.name + "/" + property.Name),
                                false,
                                () => OnBindVariable(variable, property, infoKeys.Key, p_owner));
                        }
                    }
                    
                    Dictionary<Component, List<FieldInfo>> bindableFields =
                        GetBindableFields(p_variables, p_name, p_owner);
                    foreach (var infoKeys in bindableFields)
                    {
                        foreach (FieldInfo field in infoKeys.Value)
                        {
                            menu.AddItem(new GUIContent("Bind (Controller)/" + infoKeys.Key.name + "/" + field.Name),
                                false, 
                                () => OnBindVariable(variable, field, infoKeys.Key, p_owner));
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
            
            menu.AddItem(new GUIContent("Delete Variable"), false, () => OnDeleteVariable(p_variables, p_name, p_owner));
            menu.AddItem(new GUIContent("Copy Variable"), false, () => OnCopyVariable(p_variables, p_name));
            
            menu.AddItem(new GUIContent("Refactor Variable"), false, () => OnRefactorVariable(p_variables, p_name, p_owner));

            return menu;
        }
        
        static Dictionary<Component, List<PropertyInfo>> GetBindableProperties(DashVariables p_variables, string p_name, IVariableOwner p_owner)
        {
            Dictionary<Component, List<PropertyInfo>> bindableProperties = new Dictionary<Component, List<PropertyInfo>>();
            var variable = p_variables.GetVariable(p_name);
            if (p_owner.Bindable != null)
            {
                Component[] components = p_owner.Bindable.gameObject.GetComponents<Component>();
                foreach (Component component in components)
                {
                    // Can happen if script on gameobject component is missing
                    if (component == null)
                        continue;
                    
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
        
        static Dictionary<Component, List<FieldInfo>> GetBindableFields(DashVariables p_variables, string p_name, IVariableOwner p_owner)
        {
            Dictionary<Component, List<FieldInfo>> bindableFields = new Dictionary<Component, List<FieldInfo>>();
            var variable = p_variables.GetVariable(p_name);
            if (p_owner.Bindable != null)
            {
                Component[] components = p_owner.Bindable.gameObject.GetComponents<Component>();
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
        
        static void OnBindVariable(Variable p_variable, PropertyInfo p_property, Component p_boundComponent, IVariableOwner p_owner)
        {
            p_variable.BindProperty(p_property, p_boundComponent, p_owner.Bindable);
        }
        
        static void OnBindVariable(Variable p_variable, FieldInfo p_field, Component p_boundComponent, IVariableOwner p_owner)
        {
            p_variable.BindField(p_field, p_boundComponent, p_owner.Bindable);
        }
        
        static void OnDeleteVariable(DashVariables p_variables, string p_name, IVariableOwner p_owner)
        {
            p_variables.RemoveVariable(p_name);
            
            p_owner.MarkDirty();
            //DashEditorCore.SetDirty();
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
        
        static void OnRefactorVariable(DashVariables p_variables, string p_name, IVariableOwner p_owner)
        {
            RefactorVariableWindow.RefactorVariable(p_variables, p_name);
        }
    }
}
#endif