/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash;
using UnityEditor;

namespace Dash.Editor
{
    public class SettingsMenu
    {
        [MenuItem("Tools/Dash/Settings/Show Inspector Logo")]
        public static void ShowInspectorLogo()
        {
            DashEditorCore.EditorConfig.settingsShowInspectorLogo = !DashEditorCore.EditorConfig.settingsShowInspectorLogo;
        }
        
        [MenuItem("Tools/Dash/Settings/Show Inspector Logo", true)]
        private static bool SettingsMenuValidator()
        {
            Menu.SetChecked("Tools/Settings/Show Inspector Logo", DashEditorCore.EditorConfig.settingsShowInspectorLogo);
            return true;
        }
    }
}