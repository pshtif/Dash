/*
 *	Created by:  Peter @sHTiF Stefcek
 */
#if UNITY_EDITOR

namespace Dash.Editor
{
    public class CheckVersionPopup
    {
        public static bool IsCurrentVersion()
        {
            if (DashEditorCore.EditorConfig.editingGraph != null &&
                DashEditorCore.EditorConfig.editingGraph.version < DashCore.GetVersionNumber())
            {
                return false;
            }

            return true;
        }
    }
}
#endif