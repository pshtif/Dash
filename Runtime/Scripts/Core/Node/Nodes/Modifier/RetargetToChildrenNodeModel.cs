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
        [HideInInspector]
        public float onFinishDelay = 0;

        [Order(11)]
        [UnityEngine.Tooltip("Delay in execution after last child executed.")]
        [TitledGroup("Properties")]
        [Label("onFinishDelay")]
        public Parameter<float> onFinishDelayP = new Parameter<float>(0);

        [Order(12)]
        [UnityEngine.Tooltip("Iterates child in reverse order.")]
        [TitledGroup("Properties")]
        [HideInInspector]
        public bool inReverse = false;

        [Order(12)]
        [UnityEngine.Tooltip("Iterates child in reverse order.")]
        [TitledGroup("Properties")]
        [Label("inReverse")]
        public Parameter<bool> inReverseP = new Parameter<bool>(false);
        
        [Order(13)]
        [UnityEngine.Tooltip("Iterates child that are inactive.")]
        [TitledGroup("Properties")]
        public Parameter<bool> targetInactive = new Parameter<bool>(true);
        
        [OnDeserialized]
        void OnDeserialized()
        {
#pragma warning disable 612, 618
            if (targetInactive == null)
            {
                targetInactive = new Parameter<bool>(true);
            }
            
            if (onFinishDelayP == null)
            {
                onFinishDelayP = new Parameter<float>(onFinishDelay);
            }
            
            if (inReverseP == null)
            {
                inReverseP = new Parameter<bool>(inReverse);
            }
#pragma warning restore 612, 618 
        }
    }
}