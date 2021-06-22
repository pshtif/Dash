/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    public class RetargetToChildrenNodeModel : RetargetNodeModelBase
    {
        [Order(10)]
        [Tooltip("Delay in execution after each child.")]
        public Parameter<float> onChildDelay = new Parameter<float>(0);
        
        [Order(11)]
        [Tooltip("Delay in execution after last child executed.")]
        public float onFinishDelay = 0;
        
        [Order(12)]
        [Tooltip("Iterates child in reverse order.")]
        public bool inReverse = false;
    }
}