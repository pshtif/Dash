/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;

namespace Dash
{
    public class AnimationNodeModelBase : RetargetNodeModelBase
    {
        [Order(0)] 
        [TitledGroup("Animation",2)]
        public Parameter<float> time = new Parameter<float>(1);
        
        [Order(1)]
        [TitledGroup("Animation",2)]
        public Parameter<bool> useSpeed = new Parameter<bool>(false);
        
        [Order(2)]
        [TitledGroup("Animation",2)]
        public Parameter<float> delay = new Parameter<float>(0);

        [Order(3)]
        [TitledGroup("Animation",2)]
        public Parameter<EaseType> easeType = new Parameter<EaseType>(EaseType.LINEAR);

        [Order(4)]
        [TitledGroup("Animation",2)]
        public Parameter<bool> killActive = new Parameter<bool>(false);
        
        [Order(5)]
        [TitledGroup("Animation", 2)]
        public bool killOnNullEncounter = false;
    }
}