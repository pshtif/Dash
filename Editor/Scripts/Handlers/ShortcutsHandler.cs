/*
 *	Created by:  Peter @sHTiF Stefcek
 */
#if UNITY_EDITOR

using UnityEngine;

namespace Dash.Editor
{
    public class ShortcutsHandler
    {
        public static void Handle()
        {
            if (!Event.current.control || Event.current.type != EventType.KeyDown)
                return;
            
            switch (Event.current.keyCode)
            {
                case KeyCode.D:
                    SelectionManager.DuplicateSelectedNodes(DashEditorCore.EditorConfig.editingGraph);
                    break;
                case KeyCode.X:
                    SelectionManager.DeleteSelectedNodes(DashEditorCore.EditorConfig.editingGraph);
                    break;
            }
        }
    }
}
#endif