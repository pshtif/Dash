/*
 *	Created by:  Peter @sHTiF Stefcek
 */
#if UNITY_EDITOR

using UnityEngine;

namespace Dash.Editor
{
    public class PreviewControlsView : ViewBase
    {
        public override void DrawGUI(Event p_event, Rect p_rect)
        {
            if (Application.isPlaying || Graph == null || Controller == null)
                return;

            if (Graph.previewControlsViewMinimized)
            {
                Rect rect = new Rect(p_rect.width / 2 + 170, p_rect.height - 28, 32, 70);
                
                DrawBoxGUI(rect, "", TextAnchor.MiddleLeft, Color.white, Color.white, Color.white);

                if (GUI.Button(new Rect(rect.x + rect.width - 28, rect.y + 4, 24, 24),
                    IconManager.GetIcon("rollin_icon"),
                    GUIStyle.none))
                {
                    Graph.previewControlsViewMinimized = false;
                }
            }
            else
            {
                Rect rect = new Rect(p_rect.width / 2 - 220, p_rect.height - 80, 440, 70);

                DrawBoxGUI(rect, "  Preview Controls", TextAnchor.MiddleLeft, Color.white, Color.white, Color.white);
                
                GUI.color = Color.yellow;
                GUI.DrawTexture(new Rect(rect.x + 132, rect.y + 7, 16, 16), IconManager.GetIcon("experimental_icon"));
                GUI.color = Color.white;

                if (GUI.Button(new Rect(rect.x + rect.width - 28, rect.y + 4, 24, 24),
                    IconManager.GetIcon("rollout_icon"),
                    GUIStyle.none))
                {
                    Graph.previewControlsViewMinimized = true;
                }
                
                bool _previewRunning = DashEditorCore.Previewer.IsPreviewing;

                var previewNode = Graph.previewNode;
                if (previewNode != null)
                {
                    GUIStyle style = new GUIStyle();
                    style.normal.textColor = Color.white;
                    GUI.Label(new Rect(rect.x + 214, rect.y + 40, 180, 30), "Preview: ", style);
                    style.normal.textColor = Color.magenta;
                    style.fontStyle = FontStyle.Bold;
                    GUI.Label(new Rect(rect.x + 270, rect.y + 40, 180, 30),previewNode.Name, style);
                }
                else
                {
                    GUI.Label(new Rect(rect.x + 220, rect.y + 32, 180, 30), "No preview node.");
                }

                GUI.enabled = !_previewRunning && Graph.previewNode != null;
                if (GUI.Button(new Rect(rect.x + 4, rect.y + 32, 100, 30), "RUN"))
                {
                    DashEditorCore.Previewer.StartPreview(Graph.previewNode, Owner.GetConfig().editingController);
                }
                
                GUI.enabled = _previewRunning;
                if (GUI.Button(new Rect(rect.x + 108, rect.y + 32, 100, 30), "STOP"))
                {
                    DashEditorCore.Previewer.StopPreview();
                }

                GUI.enabled = true;
            }
        }
    }
}
#endif