/*
 *	Created by:  Peter @sHTiF Stefcek
 */
#if UNITY_EDITOR

using System;
using UnityEngine;

namespace Dash.Editor
{
    public class VariablesView : ViewBase
    {
        private Vector2 scrollPosition;

        public override void DrawGUI(Event p_event, Rect p_rect)
        {
            
        }
        
        protected void DrawVariablesGUI(Vector2 p_position, Color p_color, DashVariables p_variables, ref bool p_minimized, IVariableOwner p_owner)
        {
            int height = p_variables.Count <= 10 ? 64 + p_variables.Count * 22 : 64 + 220; 
            Rect rect = new Rect(p_position.x, p_position.y, 380, p_minimized ? 32 : height);
            DrawBoxGUI(rect, "Graph Variables", TextAnchor.MiddleCenter, p_color, new Color(0.8f,.6f,.4f), Color.white);

            var minStyle = new GUIStyle();
            minStyle.normal.textColor = Color.white;
            minStyle.fontStyle = FontStyle.Bold;
            minStyle.fontSize = 20;
            GUI.color = new Color(.4f, .4f, .4f);
            GUI.Label(new Rect(rect.x + 6 + (p_minimized ? 0 : 2), rect.y + 2, 20, 20), p_minimized ? "+" : "-", minStyle);
            GUI.color = Color.white;
            
            if (GUI.Button(new Rect(p_position.x, p_position.y, 380, 20), "", GUIStyle.none))
            {
                p_minimized = !p_minimized;
                GUI.FocusControl("");
            }

            if (p_minimized)
                return;

            GUILayout.BeginArea(new Rect(rect.x+5, rect.y+30, rect.width-10, rect.height-62));
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);
            
            GUIVariableUtils.DrawVariablesInspector("", p_variables, p_owner, rect.width-20);

            GUILayout.EndScrollView();
            GUILayout.EndArea();

            GUI.color = DashEditorCore.EditorConfig.theme.InspectorButtonColor;
            if (GUI.Button(new Rect(rect.x + 4, rect.y + rect.height - 30, rect.width - 8, 24), "Add Variable"))
            {
                VariableTypesMenu.Show((type) => OnAddVariable(p_variables, type));
            }
            GUI.color = Color.white;
            
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
            
            Graph.MarkDirty();
            //DashEditorCore.SetDirty();
        }
    }
}
#endif