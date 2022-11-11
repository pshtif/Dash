/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash;
using UnityEditor;

namespace Dash.Editor
{
    public class DashUnityMenu
    {
        [MenuItem("Tools/Dash/Settings")]
        public static void ShowInspectorLogo()
        {
            SettingsWindow.InitEditorWindow();
            // DashEditorCore.EditorConfig.settingsShowInspectorLogo = !DashEditorCore.EditorConfig.settingsShowInspectorLogo;
            // EditorUtility.SetDirty(DashEditorCore.EditorConfig);
        }
        
        // [MenuItem("Tools/Dash/Settings/Show Inspector Logo", true)]
        // private static bool SettingsMenuValidator()
        // {
        //     Menu.SetChecked("Tools/Dash/Settings/Show Inspector Logo", DashEditorCore.EditorConfig.settingsShowInspectorLogo);
        //     return true;
        // }
    }
}