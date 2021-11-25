/*
 *	Created by:  Peter @sHTiF Stefcek
 */
#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Dash
{
    public class SelectionUtils
    {
        public static Transform[] GetTransformsFromSelection()
        {
            return Selection.objects.OfType<GameObject>().Select(go => go.transform).ToArray();
        }
    }
}
#endif