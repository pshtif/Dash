using System;
using UnityEditor;
using UnityEngine;

namespace Dash
{
    [CustomEditor(typeof(DashGlobalVariables))]
    public class DashGlobalVariablesInspector : Editor
    {
        protected DashVariables variables => ((DashGlobalVariables) target).variables;

        public override void OnInspectorGUI()
        {
            EditorGUIUtility.labelWidth = 100;

            int index = 0;
            foreach (var variable in variables)
            {
                GUIVariableUtils.VariableField(variables, variable.Name, ((DashGlobalVariables)target).gameObject);
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