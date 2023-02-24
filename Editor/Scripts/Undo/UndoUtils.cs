/*
 *	Created by:  Peter @sHTiF Stefcek
 */
#if UNITY_EDITOR

using UnityEngine;

namespace Dash.Editor
{
    public class UndoUtils
    {
        public static void RecordObject(Object p_object, string p_name) {
            if ( Application.isPlaying || p_object == null )
                return;
            
            UnityEditor.Undo.RecordObject(p_object, p_name);
        }
        
        public static void RegisterCompleteObject(Object p_object, string p_name) {
            if ( Application.isPlaying || p_object == null ) 
                return;

            UnityEditor.Undo.RegisterCompleteObjectUndo(p_object, p_name);
            UnityEditor.Undo.FlushUndoRecordObjects();
        }
        
        public static void RegisterFullObjectHierarchy(Object p_object, string p_name) {
            if ( Application.isPlaying || p_object == null ) 
                return;
            
            UnityEditor.Undo.RegisterFullObjectHierarchyUndo(p_object, p_name);
        }
        
        public static void SetDirty(Object p_object) {
            if ( Application.isPlaying || p_object == null ) 
                return;
            
            UnityEditor.EditorUtility.SetDirty(p_object);
        }
    }
}
#endif