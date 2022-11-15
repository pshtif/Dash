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

        [Order(11)]
        [UnityEngine.Tooltip("Delay in execution after last child executed.")]
        [TitledGroup("Properties")]
        [HideInInspector]
        public Parameter<float> onFinishDelayP = new Parameter<float>(0);

        [Order(12)] 
        [UnityEngine.Tooltip("Iterates child in reverse order.")]
        [TitledGroup("Properties")]
        public Parameter<bool> inReverse = new Parameter<bool>(false);

        [Order(12)]
        [UnityEngine.Tooltip("Iterates child in reverse order.")]
        [TitledGroup("Properties")]
        [HideInInspector]
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
            
            if (onFinishDelay == null)
            {
                onFinishDelay = new Parameter<float>(0);

                if (onFinishDelayP != null)
                {
                    if (onFinishDelayP.isExpression)
                    {
                        onFinishDelay.expression = onFinishDelayP.expression;
                    }
                    else
                    {
                        onFinishDelay.SetValue(onFinishDelayP.GetValue(null));
                    }
                }
            }
            
            if (inReverse == null)
            {
                inReverse = new Parameter<bool>(false);
                if (inReverseP != null)
                {
                    if (inReverseP.isExpression)
                    {
                        inReverse.expression = inReverseP.expression;
                    }
                    else
                    {
                        inReverse.SetValue(inReverseP.GetValue(null));
                    }
                }
            }
#pragma warning restore 612, 618 
        }
    }
}