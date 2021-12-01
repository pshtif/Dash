/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Runtime.InteropServices.ComTypes;
using Dash.Attributes;
using Dash.Enums;
using UnityEngine;

namespace Dash
{
    [Serializable]
    public class AnimateColorNodeModel : AnimationNodeModelBase
    {
        [Order(11)]
        [TitledGroup("Properties")]
        public AlphaTargetType targetType = AlphaTargetType.CANVASGROUP;
        
        [Order(12)]
        [TitledGroup("Properties")]
        public bool useFrom = false;
        
        [Order(13)]
        [TitledGroup("Properties")]
        [Dependency("useFrom", true)]
        [Dependency("targetType", AlphaTargetType.CANVASGROUP)]
        public Parameter<float> fromAlpha = new Parameter<float>(0);
        
        [Order(14)]
        [TitledGroup("Properties")]
        [Dependency("useFrom", true)]
        [Dependency("targetType", AlphaTargetType.CANVASGROUP)]
        public bool isFromRelative = true;
        
        [Order(15)]
        [TitledGroup("Properties")]
        [Dependency("targetType", AlphaTargetType.CANVASGROUP)]
        public Parameter<float> toAlpha = new Parameter<float>(1);
        
        [Order(16)]
        [TitledGroup("Properties")]
        [Dependency("targetType", AlphaTargetType.CANVASGROUP)]
        public bool isToRelative = true;

        [Order(17)]
        [DependencySingle("targetType", AlphaTargetType.IMAGE)]
        [DependencySingle("targetType", AlphaTargetType.TEXTMESHPRO)]
        [Dependency("useFrom", true)]
        [TitledGroup("Properties")]
        public Parameter<Color> fromColor = new Parameter<Color>(Color.white);
        
        [Order(18)]
        [DependencySingle("targetType", AlphaTargetType.IMAGE)]
        [DependencySingle("targetType", AlphaTargetType.TEXTMESHPRO)]
        [TitledGroup("Properties")]
        public Parameter<Color> toColor = new Parameter<Color>(Color.white);
        
        [Order(19)]
        [TitledGroup("Properties")]
        public bool storeToAttribute = false;
        
        [Order(20)]
        [TitledGroup("Properties")]
        [Dependency("storeToAttribute", true)]
        public Parameter<string> storeAttributeName = new Parameter<string>("color");
    }
}