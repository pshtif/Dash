/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEditor;
using UnityEngine;

namespace Dash.Editor
{
    public class GraphFileContextMenu
    {
        public static void Show(DashGraph p_graph)
        {
            RuntimeGenericMenu menu = new RuntimeGenericMenu();
            
            menu.AddItem(new GUIContent("Import JSON"), false, () => GraphUtils.ImportJSON(p_graph));
            menu.AddItem(new GUIContent("Export JSON"), false, () => GraphUtils.ExportJSON(p_graph));
            
            //menu.ShowAsContext();
            GenericMenuPopup.Show(menu, "File",  Event.current.mousePosition, 240, 300, false, false);
        }
    }
}