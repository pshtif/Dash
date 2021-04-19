/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEditor;

namespace Dash
{
    public class AnimationClipChangesHandler : AssetPostprocessor
    {
        static void OnPostprocessAllAssets (
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            bool animChanged = false;
            foreach (string str in importedAssets)
            {
                string[] splitStr = str.Split('.');

                if (splitStr.Length < 2)
                    return;
                
                string extension = splitStr[splitStr.Length-1];
                if (extension == "anim")
                {
                    animChanged = true;
                    break;
                }
            }

            if (animChanged)
            {
                DashEditorCore.RecacheAnimation();
            }
        }
    }
}