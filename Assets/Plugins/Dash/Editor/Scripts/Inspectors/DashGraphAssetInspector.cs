/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEditor;
using UnityEngine;

namespace Dash
{
    [CustomEditor(typeof(DashGraph))]
    public class DashGraphAssetInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            GUI.color = new Color(1, 0.75f, 0.5f);
            if (GUILayout.Button("Open Editor", GUILayout.Height(40)))
            {
                DashEditorWindow.InitEditorWindow(null);
                DashEditorCore.EditGraph((DashGraph)target);
            }
        }
    }
}