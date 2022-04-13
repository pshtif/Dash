/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEditor;
using UnityEngine;

namespace Dash.Editor
{
    public class ConnectionContextMenu
    {
        static private Vector2 _lastMousePosition;
        static private int _lastHitIndex;
        static private NodeConnectionPoint _lastPoint;
        static private DashGraph Graph => DashEditorCore.EditorConfig.editingGraph;

        static public void Show(NodeConnection p_connection, int p_hitIndex, NodeConnectionPoint p_point = null)
        {
            _lastMousePosition = DashEditorWindow.GraphView.MousePosition;
            _lastHitIndex = p_hitIndex;
            _lastPoint = p_point;
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

            if (p_point != null)
            {
                menu.AddItem(new GUIContent("Delete Point"), false, DeleteConnectionPoint, p_connection);
            }
            else
            {
                menu.AddItem(new GUIContent("Add Point"), false, AddConnectionPoint, p_connection);
            }

            //menu.ShowAsContext();
            if (p_point == null)
            {
                GenericMenuPopup.Show(menu, "", _lastMousePosition, 200, 300, false, false);
            }
            else
            {
                GenericMenuPopup.ShowLater(menu, "", _lastMousePosition, 200, 300, false, false);
            }
        }

        static void DeleteConnectionPoint(object p_connection)
        {
            Undo.RegisterCompleteObjectUndo(Graph, "Delete Connection Point");
            ((NodeConnection)p_connection).DeletePoint(_lastPoint);
            DashEditorCore.SetDirty();
        }
        
        static void AddConnectionPoint(object p_connection)
        {
            float zoom = DashEditorCore.EditorConfig.zoom;
            Vector2 offset = DashEditorCore.EditorConfig.editingGraph.viewOffset;
            Vector2 position = new Vector2(_lastMousePosition.x * zoom - offset.x, _lastMousePosition.y * zoom - offset.y);
            
            Undo.RegisterCompleteObjectUndo(Graph, "Add Connection Point");
            ((NodeConnection)p_connection).AddPoint(position, _lastHitIndex);
            DashEditorCore.SetDirty();
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