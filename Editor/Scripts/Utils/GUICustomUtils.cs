/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEngine;

namespace Dash.Editor
{
    public class GUICustomUtils
    {
        static public void DrawTitle(string p_title)
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
    }
}