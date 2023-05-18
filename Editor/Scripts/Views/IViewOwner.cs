/*
 *	Created by:  Peter @sHTiF Stefcek
 */

namespace Dash.Editor
{
    public interface IViewOwner
    {
        DashEditorConfig GetConfig();
        
        void EditController(DashController p_controller, string p_path);

        void EditGraph(DashGraph p_graph, string p_path);
    }
}