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

namespace Dash.Editor
{
    public class VariablesView : ViewBase
    {
        private Vector2 scrollPosition;

        public override void DrawGUI(Event p_event, Rect p_rect)
        {
            
        }
        
        protected void DrawVariablesGUI(Vector2 p_position, Color p_color, DashVariables p_variables, ref bool p_minimized, IVariableBindable p_bindable)
        {
            int height = p_variables.Count <= 10 ? 64 + p_variables.Count * 22 : 64 + 220; 
            Rect rect = new Rect(p_position.x, p_position.y, 380, p_minimized ? 32 : height);
            DrawBoxGUI(rect, "Graph Variables", TextAnchor.UpperCenter, p_color);

            var minStyle = new GUIStyle();
            minStyle.normal.textColor = Color.white;
            minStyle.fontStyle = FontStyle.Bold;
            minStyle.fontSize = 20;
            GUI.Label(new Rect(rect.x + rect.width - 20 + (p_minimized ? 0 : 2), rect.y + 2, 20, 20), p_minimized ? "+" : "-", minStyle);
            
            if (GUI.Button(new Rect(p_position.x, p_position.y, 380, 20), "", GUIStyle.none))
            {
                p_minimized = !p_minimized;
                GUI.FocusControl("");
            }

            if (p_minimized)
                return;

            GUILayout.BeginArea(new Rect(rect.x+5, rect.y+30, rect.width-10, rect.height-64));
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);
            
            GUIVariableUtils.DrawVariablesInspector("", p_variables, p_bindable, rect.width-20);

            GUILayout.EndScrollView();
            GUILayout.EndArea();
            
            GUI.color = new Color(1, 0.75f, 0.5f);
            if (GUI.Button(new Rect(rect.x + 4, rect.y + rect.height - 30, rect.width - 8, 24), "Add Variable"))
            {
                VariableTypesMenu.Show((type) => OnAddVariable(p_variables, type));
            }
            GUI.color = Color.white;
            
            //
            // EditorGUI.BeginChangeCheck();
            //
            // if (p_variables != null)
            // {
            //     int index = 0;
            //     foreach (var variable in p_variables)
            //     {
            //         GUIVariableUtils.VariableField(p_variables, variable.Name, p_bindable, rect.width - 10);
            //         EditorGUILayout.Space(4);
            //         index++;
            //     }
            // }
            //
            // GUILayout.EndScrollView();
            // GUILayout.EndArea();
            //
            // if (GUI.Button(new Rect(rect.x + 4, rect.y + rect.height - 48, rect.width - 8, 20), "Add Variable"))
            // {
            //     VariableTypesMenu.Show((type) => OnAddVariable(p_variables, type));
            // }
            //
            // // if (GUI.Button(new Rect(rect.x + 4, rect.y + rect.height - 24, rect.width/2-6, 20), "Copy Variables"))
            // // {
            // //     VariableUtils.CopyVariables(p_variables);
            // // }
            // //
            // // if (GUI.Button(new Rect(rect.x + rect.width/2 + 2, rect.y + rect.height - 24, rect.width/2-6, 20), "Paste Variables"))
            // // {
            // //     VariableUtils.PasteVariables(p_variables, p_bindable);
            // // }
            //
            // if (EditorGUI.EndChangeCheck())
            // {
            //     DashEditorCore.SetDirty();
            // }

            UseEvent(new Rect(rect.x, rect.y, rect.width, rect.height));
        }

        void OnAddVariable(DashVariables p_variables, Type p_type)
        {
            p_variables.AddNewVariable(p_type);
            
            DashEditorCore.SetDirty();
        }
    }
}