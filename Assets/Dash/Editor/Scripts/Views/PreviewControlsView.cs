/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEngine;

namespace Dash
{
    public class PreviewControlsView : ViewBase
    {

        public override void UpdateGUI(Event p_event, Rect p_rect)
        {
            if (Application.isPlaying || Graph == null)
                return;

            if (Graph.previewControlsViewMinimized)
            {
                Rect rect = new Rect(p_rect.width / 2 + 170, p_rect.height - 28, 32, 70);
                
                DrawBoxGUI(rect, "", TextAnchor.UpperLeft);

                if (GUI.Button(new Rect(rect.x + rect.width - 28, rect.y + 4, 24, 24),
                    IconManager.GetIcon("RollIn_Icon"),
                    GUIStyle.none))
                {
                    Graph.previewControlsViewMinimized = false;
                }
            }
            else
            {
                Rect rect = new Rect(p_rect.width / 2 - 200, p_rect.height - 80, 400, 70);

                DrawBoxGUI(rect, "Preview Controls", TextAnchor.UpperLeft);
                
                GUI.color = new Color(0.75f, .5f, 0);
                GUI.Label(new Rect(rect.x + 130, rect.y - 2, 100, 32), "[Experimental]");
                GUI.color = Color.white;
                
                if (GUI.Button(new Rect(rect.x + rect.width - 28, rect.y + 4, 24, 24),
                    IconManager.GetIcon("RollOut_Icon"),
                    GUIStyle.none))
                {
                    Graph.previewControlsViewMinimized = true;
                }

                bool hasActiveController = DashEditorCore.Config.editingGraph.Controller != null &&
                                      DashEditorCore.Config.editingGraph.Controller.gameObject.activeInHierarchy;

                bool _previewRunning = DashEditorCore.Previewer.IsPreviewing;
                GUI.enabled = !_previewRunning && hasActiveController;
                if (GUI.Button(new Rect(rect.x + 4, rect.y + 32, 100, 30), "RUN"))
                {
                    DashEditorCore.Previewer.StartPreview(Graph);
                }

                GUI.enabled = _previewRunning;
                if (GUI.Button(new Rect(rect.x + 108, rect.y + 32, 100, 30), "STOP"))
                {
                    DashEditorCore.Previewer.StopPreview();
                }

                GUI.Label(new Rect(rect.x + 220, rect.y + 32, 180, 30),
                    hasActiveController ? "Previewing on: " + DashEditorCore.Config.editingGraph.Controller.name : "No controller.");

                GUI.enabled = true;
            }
        }
    }
}