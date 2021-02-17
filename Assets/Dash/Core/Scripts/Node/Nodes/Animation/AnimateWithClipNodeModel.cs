/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using Dash.Attributes;
using DG.Tweening;
using UnityEngine;
#if UNITY_EDITOR

#endif

namespace Dash
{
    [Serializable]
    public class AnimateWithClipNodeModel : RetargetNodeBaseModel
    {
        [TitledGroup("Animation")]
        [Order(1)]
        [Tooltip("Use time from animation clip.")]
        public bool useAnimationTime = true;
        
        [TitledGroup("Animation")]
        [Order(2)]
        [Dependency("useAnimationTime", false)]
        public float time = 1;
        
        [TitledGroup("Animation")]
        [Order(3)]
        public float delay = 0;

        [TitledGroup("Animation")]
        [Order(4)]
        public Ease easing = Ease.Linear;
        
        [TitledGroup("Animation")] 
        [Order(5)]
        public bool isReverse = false;
        
        [TitledGroup("Animation")]
        [Order(6)]
        [Dependency("useDashAnimation", true)]
        public bool isRelative = true;

        [TitledGroup("Source")] 
        public bool useDashAnimation = false;
        
        [TitledGroup("Source")]
        [Dependency("useDashAnimation", true)]
        public DashAnimation source;

        [TitledGroup("Source")]
        [Dependency("useDashAnimation", false)]
        public AnimationClip clip;
    }
}