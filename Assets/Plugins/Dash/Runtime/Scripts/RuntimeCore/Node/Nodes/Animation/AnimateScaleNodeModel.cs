/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    [Serializable]
    public class AnimateScaleNodeModel : AnimationNodeModelBase
    {
        [Order(11)]
        [TitledGroup("Properties")]
        public bool useFrom = false;
        
        [Order(12)]
        [Dependency("useFrom", true)]
        [TitledGroup("Properties")]
        public Parameter<Vector3> fromScale = new Parameter<Vector3>(Vector3.one);
        
        [Order(13)]
        [Dependency("useFrom", true)]
        [TitledGroup("Properties")]
        public bool isFromRelative = false;
        
        [Order(14)]
        [TitledGroup("Properties")]
        public Parameter<Vector3> toScale = new Parameter<Vector3>(Vector3.one);
        
        [Order(15)]
        [TitledGroup("Properties")]
        public bool isToRelative = false;
        
        [Order(19)]
        [TitledGroup("Properties")]
        public bool storeToAttribute = false;
        
        [Order(20)]
        [TitledGroup("Properties")]
        [Dependency("storeToAttribute", true)]
        public Parameter<string> storeAttributeName = new Parameter<string>("scale");
    }
}