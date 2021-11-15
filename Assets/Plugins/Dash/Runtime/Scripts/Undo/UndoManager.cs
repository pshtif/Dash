/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Dash
{
    public class UndoManager
    {
        public static void RegisterUndo(Object p_object, string p_name)
        {
            #if UNITY_EDITOR
            Undo.RegisterCompleteObjectUndo(p_object, p_name);
            #endif
        }
    }
}