/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
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

        public GraphRegion(string p_comment, Rect p_rect)
        {
            comment = p_comment;
            rect = p_rect;
        }

        public void DrawGUI()
        {
            Rect offsetRect = new Rect(rect.x + Graph.viewOffset.x, rect.y + Graph.viewOffset.y, rect.width, rect.height);
            
            GUI.Box(offsetRect, comment, DashEditorCore.Skin.GetStyle("GraphRegion"));
        }

        public void StartDrag()
        {
            _draggedNodes = Graph.Nodes.FindAll(n =>
                rect.Contains(new Vector2(n.rect.x, n.rect.y)) &&
                rect.Contains(new Vector2(n.rect.x + n.rect.width, n.rect.y + n.rect.height)));
        }

        public void Drag(Vector2 p_offset)
        {
            _draggedNodes.ForEach(n => n.rect.position += p_offset);

            rect.position += p_offset;
        }
    }
}