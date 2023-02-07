/*
 *	Created by:  Peter @sHTiF Stefcek
 */
#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;

namespace Dash.Editor
{
    public class IconManager
    {
        static private Dictionary<string, Texture> _cache = new Dictionary<string, Texture>();
        
        public static Texture GetIcon(string p_name)
        {
            if (_cache.ContainsKey(p_name))
                return _cache[p_name];

            Texture texture = Resources.Load<Texture>("Textures/Icons/"+p_name);
            if (texture != null)
            {
                _cache.Add(p_name, texture);
            }

            return texture;
        }
    }
}
#endif