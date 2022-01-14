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
            if (DashEditorCore.EditorConfig.settingsShowInspectorLogo)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Box(Resources.Load<Texture>("Textures/dash"), GUILayout.ExpandWidth(true),
                    GUILayout.Height(40));
                GUILayout.EndHorizontal();
            }

            EditorGUIUtility.labelWidth = 100;

            if (PrefabUtility.GetPrefabInstanceStatus(target) != PrefabInstanceStatus.NotAPrefab)
            {
                EditorGUILayout.LabelField("Prefab overrides are not supported.");
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                variablesController.makeGlobal = EditorGUILayout.Toggle("Make Global", variablesController.makeGlobal);
                
                GUIVariableUtils.DrawVariablesInspector("Controller Variables", variablesController.Variables,variablesController.gameObject);
                
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(target);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(target);
                }
            }
        }
    }
}