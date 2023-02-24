/*
 *	Created by:  Peter @sHTiF Stefcek
 */
#if UNITY_EDITOR

using UnityEditor;

namespace Dash.Editor
{
    public class AnimationClipChangesHandler : AssetPostprocessor
    {
        static void OnPostprocessAllAssets (
            string[] p_importedAssets,
            string[] p_deletedAssets,
            string[] p_movedAssets,
            string[] p_movedFromAssetPaths)
        {
            bool animChanged = false;
            foreach (string str in p_importedAssets)
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
                DashAnimation.RecacheAnimationAssets();
            }
        }
    }
}
#endif