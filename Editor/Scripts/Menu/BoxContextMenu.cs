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
        static private DashGraph Graph => DashEditorCore.EditorConfig.editingGraph;
        
        static public void Show(GraphBox p_region)
        {
            RuntimeGenericMenu menu = new RuntimeGenericMenu();

            menu.AddItem(new GUIContent("Delete Box"), false, DeleteBox, p_region);
            
            //menu.ShowAsContext();
            GenericMenuPopup.Show(menu, "",  Event.current.mousePosition, 200, 300, false, false);
        }
        
        static void DeleteBox(object p_region)
        {
            Undo.RegisterCompleteObjectUndo(Graph, "Delete Box");
            
            Graph.DeleteBox((GraphBox)p_region);
        }
    }
}
#endif