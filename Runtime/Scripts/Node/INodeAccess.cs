/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using Dash.Editor;

namespace Dash
{
    public interface INodeAccess
    {
        Action<NodeConnection> OnConnectionRemoved { get; set; }
        void Initialize();
        void Remove();
        void Stop();
        
#if UNITY_EDITOR
        void GetCustomContextMenu(ref RuntimeGenericMenu p_menu);
#endif
    }
}