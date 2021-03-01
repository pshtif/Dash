/*
 *	Created by:  Peter @sHTiF Stefcek
 */

namespace Dash
{
    public interface IEditorGraphAccess
    {
        #if UNITY_EDITOR
        void IncreaseExecutionCount();
        void DecreaseExecutionCount();
        void SetController(DashController p_controller);
#endif
    }
}