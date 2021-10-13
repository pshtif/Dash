/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEngine;

namespace Dash.Editor
{
    public class CheckVersionPopup
    {
        public static bool IsCurrentVersion()
        {
            if (DashEditorCore.Graph != null &&
                DashEditorCore.Graph.version < DashRuntimeCore.GetVersionNumber())
            {
                return false;
            }

            return true;
        }
        
        public static void ShowVersionMigrate(Rect p_rect)
        {
            if (DashEditorCore.Graph.Nodes.Count == 0)
            {
                DashEditorCore.Graph.ValidateSerialization();
            }
            else
            {
                GUIStyle style = DashEditorCore.Skin.GetStyle("ViewBase");
                var rect = new Rect(p_rect.width / 2 - 160, p_rect.height / 2 - 70, 320, 160);
                GUI.Box(rect, new GUIContent("Version Warning"), style);
                GUILayout.BeginArea(new Rect(rect.x + 6, rect.y + 34, rect.width - 12, rect.height - 36));
                GUILayout.BeginVertical();
                GUIStyle textStyle = new GUIStyle();
                textStyle.normal.textColor = Color.white;
                textStyle.wordWrap = true;
                textStyle.alignment = TextAnchor.UpperCenter;
                GUILayout.TextArea("This graph was created by previous version of Dash Animation System version: " +
                                   DashRuntimeCore.GetVersionString(DashEditorCore.EditorConfig.editingGraph.version) + "\n" +
                                   "The current version is " +
                                   DashRuntimeCore.GetVersionString(DashRuntimeCore.GetVersionNumber()) +
                                   " so it is needed to migrate and revalidate serialization or the Graph may not work correctly.\n" +
                                   "Make sure you have backup of this Graph.", textStyle);
                GUILayout.Space(4);
                if (GUILayout.Button("Migrate"))
                {
                    DashEditorCore.Graph.ValidateSerialization();
                }

                GUILayout.EndVertical();
                GUILayout.EndArea();
            }
        }
    }
}