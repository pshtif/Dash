/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using Dash.Attributes;
using DG.Tweening;
using UnityEngine;

namespace Dash
{
    [Serializable]
    public class AnimateScaleNodeModel : AnimationNodeBaseModel
    {
        [Order(31)]
        [TitledGroup("Scale")]
        public bool useFrom = false;
        
        [Order(32)]
        [Dependency("useFrom", true)]
        [TitledGroup("Scale")]
        public Parameter<Vector3> fromScale = new Parameter<Vector3>(Vector3.one);
        
        [Order(33)]
        [Dependency("useFrom", true)]
        [TitledGroup("Scale")]
        public bool isFromRelative = false;
        
        [Order(34)]
        [TitledGroup("Scale")]
        public Parameter<Vector3> toScale = new Parameter<Vector3>(Vector3.one);
        
        [Order(35)]
        [TitledGroup("Scale")]
        public bool isToRelative = false;
    }
}