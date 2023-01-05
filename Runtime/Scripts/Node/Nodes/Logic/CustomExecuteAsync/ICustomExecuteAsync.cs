/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System.Threading.Tasks;

namespace Dash
{
    public interface ICustomExecuteAsync
    {
        Task Execute(NodeFlowData p_flowData);

        void Stop();
    }
}