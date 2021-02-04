/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEngine;

namespace Dash
{
    public class UndoUtils
    {
        public static void RecordObject(Object p_object, string p_name) {
#if UNITY_EDITOR
            if ( Application.isPlaying || p_object == null )
                return;
            
            UnityEditor.Undo.RecordObject(p_object, p_name);
#endif
        }
        
        public static void RegisterCompleteObject(Object p_object, string p_name) {
#if UNITY_EDITOR
            if ( Application.isPlaying || p_object == null ) 
                return;
            
            UnityEditor.Undo.RegisterCompleteObjectUndo(p_object, p_name);
#endif
        }
        
        public static void RegisterFullObjectHierarchy(Object p_object, string p_name) {
#if UNITY_EDITOR
            if ( Application.isPlaying || p_object == null ) 
                return;
            
            UnityEditor.Undo.RegisterFullObjectHierarchyUndo(p_object, p_name);
#endif
        }
        
        public static void SetDirty(Object p_object) {
#if UNITY_EDITOR
            if ( Application.isPlaying || p_object == null ) 
                return;
            
            UnityEditor.EditorUtility.SetDirty(p_object);
#endif
        }
    }
}