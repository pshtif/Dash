using System;
using UnityEditor;
using UnityEngine;

namespace Dash.Editor
{
    [CustomEditor(typeof(DashGlobalVariables))]
    public class DashGlobalVariablesInspector : UnityEditor.Editor
    {
        protected DashVariables variables => ((DashGlobalVariables) target).variables;

        public override void OnInspectorGUI()
        {
            EditorGUIUtility.labelWidth = 100;

            int index = 0;
            foreach (var variable in variables)
            {
                GUIVariableUtils.VariableField(variables, variable.Name, ((DashGlobalVariables)target).gameObject, EditorGUIUtility.currentViewWidth-20);
                EditorGUILayout.Space(4);
                index++;
            }

            if (GUILayout.Button("Add Variable"))
            {
                TypesMenu.Show(OnAddNewVariable);
            }
        }

        void OnAddNewVariable(Type p_type)
        {
            variables.AddNewVariable(p_type);
        }
    }
}