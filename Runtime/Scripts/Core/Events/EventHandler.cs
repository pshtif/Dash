/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;

namespace Dash
{
    public class EventHandler
    {
        public int Priority { get; }
        
        public bool Once { get; }

        public Action<NodeFlowData> Callback { get; }

        public EventHandler(Action<NodeFlowData> p_callback, int p_priority, bool p_once)
        {
            Callback = p_callback;
            Priority = p_priority;
            Once = p_once;
        }

        public void Invoke(NodeFlowData p_flowData)
        {
            Callback?.Invoke(p_flowData);
        }
    }
}