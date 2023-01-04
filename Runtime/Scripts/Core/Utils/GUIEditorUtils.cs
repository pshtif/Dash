/*
 *	Created by:  Peter @sHTiF Stefcek
 */

#if UNITY_EDITOR

using Dash.Editor;
using UnityEngine;

namespace Dash
{
    public class GUIEditorUtils
    {
        public static void DrawTitle(string p_title)
        {
            var titleStyle = new GUIStyle();
            titleStyle.alignment = TextAnchor.MiddleCenter;
            titleStyle.normal.textColor = new Color(1, .7f, 0);
            titleStyle.normal.background = Texture2D.whiteTexture;
            titleStyle.fontStyle = FontStyle.Bold;
            titleStyle.fontSize = 16;

            GUI.backgroundColor = new Color(0, 0, 0, .35f);
            
            GUILayout.Label(p_title, titleStyle, GUILayout.ExpandWidth(true), GUILayout.Height(30));
            GUILayout.Space(4);

            GUI.backgroundColor = Color.white;
        }
        
        public static void DrawSectionTitle(string p_title)
        {
            var style = new GUIStyle();
            style.normal.textColor = DashEditorCore.EditorConfig.theme.InspectorSectionTitleColor;
            style.alignment = TextAnchor.MiddleCenter;
            style.fontStyle = FontStyle.Bold;
            style.normal.background = Texture2D.whiteTexture;
            style.fontSize = 13;
            GUI.backgroundColor = new Color(0, 0, 0, .5f);
            GUILayout.Label(p_title, style, GUILayout.Height(26));
            GUI.backgroundColor = Color.white;
        }
        
        public static bool DrawMinimizableSectionTitle(string p_title, ref bool p_minimized, int? p_size = null, Color? p_color = null, TextAnchor? p_alignment = null)
        {
            
            var style = new GUIStyle();
            style.normal.textColor = p_color.HasValue ? p_color.Value : DashEditorCore.EditorConfig.theme.InspectorSectionTitleColor;
            style.alignment = p_alignment.HasValue ? p_alignment.Value : TextAnchor.MiddleCenter;
            style.fontStyle = FontStyle.Bold;
            style.normal.background = Texture2D.whiteTexture;
            style.fontSize = p_size.HasValue ? p_size.Value : 13;
            GUI.backgroundColor = new Color(0, 0, 0, .5f);
            GUILayout.Label(p_title, style, GUILayout.Height(26));
            GUI.backgroundColor = Color.white;
            
            var rect = GUILayoutUtility.GetLastRect();

            style = new GUIStyle();
            style.fontSize = p_size.HasValue ? p_size.Value + 6 : 20;
            style.normal.textColor = p_color.HasValue
                ? p_color.Value * 2f / 3
                : DashEditorCore.EditorConfig.theme.InspectorSectionTitleColor * 2f / 3;
            //style.normal.textColor = Color.white;

            GUI.Label(new Rect(rect.x + 6 + (p_minimized ? 0 : 2), rect.y + (p_size.HasValue ? 14 - p_size.Value : 0), 24, 24), p_minimized ? "+" : "-", style);
            
            if (GUI.Button(new Rect(rect.x, rect.y, rect.width, rect.height), "", GUIStyle.none))
            {
                p_minimized = !p_minimized;
            }

            return !p_minimized;
        }
    }
}

#endif