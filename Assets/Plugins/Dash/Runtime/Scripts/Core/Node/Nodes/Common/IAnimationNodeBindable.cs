/*
 *	Created by:  Peter @sHTiF Stefcek
 */

namespace Dash
{
    public interface IAnimationNodeBindable
    {
        #if UNITY_EDITOR
        bool IsToEnabled();

        void SetTargetTo(object p_target);

        void GetTargetTo(object p_target);
        
        bool IsFromEnabled();

        void SetTargetFrom(object p_target);

        void GetTargetFrom(object p_target);
        #endif
    }
}