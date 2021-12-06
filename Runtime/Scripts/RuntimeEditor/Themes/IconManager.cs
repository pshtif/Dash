/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
namespace Dash
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
            else
            {
                Debug.Log("Icon not found "+p_name);
            }

            return texture;
        }
    }
}
#endif