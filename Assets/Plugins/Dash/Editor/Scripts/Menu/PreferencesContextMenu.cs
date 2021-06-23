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
            GenericMenu menu = new GenericMenu();
            
            menu.AddItem(new GUIContent("Settings/Show Experimental"), DashEditorCore.Config.showExperimental, () => DashEditorCore.Config.showExperimental = !DashEditorCore.Config.showExperimental);
            menu.AddItem(new GUIContent("Settings/Show Node Ids"), DashEditorCore.Config.showNodeIds, () => DashEditorCore.Config.showNodeIds = !DashEditorCore.Config.showNodeIds);
            menu.AddItem(new GUIContent("Settings/Show Node Search"), DashEditorCore.Config.showNodeSearch, () => DashEditorCore.Config.showNodeSearch = !DashEditorCore.Config.showNodeSearch);
            menu.AddItem(new GUIContent("Settings/Enable Sound in Editor"), DashEditorCore.Config.enableSoundInPreview, () => DashEditorCore.Config.enableSoundInPreview = !DashEditorCore.Config.enableSoundInPreview);
            
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Advanced/Validate Serialization"), false, p_graph.ValidateSerialization);
            menu.AddItem(new GUIContent("Advanced/Cleanup Null"), false, p_graph.RemoveNullReferences);
            menu.AddItem(new GUIContent("Advanced/Cleanup Exposed"), false, p_graph.CleanupExposedReferenceTable);
            
            menu.AddItem(new GUIContent("Reset Graph Position"), false, p_graph.ResetPosition);
            menu.ShowAsContext();
        }
    }
}