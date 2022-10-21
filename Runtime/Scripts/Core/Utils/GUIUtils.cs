/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEngine;

namespace Dash
{
    public class GUIUtils
    {
        public static string GetMaxLabelForStyleAndWidth(GUIStyle p_style, float p_width, string p_label)
        {
            Vector2 size = p_style.CalcSize(new GUIContent(p_label));
            while (p_label.Length > 0 && size.x > p_width)
            {
                p_label = p_label.Substring(0, p_label.Length - 1);
                size = p_style.CalcSize(new GUIContent(p_label));
            }

            return p_label;
        }
    }
}