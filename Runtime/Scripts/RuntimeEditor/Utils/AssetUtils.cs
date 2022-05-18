/*
 *	Created by:  Peter @sHTiF Stefcek
 */

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;

namespace Dash
{
    public class AssetUtils
    {
        public static List<T> FindAllAssetsByType<T>() where T : UnityEngine.Object
        {
            List<T> assets = new List<T>();
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));
            for( int i = 0; i < guids.Length; i++ )
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if( asset != null )
                {
                    assets.Add(asset);
                }
            }
            return assets;
        }
    }
}
#endif