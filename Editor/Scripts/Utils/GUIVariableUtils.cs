#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Dash.Editor
{
    public class GUIVariableUtils
    {
        public static bool DrawVariablesInspector(string p_title, DashVariables p_variables, IVariableOwner p_owner, float p_maxWidth, ref bool p_minimized)
        {
            if (!GUIUtils.DrawMinimizableSectionTitle(p_title,
                    ref p_minimized))
                return false;

            return DrawVariablesInspector("", p_variables, p_owner, p_maxWidth);
        }
        
        public static bool DrawVariablesInspector(string p_title, DashVariables p_variables, IVariableOwner p_owner, float p_maxWidth)
        {
            if (p_title != "") GUIUtils.DrawSectionTitle(p_title);

            GUILayout.Space(2);
            int index = 0;
            bool invalidate = false;
            p_variables.variables?.ForEach(variable =>
            {
                invalidate = invalidate || VariableField(p_variables, variable.Name, p_owner, p_maxWidth);
                GUILayout.Space(4);
                index++;
            });

            return invalidate;
        }

        public static bool VariableField(DashVariables p_variables, string p_name, IVariableOwner p_owner, float p_maxWidth)
        {
            bool invalidate = false;
            var variable = p_variables.GetVariable(p_name);
            GUILayout.BeginHorizontal();
            var oldColor = GUI.color;
            GUI.color = ReservedParameters.IsReservedParameter(p_name) ? Color.red : Color.white;
            string newName = GUILayout.TextField(p_name, GUILayout.Width(120));
            newName = newName.RemoveWhitespace();
            GUILayout.Space(2);
            if (newName != p_name)
            {
                invalidate = true;   
                p_variables.RenameVariable(p_name, newName);
            }
            
            invalidate = invalidate || variable.ValueField(p_maxWidth-150, p_owner);
            
            GUI.color = variable.IsBound || variable.IsLookup ? Color.yellow : Color.gray;
            
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical(GUILayout.Width(16));
            GUILayout.Space(2);
            if (GUILayout.Button(IconManager.GetIcon("settings_icon"), GUIStyle.none, GUILayout.Height(16), GUILayout.Width(16)))
            {
                var menu = VariableSettingsMenu.Get(p_variables, p_name, p_owner);
                GenericMenuPopup.Show(menu, "", Event.current.mousePosition, 240, 300, false, false);
            }
            GUILayout.EndVertical();

            GUI.color = oldColor;

            GUILayout.EndHorizontal();

            return invalidate;
        }
    }
}
#endif