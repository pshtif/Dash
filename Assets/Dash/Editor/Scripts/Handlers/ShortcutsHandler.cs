/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEngine;

namespace Dash
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
                    DashEditorCore.DuplicateSelectedNodes();
                    break;
            }
        }
    }
}