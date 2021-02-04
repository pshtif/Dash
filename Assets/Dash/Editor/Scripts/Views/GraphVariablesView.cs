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
        public int height => Graph.variablesViewMinimized ? 40 : 200;

        private Vector2 scrollPosition;

        public GraphVariablesView()
        {

        }

        public override void UpdateGUI(Event p_event, Rect p_rect)
        {
            if (Graph == null || !Graph.showVariables)
                return;

            Rect rect = new Rect(20, 30, 340, height);
            
            DrawBoxGUI(rect, "Graph Variables", TextAnchor.UpperCenter);
            
            if (Graph.variablesViewMinimized)
            {
                if (GUI.Button(new Rect(rect.x + rect.width - 28, rect.y + 4, 24, 24),
                    IconManager.GetIcon("RollOut_Icon"),
                    GUIStyle.none))
                {
                    Graph.variablesViewMinimized = false;
                }
            }
            else
            {
                if (GUI.Button(new Rect(rect.x + rect.width - 28, rect.y + 4, 24, 24),
                    IconManager.GetIcon("RollIn_Icon"),
                    GUIStyle.none))
                {
                    Graph.variablesViewMinimized = true;
                }

                // Should never happen now, can remove later
                if (Graph.variables != null)
                {
                    scrollPosition = GUI.BeginScrollView(new Rect(rect.x, rect.y + 32, rect.width, rect.height - 42), scrollPosition,
                        new Rect(0, 30, rect.width, Graph.variables.Count), false, false);
                    
                    EditorGUI.BeginChangeCheck();

                    EditorGUIUtility.labelWidth = 100;

                    int index = 0;
                    foreach (var variable in Graph.variables)
                    {
                        var r = new Rect(0, 25 + 24 * index, rect.width, 30);
                        VariableField(r, variable);
                        index++;
                    }

                    if (EditorGUI.EndChangeCheck())
                    {
                        EditorUtility.SetDirty(Graph);
                    }
                    
                    GUI.EndScrollView();
                }

                if (GUI.Button(new Rect(rect.x + 4, rect.y + rect.height - 24, rect.width - 8, 20), "Add Variable"))
                {
                    VariablesMenu();
                }
            }

            if (new Rect(0, 0, rect.width, rect.height).Contains(p_event.mousePosition) &&
                p_event.type != EventType.Layout && p_event.type != EventType.Repaint)
            {
                p_event.type = EventType.Used;
            }
        }
        
        public void VariableField(Rect p_position, Variable p_variable)
        {
            string newName = EditorGUI.TextField(new Rect(p_position.x+5, p_position.y+5, 100, p_position.height-10), p_variable.Name);
            if (newName != p_variable.Name) 
            {
                Graph.variables.RenameVariable(p_variable.Name, newName);
            }

            var rect = new Rect(p_position.x + 110, p_position.y + 5, p_position.width - 135, p_position.height - 10);

            EditorGUI.BeginChangeCheck();
            p_variable.PropertyField(rect);

            GUI.color = p_variable.IsBound ? Color.yellow : Color.gray;
            
            if (GUI.Button(new Rect(p_position.x + p_position.width - 20, p_position.y + 6, 16, 16),
                IconManager.GetIcon("Bind_Icon"), GUIStyle.none))
            {
                GetVariableMenu(p_variable).ShowAsContext();
            }
            
            GUI.color = Color.white;

            if (EditorGUI.EndChangeCheck())
            {
                
            }
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
            if (p_variable.value.GetType().IsAssignableFrom(p_propertyInfo.PropertyType))
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

        void VariablesMenu()
        {
            GenericMenu menu = new GenericMenu();
            
            foreach (Type type in Variable.SupportedTypes)
            {
                menu.AddItem(new GUIContent(Variable.ConvertToTypeName(type)), false, OnAddVariable, type);
            }

            menu.ShowAsContext();            
        }

        void OnAddVariable(object p_type)
        {
            string name = "new"+p_type.ToString().Substring(p_type.ToString().LastIndexOf(".")+1);

            int index = 0;
            while (Graph.variables.HasVariable(name + index)) index++;
            
            Graph.variables.AddVariableByType((Type)p_type, name+index, null);
        }
    }
}