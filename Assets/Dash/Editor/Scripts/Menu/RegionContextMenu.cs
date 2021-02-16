/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEditor;
using UnityEngine;

namespace Dash
{
    public class RegionContextMenu
    {
        static private DashGraph Graph => DashEditorCore.Config.editingGraph;
        
        static public void Show(GraphBox p_region)
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Delete Region"), false, DeleteRegion, p_region);
            
            menu.ShowAsContext();
        }
        
        static void DeleteRegion(object p_region)
        {
            Undo.RecordObject(Graph, "Delete region.");
            Graph.DeleteBox((GraphBox)p_region);
        }
    }
}