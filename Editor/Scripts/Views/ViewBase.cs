/*
 *	Created by:  Peter @sHTiF Stefcek
 */
#if UNITY_EDITOR

using UnityEngine;

namespace Dash.Editor
{
    public abstract class ViewBase
    {
        public DashController Controller => DashEditorCore.EditorConfig.editingController;
        
        public DashGraph Graph =>  DashEditorCore.EditorConfig.editingGraph;

        public abstract void DrawGUI(Event p_event, Rect p_rect);
        
        public void DrawBoxGUI(Rect p_rect, string p_title, TextAnchor p_titleAlignment, Color p_boxColor, Color p_titleColor, Color p_titleTextColor)
        {
            GUIStyle style = DashEditorCore.Skin.GetStyle("ViewBase");

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

            GUI.color = p_boxColor;
            GUI.Box(p_rect, "", style);
            GUI.color = p_titleColor;
            var rect = new Rect(p_rect.x, p_rect.y, p_rect.width, 32);
            GUI.Box(rect, "", style);
            style = new GUIStyle();
            style.alignment = p_titleAlignment;
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.white;
            style.fontSize = 14;
            style.padding.right = p_titleAlignment == TextAnchor.MiddleCenter ? 0 : 8;
            style.padding.bottom = 3;
            GUI.color = p_titleTextColor;
            GUI.Label(rect, p_title, style);
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
#endif