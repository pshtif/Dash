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
            GenericMenu menu = new GenericMenu();
            
            menu.AddItem(new GUIContent("Import JSON"), false, () => GraphUtils.ImportJSON(p_graph));
            menu.AddItem(new GUIContent("Export JSON"), false, () => GraphUtils.ExportJSON(p_graph));
            
            menu.ShowAsContext();
        }
    }
}