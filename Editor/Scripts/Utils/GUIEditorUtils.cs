/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEngine;

namespace Dash.Editor
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
        
        public static bool DrawMinimizableSectionTitle(string p_title, ref bool p_minimized)
        {
            var style = new GUIStyle();
            style.normal.textColor = new Color(1, 0.7f, 0);
            style.alignment = TextAnchor.MiddleCenter;
            style.fontStyle = FontStyle.Bold;
            style.normal.background = Texture2D.whiteTexture;
            style.fontSize = 14;
            GUI.backgroundColor = new Color(0, 0, 0, .5f);
            GUILayout.Label(p_title, style, GUILayout.Height(26));
            GUI.backgroundColor = Color.white;
            
            var rect = GUILayoutUtility.GetLastRect();

            style = new GUIStyle();
            style.fontSize = 20;
            style.normal.textColor = Color.white;
            
            GUI.Label(new Rect(rect.x+rect.width- (p_minimized ? 24 : 21), rect.y, 24, 24), p_minimized ? "+" : "-", style);
            
            if (GUI.Button(new Rect(rect.x, rect.y, rect.width, rect.height), "", GUIStyle.none))
            {
                p_minimized = !p_minimized;
            }

            return !p_minimized;
        }
    }
}