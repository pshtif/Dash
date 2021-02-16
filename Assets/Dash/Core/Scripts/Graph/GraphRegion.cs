/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Dash
{
    [Serializable]
    public class GraphRegion
    {
        public DashGraph Graph => DashEditorCore.Config.editingGraph;
        
        public string comment;
        
        public Rect rect;

        public Rect titleRect => new Rect(rect.x, rect.y, rect.width, 45);

        private List<NodeBase> _draggedNodes = new List<NodeBase>();
        private double _lastClickTime = 0;
        private bool _isDragging = false;

        public GraphRegion(string p_comment, Rect p_rect)
        {
            comment = p_comment;
            rect = p_rect;
        }

        public void DrawGUI()
        {
            Rect offsetRect = new Rect(rect.x + Graph.viewOffset.x, rect.y + Graph.viewOffset.y, rect.width, rect.height);
            

            if (_isDragging) GUI.color = Color.green;

            GUI.Box(offsetRect, "", DashEditorCore.Skin.GetStyle("GraphRegion"));

            Rect titleRect = new Rect(offsetRect.x + 12, offsetRect.y, offsetRect.width, 40);
            if (Event.current.type == EventType.MouseDown && titleRect.Contains(Event.current.mousePosition))
            {
                if (EditorApplication.timeSinceStartup - _lastClickTime < 0.3)
                {
                    DashEditorCore.editingRegionTitle = true;
                }
                _lastClickTime = EditorApplication.timeSinceStartup;
            }

            GUIStyle style = new GUIStyle();
            style.fontStyle = FontStyle.Bold;
            style.fontSize = 24;
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.LowerLeft;
            if (DashEditorCore.editingRegionTitle)
            {
                comment = GUI.TextField(titleRect, comment, style);
            }
            else
            {
                GUI.Label(titleRect, comment, style);
            }
            
        }

        public void StartDrag()
        {
            _isDragging = true;
            _draggedNodes = Graph.Nodes.FindAll(n =>
                rect.Contains(new Vector2(n.rect.x, n.rect.y)) &&
                rect.Contains(new Vector2(n.rect.x + n.rect.width, n.rect.y + n.rect.height)));
        }

        public void Drag(Vector2 p_offset)
        {
            _draggedNodes.ForEach(n => n.rect.position += p_offset);

            rect.position += p_offset;
        }

        public void EndDrag()
        {
            _isDragging = false;
        }
    }
}