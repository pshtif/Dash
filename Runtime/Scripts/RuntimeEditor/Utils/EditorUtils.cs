/*
 *	Created by:  Peter @sHTiF Stefcek
 */
#if UNITY_EDITOR

using System;
using System.Linq;
using UnityEditor;

namespace Dash
{
    public class EditorUtils
    {
        public static MonoScript GetScriptFromType(Type p_type) {
            if ( p_type == null )
                return null;

            string typeName = p_type.Name;
            if ( p_type.IsGenericType ) {
                p_type = p_type.GetGenericTypeDefinition();
                typeName = typeName.Substring(0, typeName.IndexOf('`'));
            }
            
            return AssetDatabase.FindAssets(string.Format(typeName + " t:MonoScript"))
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<MonoScript>)
                .FirstOrDefault(m => m != null && m.GetClass() == p_type);
        }
    }
}
#endif