/*
 *	Created by:  Peter @sHTiF Stefcek
 */
#if UNITY_EDITOR

namespace Dash.Editor
{
    public enum DraggingType
    {
        NONE,
        NODE_DRAG,
        CONNECTION_DRAG,
        SELECTION,
        BOX_DRAG, 
        BOX_RESIZE
    }
}
#endif