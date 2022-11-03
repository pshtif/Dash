using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace Dash.Editor
{
    public class GUIVariableUtils
    {
        public static bool DrawVariablesInspector(string p_title, DashVariables p_variables, IVariableBindable p_bindable, ref bool p_minimized)
        {
            if (!GUIEditorUtils.DrawMinimizableSectionTitle(p_title,
                    ref p_minimized))
                return false;

            return DrawVariablesInspector("", p_variables, p_bindable);
        }
        
        public static bool DrawVariablesInspector(string p_title, DashVariables p_variables, IVariableBindable p_bindable)
        {
            if (p_title != "") GUIEditorUtils.DrawSectionTitle(p_title);

            int index = 0;
            bool invalidate = false;
            p_variables.variables?.ForEach(variable =>
            {
                invalidate = invalidate || VariableField(p_variables, variable.Name, p_bindable,
                    EditorGUIUtility.currentViewWidth - 20);
                GUILayout.Space(4);
                index++;
            });

            if (GUILayout.Button("Add Variable"))
            {
                VariableTypesMenu.Show((type) => OnAddNewVariable(p_variables, type));
            }

            return invalidate;
        }
        
        static void OnAddNewVariable(DashVariables p_variables, Type p_type)
        {
            p_variables.AddNewVariable(p_type);
            // EditorUtility.SetDirty(target);
            // PrefabUtility.RecordPrefabInstancePropertyModifications(target);
        }
        
        public static bool VariableField(DashVariables p_variables, string p_name, IVariableBindable p_bindable, float p_maxWidth)
        {
            bool invalidate = false;
            var variable = p_variables.GetVariable(p_name);
            GUILayout.BeginHorizontal();
            string newName = EditorGUILayout.TextField(p_name, GUILayout.Width(120));
            GUILayout.Space(2);
            if (newName != p_name)
            {
                invalidate = true;   
                p_variables.RenameVariable(p_name, newName);
            }
            
            invalidate = invalidate || variable.ValueField(p_maxWidth-150, p_bindable);

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

            return invalidate;
        }
    }
}