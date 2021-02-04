/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;
using DG.Tweening;

namespace Dash
{
    public class AnimationNodeBaseModel : RetargetNodeBaseModel
    {
        [Order(0)]
        [TitledGroup("Animation")]
        [Dependency("useAnimationTime", false)]
        public float time = 1;
        
        [Order(1)]
        [TitledGroup("Animation")]
        public float delay = 0;

        [Order(2)]
        [TitledGroup("Animation")]
        public Ease easing = Ease.Linear;
    }
}