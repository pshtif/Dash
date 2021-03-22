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
    public class AnimateRotationNodeModel : AnimationNodeModelBase
    {
        [Order(21)]
        [TitledGroup("Rotation")]
        public bool useFrom = false;
        
        [Order(22)]
        [Dependency("useFrom", true)]
        [TitledGroup("Rotation")]
        public Parameter<Vector3> fromRotation = new Parameter<Vector3>(Vector3.zero);

        [Order(23)]
        [Dependency("useFrom", true)]
        [TitledGroup("Rotation")]
        public bool isFromRelative = false;
        
        [Order(24)]
        [TitledGroup("Rotation")]
        public Parameter<Vector3> toRotation = new Parameter<Vector3>(Vector3.zero);
        
        [Order(25)]
        [TitledGroup("Rotation")]
        public bool isToRelative = false;
    }
}