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
        static private DashGraph Graph => DashEditorCore.EditorConfig.editingGraph;
        
        static public void Show(NodeConnection p_connection)
        {
            RuntimeGenericMenu menu = new RuntimeGenericMenu();

            if (p_connection.active)
            {
                menu.AddItem(new GUIContent("Deactivate Connection"), false, DeactivateConnection, p_connection);

            }
            else
            {
                menu.AddItem(new GUIContent("Activate Connection"), false, ActivateConnection, p_connection);
            }

            menu.AddItem(new GUIContent("Delete Connection"), false, DeleteConnection, p_connection);
            
            //menu.ShowAsContext();
            GenericMenuPopup.Show(menu, "",  Event.current.mousePosition, 200, 300, false, false);
        }
        
        static void DeleteConnection(object p_connection)
        {
            Undo.RegisterCompleteObjectUndo(Graph, "Delete Connection");
            Graph.Disconnect((NodeConnection)p_connection);
            DashEditorCore.SetDirty();
        }
        
        static void DeactivateConnection(object p_connection)
        {
            ((NodeConnection) p_connection).active = false;
            DashEditorCore.SetDirty();
        }
        
        static void ActivateConnection(object p_connection)
        {
            ((NodeConnection) p_connection).active = true;
            DashEditorCore.SetDirty();
        }
    }
}
#endif