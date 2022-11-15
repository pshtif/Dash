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

            GUI.color = DashEditorCore.EditorConfig.theme.InspectorButtonColor;
            if (GUILayout.Button("Add Variable", GUILayout.Height(24)))
            {
                VariableTypesMenu.Show(variablesController.Variables, GetGraphOnGameObject());
            }
            GUI.color = Color.white;
            
            if (invalidate)
            {
                PrefabUtility.RecordPrefabInstancePropertyModifications(target);
                EditorUtility.SetDirty(target);
            }
        }

        public DashGraph GetGraphOnGameObject()
        {
            var controller = variablesController.GetComponent<DashController>();
            return controller != null ? controller.Graph : null;
        }
    }
}