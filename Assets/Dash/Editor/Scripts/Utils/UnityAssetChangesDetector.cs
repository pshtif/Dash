/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEditor;
using UnityEngine;

namespace Dash
{
    public class UnityAssetChangesDetector : AssetPostprocessor
    {
        static void OnPostprocessAllAssets (
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths) 
        {            
            foreach (string str in importedAssets)
            {
                string[] splitStr = str.Split('.');

                if (splitStr.Length < 2)
                    return;
                
                string extension = splitStr[splitStr.Length-1];
                if (extension == "anim")
                {
                    DashEditorCore.RecacheAnimations();
                }
            }
        }
    }
}