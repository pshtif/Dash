using Dash.Editor;
using UnityEditor;
using UnityEngine;

namespace Dash
{
    [CustomEditor(typeof(DashVariablesController))]
    public class DashVariablesControllerInspector : UnityEditor.Editor
    {
        protected DashVariablesController variablesController => (DashVariablesController) target;

        public override void OnInspectorGUI()
        {
            EditorGUIUtility.labelWidth = 100;

            EditorGUI.BeginChangeCheck();
            variablesController.makeGlobal = EditorGUILayout.Toggle("Make Global", variablesController.makeGlobal);
            bool invalidate = EditorGUI.EndChangeCheck();
            

            invalidate = invalidate || GUIVariableUtils.DrawVariablesInspector("Variables", variablesController.Variables, variablesController, EditorGUIUtility.currentViewWidth-20);

            GUI.color = new Color(1, 0.7f, 0);
            if (GUILayout.Button("Add Variable", GUILayout.Height(24)))
            {
                VariableTypesMenu.Show((type) => variablesController.Variables.AddNewVariable(type));
            }
            GUI.color = Color.white;
            
            if (invalidate)
            {
                PrefabUtility.RecordPrefabInstancePropertyModifications(target);
                EditorUtility.SetDirty(target);
            }
        }
    }
}