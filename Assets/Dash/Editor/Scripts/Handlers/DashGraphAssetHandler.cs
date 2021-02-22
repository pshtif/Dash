/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

namespace Dash
{
    public class DashGraphAssetHandler
    {
        [OnOpenAssetAttribute(1)]
        public static bool OpenDashGraphEditor(int p_instanceID, int p_line)
        {
            Object asset = EditorUtility.InstanceIDToObject(p_instanceID);
            if (asset.GetType() == typeof(DashGraph))
            {
                string path = AssetDatabase.GetAssetPath(asset);
                DashEditorWindow.InitEditorWindow(null);
                DashEditorCore.EditGraph((DashGraph) AssetDatabase.LoadAssetAtPath<DashGraph>(path));
                
                return true;
            }

            return false;
        }
    }
}