using System;
using OdinSerializer.Utilities;
using UnityEditor;
using UnityEditor.SceneManagement;
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

            if (PrefabUtility.GetPrefabInstanceStatus(target) != PrefabInstanceStatus.NotAPrefab)
            {
                EditorGUILayout.LabelField("Prefab overrides are not supported.");
            }
            else
            {
                EditorGUI.BeginChangeCheck();

                GUIVariableUtils.DrawVariablesInspector(variables, ((DashGlobalVariables) target).gameObject);

                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(target);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(target);
                }
            }
        }
    }
}