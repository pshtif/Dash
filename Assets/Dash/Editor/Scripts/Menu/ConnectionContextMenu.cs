/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEditor;
using UnityEngine;

namespace Dash
{
    public class ConnectionContextMenu
    {
        static private DashGraph Graph => DashEditorCore.Config.editingGraph;
        
        static public void Show(NodeConnection p_connection)
        {
            GenericMenu menu = new GenericMenu();

            if (p_connection.active)
            {
                menu.AddItem(new GUIContent("Deactivate connection."), false, DeactivateConnection, p_connection);

            }
            else
            {
                menu.AddItem(new GUIContent("Activate connection."), false, ActivateConnection, p_connection);
            }

            menu.AddItem(new GUIContent("Delete Connection"), false, DeleteConnection, p_connection);
            
            menu.ShowAsContext();
        }
        
        static void DeleteConnection(object p_connection)
        {
            Undo.RecordObject(Graph, "Delete connection.");
            Graph.Disconnect((NodeConnection)p_connection);
        }
        
        static void DeactivateConnection(object p_connection)
        {
            ((NodeConnection) p_connection).active = false;
        }
        
        static void ActivateConnection(object p_connection)
        {
            ((NodeConnection) p_connection).active = true;
        }
    }
}