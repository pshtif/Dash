/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEditor;
using UnityEngine;

namespace Dash
{
    public class GraphViewMenu
    {
        
        
        public void Draw(DashGraph p_graph)
        {
            if (p_graph != null)
            {
                if (GUI.Button(new Rect(0, 1, 100, 22), "File"))
                {
                    FileMenu(p_graph);
                }

                GUI.DrawTexture(new Rect(80, 6, 10, 10), IconManager.GetIcon("ArrowDown_Icon"));

                if (GUI.Button(new Rect(102, 1, 120, 22), "Preferences"))
                {
                    PreferencesMenu(p_graph);
                }
                
                GUI.DrawTexture(new Rect(202, 6, 10, 10), IconManager.GetIcon("ArrowDown_Icon"));
            }
        }
        
        void FileMenu(DashGraph p_graph)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Import JSON"), false, () => GraphUtils.ImportJSON(p_graph));
            menu.AddItem(new GUIContent("Export JSON"), false, () => GraphUtils.ExportJSON(p_graph));
            menu.ShowAsContext();
        }
        
        void PreferencesMenu(DashGraph p_graph)
        {
            GenericMenu menu = new GenericMenu();
            
            menu.AddItem(new GUIContent("Show Experimental"), DashEditorCore.Config.showExperimental, () => DashEditorCore.Config.showExperimental = !DashEditorCore.Config.showExperimental);
            menu.AddItem(new GUIContent("Show Variables"), p_graph.showVariables, () => p_graph.showVariables = !p_graph.showVariables);
            menu.AddItem(new GUIContent("Show Node Ids"), DashEditorCore.Config.showIds, () => DashEditorCore.Config.showIds = !DashEditorCore.Config.showIds);
            
            menu.AddItem(new GUIContent("Validate Serialization"), false, p_graph.ValidateSerialization);
            menu.AddItem(new GUIContent("Cleanup Null"), false, p_graph.RemoveNullReferences);
            menu.AddItem(new GUIContent("Cleanup Exposed"), false, p_graph.CleanupExposedReferenceTable);
            menu.ShowAsContext();
        }
    }
}