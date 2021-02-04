/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;
using DG.Tweening;

namespace Dash
{
    public class AnimateTextNodeModel : AnimationNodeBaseModel
    {
        [Order(10)] 
        [TitledGroup("Text")] 
        public float characterDelay = .1f;

    }
}