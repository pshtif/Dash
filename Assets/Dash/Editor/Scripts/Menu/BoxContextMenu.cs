/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEditor;
using UnityEngine;

namespace Dash
{
    public class BoxContextMenu
    {
        static private DashGraph Graph => DashEditorCore.Config.editingGraph;
        
        static public void Show(GraphBox p_region)
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Delete Box"), false, DeleteBox, p_region);
            
            menu.ShowAsContext();
        }
        
        static void DeleteBox(object p_region)
        {
            Undo.RecordObject(Graph, "Delete Box");
            
            Graph.DeleteBox((GraphBox)p_region);
        }
    }
}