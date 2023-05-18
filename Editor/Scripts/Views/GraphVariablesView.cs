/*
 *	Created by:  Peter @sHTiF Stefcek
 */
#if UNITY_EDITOR

using UnityEngine;

namespace Dash.Editor
{
    public class GraphVariablesView : VariablesView
    {
        private Vector2 scrollPosition;

        public override void DrawGUI(Event p_event, Rect p_rect)
        {
            if (Graph == null)
                return;

            DrawVariablesGUI(new Vector2(20,30), Color.white, Graph.variables, ref Graph.graphVariablesMinimized, Graph);
        }
    }
}
#endif