/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    [Serializable]
    public class AnimateSizeDeltaNodeModel : AnimationNodeModelBase
    {
        [Order(11)]
        [TitledGroup("Properties")]
        public bool useFrom = false;
        
        [Order(12)]
        [Dependency("useFrom", true)]
        [TitledGroup("Properties")]
        public Parameter<Vector2> fromSizeDelta = new Parameter<Vector2>(Vector2.zero);
        
        [Order(13)]
        [Dependency("useFrom", true)]
        [TitledGroup("Properties")]
        public bool isFromRelative = false;
        
        [Order(14)]
        [TitledGroup("Properties")]
        public Parameter<Vector2> toSizeDelta = new Parameter<Vector2>(Vector2.zero);
        
        [Order(15)]
        [TitledGroup("Properties")]
        public bool isToRelative = false;
        
        [Order(19)]
        [TitledGroup("Properties")]
        public bool storeToAttribute = false;
        
        [Order(20)]
        [TitledGroup("Properties")]
        [Dependency("storeToAttribute", true)]
        public Parameter<string> storeAttributeName = new Parameter<string>("sizedelta");
    }
}