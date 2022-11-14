/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEngine;

namespace Dash
{
    public class GUIUtils
    {
        public static string GetMaxTextForStyleAndWidth(GUIStyle p_style, float p_width, string p_label, string p_suffix = "")
        {
            var maxLabel = p_label;
            Vector2 size = p_style.CalcSize(new GUIContent(maxLabel));
            while (maxLabel.Length > 0 && size.x > p_width)
            {
                maxLabel = maxLabel.Substring(0, maxLabel.Length - 1);
                size = p_style.CalcSize(new GUIContent(maxLabel));
                
            }

            if (maxLabel != p_label)
            {
                maxLabel += p_suffix;
            }

            return maxLabel;
        }
    }
}