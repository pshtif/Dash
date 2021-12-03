/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEngine;

namespace Dash
{
    public class TransformUtils
    {
        public static Vector2 FromToRectTransform(RectTransform p_from, RectTransform p_to)
        {
            Vector2 local;
            Vector2 fromPivotOffset= new Vector2(p_from.rect.width * 0.5f + p_from.rect.xMin, p_from.rect.height * 0.5f + p_from.rect.yMin);
            Vector2 screen = RectTransformUtility.WorldToScreenPoint(null, p_from.position);
            screen += fromPivotOffset;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(p_to, screen, null, out local);
            Vector2 toPivotOffset = new Vector2(p_to.rect.width * 0.5f + p_to.rect.xMin, p_to.rect.height * 0.5f + p_to.rect.yMin);
            return p_to.anchoredPosition + local - toPivotOffset;
        }
    }
}