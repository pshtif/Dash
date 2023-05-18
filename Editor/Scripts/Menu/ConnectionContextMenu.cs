/*
 *	Created by:  Peter @sHTiF Stefcek
 */
#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Dash.Editor
{
    public class ConnectionContextMenu
    {
        static public void Show(DashGraph p_graph, NodeConnection p_connection)
        {
            RuntimeGenericMenu menu = new RuntimeGenericMenu();

            if (p_connection.active)
            {
                menu.AddItem(new GUIContent("Deactivate Connection"), false, () => DeactivateConnection(p_graph, p_connection));

            }
            else
            {
                menu.AddItem(new GUIContent("Activate Connection"), false, () => ActivateConnection(p_graph, p_connection));
            }

            menu.AddItem(new GUIContent("Delete Connection"), false, () => DeleteConnection(p_graph, p_connection));
            
            //menu.ShowAsContext();
            GenericMenuPopup.Show(menu, "",  Event.current.mousePosition, 200, 300, false, false);
        }
        
        static void DeleteConnection(DashGraph p_graph ,NodeConnection p_connection)
        {
            Undo.RegisterCompleteObjectUndo(p_graph, "Delete Connection");
            p_graph.Disconnect(p_connection);
            p_graph.MarkDirty();
            //DashEditorCore.SetDirty();
        }
        
        static void DeactivateConnection(DashGraph p_graph ,NodeConnection p_connection)
        {
            p_connection.active = false;
            p_graph.MarkDirty();
            //DashEditorCore.SetDirty();
        }
        
        static void ActivateConnection(DashGraph p_graph ,NodeConnection p_connection)
        {
            p_connection.active = true;
            p_graph.MarkDirty();
            //DashEditorCore.SetDirty();
        }
    }
}
#endif