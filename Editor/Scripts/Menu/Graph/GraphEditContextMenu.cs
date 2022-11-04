/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEngine;

namespace Dash.Editor
{
    public class GraphEditContextMenu
    {
        public static void Show(DashGraph p_graph)
        {
            RuntimeGenericMenu menu = new RuntimeGenericMenu();
            
            menu.AddItem(new GUIContent("Copy Variables"), false, () => CopyVariables(p_graph.variables));
            if (VariableUtils.copiedVariables != null)
            {
                menu.AddItem(new GUIContent("Paste Variables"), false, () => PasteVariables(p_graph.variables));
            }

            //menu.ShowAsEditorMenu();
            GenericMenuPopup.Show(menu, "Edit",  Event.current.mousePosition, 240, 300, false, false);
        }

        private static void CopyVariables(DashVariables p_variables)
        {
            VariableUtils.CopyVariables(p_variables);
        }

        private static void PasteVariables(DashVariables p_variables)
        {
            VariableUtils.PasteVariables(p_variables, null);
        }
    }
}