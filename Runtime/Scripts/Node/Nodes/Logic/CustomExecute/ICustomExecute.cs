/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;

namespace Dash
{
    public interface ICustomExecute
    {
        void Execute(NodeFlowData p_flowData, Action p_callback);

        void Stop();
    }
}