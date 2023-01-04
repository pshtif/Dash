/*
 *	Created by:  Peter @sHTiF Stefcek
 */
#if UNITY_EDITOR

using UnityEditor;

namespace Dash.Editor
{
    public class DashCoreModificationProcessor : UnityEditor.AssetModificationProcessor
    {
        static AssetDeleteResult OnWillDeleteAsset(string p_path, RemoveAssetOptions p_options)
        {
            if (AssetDatabase.GetMainAssetTypeAtPath(p_path) == typeof(DashGraph))
                DashEditorCore.ScanGraphAssets();
            
            return AssetDeleteResult.DidNotDelete;
        }
    }
}
#endif