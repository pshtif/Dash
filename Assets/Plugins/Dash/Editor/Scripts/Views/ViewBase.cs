/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using UnityEditor.Graphs;
using UnityEngine;

namespace Dash.Editor
{
    public abstract class ViewBase
    {
        public DashGraph Graph =>  DashEditorCore.EditorConfig.editingGraph;

        public abstract void DrawGUI(Event p_event, Rect p_rect);
        
        public void DrawBoxGUI(Rect p_rect, string p_title, TextAnchor p_titleAlignment, Color p_color)
        {
            GUIStyle style = DashEditorCore.Skin.GetStyle("ViewBase");
            style.alignment = p_titleAlignment;
            
            switch (p_titleAlignment)
            {
                case TextAnchor.UpperLeft:
                    style.contentOffset = new Vector2(10,0);
                    break;
                case TextAnchor.UpperRight:
                    style.contentOffset = new Vector2(-10,0);
                    break;
                default:
                    style.contentOffset = Vector2.zero;
                    break;
            }

            GUI.color = p_color;
            GUI.Box(p_rect, "", style);
            GUI.Box(new Rect(p_rect.x, p_rect.y, p_rect.width, 32), p_title, style);
            GUI.color = Color.white;
        }
        
        public virtual void ProcessEvent(Event p_event, Rect p_rect) { }

        protected void UseEvent(Rect p_rect)
        {
            if (p_rect.Contains(Event.current.mousePosition) &&
                Event.current.isMouse)
            {
                Event.current.type = EventType.Used;
            }
        }
    }
}