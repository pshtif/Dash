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

                int index = 0;
                variables?.ForEach(variable =>
                {
                    GUIVariableUtils.VariableField(variables, variable.Name, ((DashGlobalVariables) target).gameObject,
                        EditorGUIUtility.currentViewWidth - 20);
                    EditorGUILayout.Space(4);
                    index++;
                });

                if (GUILayout.Button("Add Variable"))
                {
                    TypesMenu.Show(OnAddNewVariable);
                }

                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(target);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(target);
                }
            }
        }
        
        void OnAddNewVariable(Type p_type)
        {
            variables.AddNewVariable(p_type);
            EditorUtility.SetDirty(target);
            PrefabUtility.RecordPrefabInstancePropertyModifications(target);
        }
    }
}