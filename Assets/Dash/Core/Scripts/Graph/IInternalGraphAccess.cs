/*
 *	Created by:  Peter @sHTiF Stefcek
 */

namespace Dash
{
    public interface IInternalGraphAccess
    {
        DashGraph parentGraph { set; }

        void OutputExecuted(OutputNode p_node, NodeFlowData p_data);
    }
}