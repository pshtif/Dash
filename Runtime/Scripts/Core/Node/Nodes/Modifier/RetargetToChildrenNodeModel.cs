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
        [UnityEngine.Tooltip("Delay in execution after each child.")]
        [TitledGroup("Properties")]
        public Parameter<float> onChildDelay = new Parameter<float>(0);
        
        [Order(11)]
        [UnityEngine.Tooltip("Delay in execution after last child executed.")]
        [TitledGroup("Properties")]
        public float onFinishDelay = 0;
        
        [Order(12)]
        [UnityEngine.Tooltip("Iterates child in reverse order.")]
        [TitledGroup("Properties")]
        public bool inReverse = false;
    }
}