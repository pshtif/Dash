/*
 *	Created by:  Peter @sHTiF Stefcek
 */

namespace Dash
{
    public interface IAnimationNodeBindable
    {
        bool IsToEnabled();

        void SetTargetTo(object p_target);

        void BindTargetTo(object p_target);
        
        bool IsFromEnabled();

        void SetTargetFrom(object p_target);

        void BindTargetFrom(object p_target);
    }
}