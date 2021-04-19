/*
 *	Created by:  Peter @sHTiF Stefcek
 */

namespace Dash
{
    public interface IEditorGraphAccess
    {
        #if UNITY_EDITOR
        void SetController(DashController p_controller);
#endif
    }
}