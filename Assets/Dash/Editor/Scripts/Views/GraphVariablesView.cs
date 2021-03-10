/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using Dash.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using Object = System.Object;

namespace Dash
{
    public class GraphVariablesView : ViewBase
    {
        private Vector2 scrollPosition;

        public GraphVariablesView()
        {

        }

        public override void DrawGUI(Event p_event, Rect p_rect)
        {
            if (Graph == null || !Graph.showVariables)
                return;

            Rect rect = new Rect(20, 30, 340, 200);
            
            DrawBoxGUI(rect, "Graph Variables", TextAnchor.UpperCenter);

            GUILayout.BeginArea(new Rect(rect.x+5, rect.y+30, rect.width-10, rect.height-35));
            // scrollPosition = GUI.BeginScrollView(new Rect(rect.x, rect.y + 32, rect.width, rect.height - 42), scrollPosition,
            //     new Rect(0, 30, rect.width, Graph.variables.Count), false, false);
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);
            
            EditorGUI.BeginChangeCheck();

            EditorGUIUtility.labelWidth = 100;

            int index = 0;
            foreach (var variable in Graph.variables)
            {
                //var r = new Rect(0, 25 + 24 * index, rect.width, 30);
                VariableField(variable);
                EditorGUILayout.Space(4);
                index++;
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(Graph);
            }
            
            GUILayout.EndScrollView();
            GUILayout.EndArea();

            if (GUI.Button(new Rect(rect.x + 4, rect.y + rect.height - 24, rect.width - 8, 20), "Add Variable"))
            {
                TypesMenu.Show(OnAddVariable);
            }

            UseEvent(new Rect(rect.x, rect.y, rect.width, rect.height));
        }
        
        public void VariableField(Variable p_variable)
        {
            EditorGUILayout.BeginHorizontal();
            string newName = EditorGUILayout.TextField(p_variable.Name, GUILayout.Width(120));
            EditorGUILayout.Space(8);
            if (newName != p_variable.Name) 
            {
                Graph.variables.RenameVariable(p_variable.Name, newName);
            }
            
            EditorGUI.BeginChangeCheck();
            p_variable.PropertyField();

            GUI.color = p_variable.IsBound ? Color.yellow : Color.gray;
            
            if (GUILayout.Button(IconManager.GetIcon("Bind_Icon"), GUIStyle.none, GUILayout.Height(16), GUILayout.MaxWidth(16)))
            {
                GetVariableMenu(p_variable).ShowAsContext();
            }
            
            GUI.color = Color.white;

            EditorGUILayout.EndHorizontal();
        }

        GenericMenu GetVariableMenu(Variable p_variable)
        {
            GenericMenu menu = new GenericMenu();

            if (p_variable.IsBound)
            {
                menu.AddItem(new GUIContent("Unbind"), false, () => p_variable.UnbindProperty());
            } 
            else
            {
                Dictionary<Component, List<PropertyInfo>> bindableFields = GetBindableProperties(p_variable);
                foreach (var infoKeys in bindableFields)
                {
                    foreach (PropertyInfo property in infoKeys.Value)
                    {
                        //PropertyInfo prop = property;
                        menu.AddItem(new GUIContent("Bind (Controller)/" + infoKeys.Key + "/" + property.Name), false,
                            () => OnBindVariable(p_variable, property, infoKeys.Key));
                    }
                }
            }

            menu.AddItem(new GUIContent("Delete Variable"), false, () => OnDeleteVariable(p_variable));

            return menu;
        }

        Dictionary<Component, List<PropertyInfo>> GetBindableProperties(Variable p_variable)
        {
            Dictionary<Component, List<PropertyInfo>> bindableFields = new Dictionary<Component, List<PropertyInfo>>();
            
            if (Graph.Controller != null)
            {
                Component[] components = Graph.Controller.gameObject.GetComponents<Component>();
                foreach (Component component in components)
                {
                    Type componentType = component.GetType();
                    PropertyInfo[] properties = componentType.GetProperties();
                    foreach (PropertyInfo property in properties)
                    {
                        if (IsPropertyBindable(p_variable, property))
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
        
        bool IsPropertyBindable(Variable p_variable, PropertyInfo p_propertyInfo)
        {
            if (p_variable.GetVariableType().IsAssignableFrom(p_propertyInfo.PropertyType))
                return true;

            return false;
        }

        void OnBindVariable(Variable p_variable, PropertyInfo p_property, Component p_boundComponent)
        {
            p_variable.BindProperty(p_property, p_boundComponent);
        }
        
        void OnDeleteVariable(Variable p_variable)
        {
            Graph.variables.RemoveVariable(p_variable.Name);
        }

        void OnAddVariable(Type p_type)
        {
            string name = "new"+p_type.ToString().Substring(p_type.ToString().LastIndexOf(".")+1);

            int index = 0;
            while (Graph.variables.HasVariable(name + index)) index++;
            
            Graph.variables.AddVariableByType((Type)p_type, name+index, null);
        }
    }
}