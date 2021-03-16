/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;
using DG.Tweening;

namespace Dash
{
    public class AnimationNodeModelBase : RetargetNodeModelBase
    {
        [Order(0)] 
        [TitledGroup("Animation")]
        [Dependency("useAnimationTime", false)]
        public Parameter<float> time = new Parameter<float>(1);
        
        [Order(1)]
        [TitledGroup("Animation")]
        public Parameter<float> delay = new Parameter<float>(0);

        [Order(2)]
        [TitledGroup("Animation")]
        public Ease easing = Ease.Linear;
    }
}