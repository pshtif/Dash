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
#if UNITY_6000_5_OR_NEWER
        public static bool OpenDashGraphEditor(EntityId p_entityId, int p_line)
        {
            Object asset = EditorUtility.EntityIdToObject(p_entityId);
#else
        public static bool OpenDashGraphEditor(int p_instanceID, int p_line)
        {
            Object asset = EditorUtility.InstanceIDToObject(p_instanceID);
#endif
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