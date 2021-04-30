/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;

namespace Dash
{
    public class AnimateTextNodeModel : AnimationNodeModelBase
    {
        [Order(10)] 
        [TitledGroup("Text")] 
        public Parameter<float> characterDelay = new Parameter<float>(.1f);

    }
}