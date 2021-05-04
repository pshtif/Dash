/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEditor;
using UnityEngine;

namespace Dash
{
    public class PreferencesContextMenu
    {
        public static void Show(DashGraph p_graph)
        {
            GenericMenu menu = new GenericMenu();
            
            menu.AddItem(new GUIContent("Show Experimental"), DashEditorCore.Config.showExperimental, () => DashEditorCore.Config.showExperimental = !DashEditorCore.Config.showExperimental);
            //menu.AddItem(new GUIContent("Show Variables"), p_graph.showVariables, () => p_graph.showVariables = !p_graph.showVariables);
            menu.AddItem(new GUIContent("Show Node Ids"), DashEditorCore.Config.showNodeIds, () => DashEditorCore.Config.showNodeIds = !DashEditorCore.Config.showNodeIds);
            menu.AddItem(new GUIContent("Show Node Search"), DashEditorCore.Config.showNodeSearch, () => DashEditorCore.Config.showNodeSearch = !DashEditorCore.Config.showNodeSearch);


            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Validate Serialization"), false, p_graph.ValidateSerialization);
            menu.AddItem(new GUIContent("Cleanup Null"), false, p_graph.RemoveNullReferences);
            menu.AddItem(new GUIContent("Cleanup Exposed"), false, p_graph.CleanupExposedReferenceTable);
            menu.ShowAsContext();
        }
    }
}