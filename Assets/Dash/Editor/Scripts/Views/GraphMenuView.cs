/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEngine;

namespace Dash
{
    public class GraphMenuView
    {
        public void Draw(DashGraph p_graph)
        {
            if (p_graph != null)
            {
                if (GUI.Button(new Rect(0, 1, 100, 22), "File"))
                {
                    GraphFileContextMenu.Show(p_graph);
                }

                GUI.DrawTexture(new Rect(80, 6, 10, 10), IconManager.GetIcon("ArrowDown_Icon"));

                if (GUI.Button(new Rect(102, 1, 120, 22), "Preferences"))
                {
                    PreferencesContextMenu.Show(p_graph);
                }
                
                GUI.DrawTexture(new Rect(202, 6, 10, 10), IconManager.GetIcon("ArrowDown_Icon"));
            }
        }
    }
}