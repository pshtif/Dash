/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using DG.Tweening;

namespace Dash
{
    public interface IInternalGraphAccess
    {
        DashGraph parentGraph { set; }
        
        void AddActiveTween(UnityEngine.Object p_target, Tween p_tween);
        
        void RemoveActiveTween(Tween p_tween);

        void StopActiveTweens(UnityEngine.Object p_target, bool p_complete = false);

        void OutputExecuted(OutputNode p_node, NodeFlowData p_data);
    }
}