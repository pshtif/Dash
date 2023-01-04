/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEngine;

namespace Dash.Editor
{
    public class GraphPreferencesContextMenu
    {
        public static void Show(DashGraph p_graph)
        {
            RuntimeGenericMenu menu = new RuntimeGenericMenu();
            
            menu.AddItem(new GUIContent("Settings/Show Experimental"), DashEditorCore.EditorConfig.showExperimental, () => DashEditorCore.EditorConfig.showExperimental = !DashEditorCore.EditorConfig.showExperimental);
            menu.AddItem(new GUIContent("Settings/Show Obsolete"), DashEditorCore.EditorConfig.showObsolete, () => DashEditorCore.EditorConfig.showObsolete = !DashEditorCore.EditorConfig.showObsolete);
            menu.AddItem(new GUIContent("Settings/Show Node Ids"), DashEditorCore.EditorConfig.showNodeIds, () => DashEditorCore.EditorConfig.showNodeIds = !DashEditorCore.EditorConfig.showNodeIds);
            menu.AddItem(new GUIContent("Settings/Show Node Search"), DashEditorCore.EditorConfig.showNodeSearch, () => DashEditorCore.EditorConfig.showNodeSearch = !DashEditorCore.EditorConfig.showNodeSearch);
            menu.AddItem(new GUIContent("Settings/Show Node Asynchronity"), DashEditorCore.EditorConfig.showNodeAsynchronity, () => DashEditorCore.EditorConfig.showNodeAsynchronity = !DashEditorCore.EditorConfig.showNodeAsynchronity);
            menu.AddItem(new GUIContent("Settings/Enable Sound in Editor"), DashEditorCore.EditorConfig.enableSoundInPreview, () => DashEditorCore.EditorConfig.enableSoundInPreview = !DashEditorCore.EditorConfig.enableSoundInPreview);
            menu.AddItem(new GUIContent("Settings/Enable AnimateNode Interface"), DashEditorCore.EditorConfig.enableAnimateNodeInterface, () => DashEditorCore.EditorConfig.enableAnimateNodeInterface = !DashEditorCore.EditorConfig.enableAnimateNodeInterface);
            
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Advanced/Cleanup Null"), false, p_graph.RemoveNullReferences);

            menu.AddItem(new GUIContent("Reset Graph Position"), false, p_graph.ResetPosition);
            
            menu.AddItem(new GUIContent("Help"), false, () =>
            {
                Application.OpenURL("https://github.com/pshtif/Dash/tree/main/Documentation");
            });
            
            GenericMenuPopup.Show(menu, "Preferences",  Event.current.mousePosition, 240, 300, false, false);
        }
    }
}