/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEditor;
using UnityEngine;

namespace Dash.Editor
{
    [CustomEditor(typeof(DashGraph))]
    public class DashGraphAssetInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            GUI.color = DashEditorCore.EditorConfig.theme.InspectorButtonColor;
            if (GUILayout.Button("Open Editor", GUILayout.Height(40)))
            {
                DashEditorWindow.InitEditorWindow((DashGraph)target);
            }
        }
    }
}