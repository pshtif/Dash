/*
 *	Created by:  Peter @sHTiF Stefcek
 */
#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

namespace Dash.Editor
{
    public class DashGraphAssetHandler
    {
        [OnOpenAsset(1)]
        public static bool OpenDashGraphEditor(int p_instanceID, int p_line)
        {
            Object asset = EditorUtility.InstanceIDToObject(p_instanceID);
            if (asset.GetType() == typeof(DashGraph))
            {
                string path = AssetDatabase.GetAssetPath(asset);
                DashEditorWindow.InitEditorWindow(AssetDatabase.LoadAssetAtPath<DashGraph>(path));

                return true;
            }

            return false;
        }
    }
}
#endif