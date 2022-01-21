/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEngine;

namespace Dash
{
    public interface IInternalGraphAccess
    {
        void SetParentGraph(DashGraph p_graph);
        
        // void AddActiveTween(DashTween p_tween);
        //
        // void RemoveActiveTween(DashTween p_tween);
        //
        // void StopActiveTweens(System.Object p_target);

        void OutputExecuted(OutputNode p_node, NodeFlowData p_data);
        
        void SetVersion(int p_version);
    }
}