/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEditor;
using UnityEngine;

namespace Dash
{
    public class DebugItemContextMenu
    {
        public static void Show(DebugItem p_debug)
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Go to Node"), false, () => GoToNode(p_debug));

            menu.AddSeparator("");
            menu.ShowAsContext();
        }

        static void GoToNode(DebugItem p_debug)
        {
            DashController controller = DashCore.Instance.GetControllerByName(p_debug.controllerName);
            var graphPath = p_debug.graphPath.IndexOf("/") >= 0 ? p_debug.graphPath.Substring(p_debug.graphPath.IndexOf("/")+1) : "";
            
            DashEditorCore.GoToNode(controller, graphPath, p_debug.nodeId);
        }
    }
}