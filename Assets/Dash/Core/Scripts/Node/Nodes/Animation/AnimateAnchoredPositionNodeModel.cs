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
    public class AnimateAnchoredPositionNodeModel : AnimationNodeBaseModel
    {
        [Order(11)]
        [TitledGroup("Properties")]
        public bool useFrom = false;
        
        [Order(12)]
        [Dependency("useFrom", true)]
        [TitledGroup("Properties")]
        public Parameter<Vector2> fromPosition = new Parameter<Vector2>(Vector2.zero);
        
        [Order(13)]
        [Dependency("useFrom", true)]
        [TitledGroup("Properties")]
        public bool isFromRelative = false;
        
        [Order(14)]
        [TitledGroup("Properties")]
        public Parameter<Vector2> toPosition = new Parameter<Vector2>(Vector2.zero);
        
        [Order(15)]
        [TitledGroup("Properties")]
        public bool isToRelative = false;
    }
}