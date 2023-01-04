/*
 *	Created by:  Peter @sHTiF Stefcek
 */
#if UNITY_EDITOR

using UnityEngine;

namespace Dash.Editor
{
    public class GraphPropertiesView : ViewBase
    {
        private Vector2 scrollPosition;

        public GraphPropertiesView()
        {

        }

        public override void DrawGUI(Event p_event, Rect p_rect)
        {
            return;
            
            // if (Graph == null || SelectionManager.GetSelectedNode(Graph) != null || Graph.isBound)
            //     return;
            //
            // DrawProperties(p_rect);
        }

        private void DrawProperties(Rect p_rect)
        {
            Rect rect = new Rect(p_rect.width - 400, 30, 390, 140);
            
            DrawBoxGUI(rect, "Graph Properties", TextAnchor.MiddleRight, Color.white, Color.white, Color.white);

            GUILayout.BeginArea(new Rect(rect.x+5, rect.y+30, rect.width-10, rect.height-35));

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);

            //DashEditorCore.selectedBox.DrawInspector();

            GUILayout.EndScrollView();
            GUILayout.EndArea();

            UseEvent(rect);
        }
    }
}
#endif