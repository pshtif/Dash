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
    public class AnimateToRectNodeModel : AnimationNodeModelBase
    {
        [Order(10)] 
        [TitledGroup("Target")] 
        public ExposedReference<Transform> toTarget;
        
        [Order(11)]
        [TitledGroup("Target")]
        public bool useToPosition = false;
        
        [Order(12)]
        [TitledGroup("Target")]
        public bool useToRotation = false;
        
        [Order(13)]
        [TitledGroup("Target")]
        public bool useToScale = false;
    }
}