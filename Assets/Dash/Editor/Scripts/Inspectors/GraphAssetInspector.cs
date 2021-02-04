/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEditor;
using UnityEngine;

namespace Dash
{
    [CustomEditor(typeof(DashGraph))]
    public class GraphAssetInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open Editor"))
            {
                DashEditorWindow.InitEditorWindow(null);
                DashEditorCore.EditGraph((DashGraph)target);
            }
        }
    }
}