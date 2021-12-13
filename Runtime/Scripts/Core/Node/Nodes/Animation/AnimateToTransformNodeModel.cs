/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    [Serializable]
    public class AnimateToTransformNodeModel : AnimationNodeModelBase
    {
        // [Order(10)] 
        // [TitledGroup("Target")] 
        // public ExposedReference<Transform> targetTransform;
        
        [Order(10)]
        [TitledGroup("Target")]
        [Expression("useToExpression", "targetToExpression")]
        [UnityEngine.Tooltip("Reference of transform to animate to.")]
        public ExposedReference<Transform> targetTransform;
        
        [HideInInspector]
        public bool useToExpression = false;
        
        [HideInInspector]
        public string targetToExpression = "";
        
        [Order(11)] 
        [TitledGroup("Target")]
        public Parameter<bool> useRectTransform = new Parameter<bool>(false);

        [Order(12)]
        [TitledGroup("Target")]
        public Parameter<bool> useToPosition = new Parameter<bool>(false);

        [Order(13)]
        [TitledGroup("Target")]
        public Parameter<bool> useToRotation = new Parameter<bool>(false);

        [Order(14)]
        [TitledGroup("Target")] 
        public Parameter<bool> useToScale = new Parameter<bool>(false);
    }
}