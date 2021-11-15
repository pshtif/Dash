/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Dash
{
    public class EditorUtils
    {
        
#if UNITY_EDITOR
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
#endif
    }
}