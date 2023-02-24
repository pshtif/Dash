/*
 *	Created by:  Peter @sHTiF Stefcek
 */
#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;

namespace Dash.Editor
{
    public class TextureUtils
    {
        private static Dictionary<Color, Texture2D> _cachedColorTextures;
        public static Texture2D GetColorTexture(Color p_color)
        {
            if (_cachedColorTextures == null)
            {
                _cachedColorTextures = new Dictionary<Color, Texture2D>();
            }

            if (_cachedColorTextures.ContainsKey(p_color) && _cachedColorTextures[p_color] != null)
            {
                return _cachedColorTextures[p_color];
            }

            var tex = new Texture2D(4, 4);
            var cols = tex.GetPixels();
            for (int i = 0; i < cols.Length; i++)
            {
                cols[i] = p_color;
            }

            tex.SetPixels(cols);
            tex.Apply();

            _cachedColorTextures[p_color] = tex;
            return tex;
        }
    }
}
#endif