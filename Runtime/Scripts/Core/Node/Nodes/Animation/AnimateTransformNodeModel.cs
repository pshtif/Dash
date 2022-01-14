/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    [Serializable]
    public class AnimateTransformNodeModel : AnimationNodeModelBase
    {
        [Order(11)] 
        [TitledGroup("Position")]
        public Parameter<bool> usePosition = new Parameter<bool>(false);

        [Order(12)] 
        [Dependency("usePosition", true)]
        [TitledGroup("Position")]
        public Parameter<bool> useFromPosition = new Parameter<bool>(false);
        
        [Order(13)]
        [TitledGroup("Position")]
        public Parameter<Vector3> fromPosition = new Parameter<Vector3>(Vector3.zero);

        [Order(14)]
        [TitledGroup("Position")]
        public Parameter<bool> isFromPositionRelative = new Parameter<bool>(false);
        
        [Order(15)]
        [TitledGroup("Position")]
        public Parameter<Vector3> toPosition = new Parameter<Vector3>(Vector3.zero);

        [Order(16)] 
        [TitledGroup("Position")]
        public Parameter<bool> isToPositionRelative = new Parameter<bool>(false);

        [Order(17)] 
        [TitledGroup("Position")]
        public Parameter<bool> storePositionToAttribute = new Parameter<bool>(false);
        
        [Order(18)]
        [TitledGroup("Position")]
        public Parameter<string> storePositionAttributeName = new Parameter<string>("position");
        
        [Order(21)]
        [TitledGroup("Rotation")]
        public Parameter<bool> useRotation = new Parameter<bool>(false);
        
        [Order(22)]
        [TitledGroup("Rotation")]
        public Parameter<bool> useFromRotation = new Parameter<bool>(false);
        
        [Order(23)]
        [TitledGroup("Rotation")]
        public Parameter<Vector3> fromRotation = new Parameter<Vector3>(Vector3.zero);

        [Order(24)] 
        [TitledGroup("Rotation")]
        public Parameter<bool> isFromRotationRelative = new Parameter<bool>(false);
        
        [Order(25)]
        [TitledGroup("Rotation")]
        public Parameter<Vector3> toRotation = new Parameter<Vector3>(Vector3.zero);
        
        [Order(26)]
        [TitledGroup("Rotation")]
        public Parameter<bool> isToRotationRelative = new Parameter<bool>(false);
        
        [Order(27)]
        [TitledGroup("Rotation")]
        public Parameter<bool> storeRotationToAttribute = new Parameter<bool>(false);
        
        [Order(28)]
        [TitledGroup("Rotation")]
        public Parameter<string> storeRotationAttributeName = new Parameter<string>("rotation");
        
        [Order(31)]
        [TitledGroup("Scale")]
        public Parameter<bool> useScale = new Parameter<bool>(false);
        
        [Order(32)]
        [TitledGroup("Scale")]
        public Parameter<bool> useFromScale = new Parameter<bool>(false);
        
        [Order(33)]
        [TitledGroup("Scale")]
        public Parameter<Vector3> fromScale = new Parameter<Vector3>(Vector3.one);

        [Order(34)] 
        [TitledGroup("Scale")] 
        public Parameter<bool> isFromScaleRelative = new Parameter<bool>(false);
        
        [Order(35)]
        [TitledGroup("Scale")]
        public Parameter<Vector3> toScale = new Parameter<Vector3>(Vector3.one);
        
        [Order(36)]
        [TitledGroup("Scale")]
        public Parameter<bool> isToScaleRelative = new Parameter<bool>(false);
        
        [Order(37)]
        [TitledGroup("Scale")]
        public Parameter<bool> storeScaleToAttribute = new Parameter<bool>(false);
        
        [Order(38)]
        [TitledGroup("Scale")]
        public Parameter<string> storeScaleAttributeName = new Parameter<string>("scale");
    }
}