/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System.Runtime.Serialization;
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
        public Parameter<float> onFinishDelay = new Parameter<float>(0);

        [Order(12)] 
        [UnityEngine.Tooltip("Iterates child in reverse order.")]
        [TitledGroup("Properties")]
        public Parameter<bool> inReverse = new Parameter<bool>(false);

        [Order(13)]
        [UnityEngine.Tooltip("Iterates child that are inactive.")]
        [TitledGroup("Properties")]
        public Parameter<bool> targetInactive = new Parameter<bool>(true);
        
    }
}