/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using UnityEngine;

namespace Dash
{
    [Serializable]
    public class NodeConnectionPoint : IDraggable
    {
        public DashGraph Graph => DashEditorCore.EditorConfig.editingGraph;
        
        public Vector2 position;

        private NodeConnection _connection;
        private bool _isDragging = false;

        public NodeConnectionPoint(NodeConnection p_connection, Vector2 p_position)
        {
            _connection = p_connection;
            position = p_position;
        }

        public void DrawGUI(Color p_connectionColor)
        {
            var op = position + Graph.viewOffset;
            var rect = new Rect(op.x - 6, op.y - 6, 12, 12);

            //GUI.color = buttonRect.Contains(Event.current.mousePosition) ? Color.green : p_connectionColor;
            GUI.color = p_connectionColor;
            GUI.Box(rect, "", DashEditorCore.Skin.GetStyle("NodeReconnect"));

            HandleMouse(rect);
        }

        private void HandleMouse(Rect p_rect)
        {
            if (Event.current.button == 0)
            {
                if (Event.current.type == EventType.MouseDown && p_rect.Contains(Event.current.mousePosition))
                {
                    _isDragging = true;
                    Event.current.Use();
                }

                if (Event.current.type == EventType.MouseDrag && _isDragging)
                {
                    position += Event.current.delta * DashEditorCore.EditorConfig.zoom;
                    Event.current.Use();
                }

                if (Event.current.type == EventType.MouseUp)
                {
                    _isDragging = false;
                }
            } else if (Event.current.button == 1)
            {
                if (Event.current.type == EventType.MouseUp && p_rect.Contains(Event.current.mousePosition))
                {
                    EditorAssemblyCaller.Call("ConnectionContextMenu", "Show", new object[] { _connection, -1, this });
                }
            }
        }
    }
}