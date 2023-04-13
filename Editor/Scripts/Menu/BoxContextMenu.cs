/*
 *	Created by:  Peter @sHTiF Stefcek
 */
#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Dash.Editor
{
    public class BoxContextMenu
    {
        static public void Show(DashGraph p_graph, GraphBox p_region)
        {
            RuntimeGenericMenu menu = new RuntimeGenericMenu();

            menu.AddItem(new GUIContent("Delete Box"), false, () => DeleteBox(p_graph, p_region));
            
            //menu.ShowAsContext();
            GenericMenuPopup.Show(menu, "",  Event.current.mousePosition, 200, 300, false, false);
        }
        
        static void DeleteBox(DashGraph p_graph, GraphBox p_region)
        {
            Undo.RegisterCompleteObjectUndo(p_graph, "Delete Box");
            
            p_graph.DeleteBox((GraphBox)p_region);
        }
    }
}
#endif