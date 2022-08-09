using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace Dash.Editor
{
    public class GUIVariableUtils
    {
        public static void DrawVariablesInspector(string p_title, DashVariables p_variables, IVariableBindable p_bindable)
        {
            var style = new GUIStyle();
            style.normal.textColor = new Color(1, 0.7f, 0);
            style.alignment = TextAnchor.MiddleCenter;
            style.fontStyle = FontStyle.Bold;
            style.normal.background = Texture2D.whiteTexture;
            style.fontSize = 16;
            GUI.backgroundColor = new Color(0, 0, 0, .5f);
            GUILayout.Label(p_title, style, GUILayout.Height(28));
            GUI.backgroundColor = Color.white;

            int index = 0;
            p_variables.variables?.ForEach(variable =>
            {
                VariableField(p_variables, variable.Name, p_bindable,
                    EditorGUIUtility.currentViewWidth - 20);
                GUILayout.Space(4);
                index++;
            });

            if (GUILayout.Button("Add Variable"))
            {
                VariableTypesMenu.Show((type) => OnAddNewVariable(p_variables, type));
            }
        }
        
        static void OnAddNewVariable(DashVariables p_variables, Type p_type)
        {
            p_variables.AddNewVariable(p_type);
            // EditorUtility.SetDirty(target);
            // PrefabUtility.RecordPrefabInstancePropertyModifications(target);
        }
        
        public static void VariableField(DashVariables p_variables, string p_name, IVariableBindable p_bindable, float p_maxWidth)
        {
            var variable = p_variables.GetVariable(p_name);
            GUILayout.BeginHorizontal();
            string newName = EditorGUILayout.TextField(p_name, GUILayout.Width(120));
            GUILayout.Space(2);
            if (newName != p_name) 
            {
                p_variables.RenameVariable(p_name, newName);
            }
            
            variable.ValueField(p_maxWidth-150, p_bindable);

            var oldColor = GUI.color;
            GUI.color = variable.IsBound || variable.IsLookup ? Color.yellow : Color.gray;
            
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical(GUILayout.Width(16));
            GUILayout.Space(2);
            if (GUILayout.Button(IconManager.GetIcon("bind_icon"), GUIStyle.none, GUILayout.Height(16), GUILayout.Width(16)))
            {
                var menu = VariableSettingsMenu.Get(p_variables, p_name, p_bindable);
                GenericMenuPopup.Show(menu, "", Event.current.mousePosition, 240, 300, false, false);
            }
            GUILayout.EndVertical();

            GUI.color = oldColor;

            GUILayout.EndHorizontal();
        }
    }
}