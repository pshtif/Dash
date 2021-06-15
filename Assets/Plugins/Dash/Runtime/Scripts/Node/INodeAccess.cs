/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;

namespace Dash
{
    public interface INodeAccess
    {
        Action<NodeConnection> OnConnectionRemoved { get; set; }
        void Initialize();
        void Remove();
    }
}