/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEditor;
using UnityEngine;

namespace Dash.Editor
{
    public class PreferencesContextMenu
    {
        public static void Show(DashGraph p_graph)
        {
            RuntimeGenericMenu menu = new RuntimeGenericMenu();
            
            menu.AddItem(new GUIContent("Settings/Show Experimental"), DashEditorCore.EditorConfig.showExperimental, () => DashEditorCore.EditorConfig.showExperimental = !DashEditorCore.EditorConfig.showExperimental);
            menu.AddItem(new GUIContent("Settings/Show Node Ids"), DashEditorCore.EditorConfig.showNodeIds, () => DashEditorCore.EditorConfig.showNodeIds = !DashEditorCore.EditorConfig.showNodeIds);
            menu.AddItem(new GUIContent("Settings/Show Node Search"), DashEditorCore.EditorConfig.showNodeSearch, () => DashEditorCore.EditorConfig.showNodeSearch = !DashEditorCore.EditorConfig.showNodeSearch);
            menu.AddItem(new GUIContent("Settings/Show Node Asynchronity"), DashEditorCore.EditorConfig.showNodeAsynchronity, () => DashEditorCore.EditorConfig.showNodeAsynchronity = !DashEditorCore.EditorConfig.showNodeAsynchronity);
            menu.AddItem(new GUIContent("Settings/Enable Sound in Editor"), DashEditorCore.EditorConfig.enableSoundInPreview, () => DashEditorCore.EditorConfig.enableSoundInPreview = !DashEditorCore.EditorConfig.enableSoundInPreview);
            
            menu.AddSeparator("");
            //menu.AddItem(new GUIContent("Advanced/Validate Serialization"), false, p_graph.ValidateSerialization);
            menu.AddItem(new GUIContent("Advanced/Cleanup Null"), false, p_graph.RemoveNullReferences);
            menu.AddItem(new GUIContent("Advanced/Cleanup Exposed"), false, p_graph.CleanupExposedReferenceTable);
            
            menu.AddItem(new GUIContent("Reset Graph Position"), false, p_graph.ResetPosition);
            
            //menu.ShowAsEditorMenu();
            GenericMenuPopup.Show(menu, "",  Event.current.mousePosition, 240, 300, false, false);
        }
    }
}