/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEngine;

namespace Dash
{
    public class RectUtils
    {
        public static bool IsInsideRect(Rect p_insideRect, Rect p_outsideRect, float p_offsetX = 0, float p_offsetY = 0, float p_zoom = 1)
        {
            if (p_outsideRect.Contains(new Vector2((p_insideRect.x + p_offsetX) / p_zoom,
                    (p_insideRect.y + p_offsetY) / p_zoom)) ||
                p_outsideRect.Contains(new Vector2((p_insideRect.x + p_insideRect.width + p_offsetX) / p_zoom,
                    (p_insideRect.y + p_offsetY) / p_zoom)) ||
                p_outsideRect.Contains(new Vector2((p_insideRect.x + p_offsetX) / p_zoom,
                    (p_insideRect.y + p_insideRect.height + p_offsetY) / p_zoom)) ||
                p_outsideRect.Contains(new Vector2(
                    (p_insideRect.x + p_insideRect.width + p_offsetX) / p_zoom,
                    (p_insideRect.y + p_insideRect.height + p_offsetY) / p_zoom))) 
            {
                return true;
            }

            return false;
        }
        
        public static Rect FixRect(Rect p_rect)
        {
            if (p_rect.width < 0)
            {
                p_rect.x += p_rect.width;
                p_rect.width = -p_rect.width;
            }
            if (p_rect.height < 0)
            {
                p_rect.y += p_rect.height;
                p_rect.height = -p_rect.height;
            }

            return p_rect;
        }
    }
}