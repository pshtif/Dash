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
        [Dependency("useAnimationTime", false)]
        public float time = 1;
        [TitledGroup("Animation")]
        public float delay = 0;

        [TitledGroup("Animation")]
        public Ease easing = Ease.Linear;
        
        [TitledGroup("Animation")]
        public AnimationClip sourceClip;

        [TitledGroup("Animation")]
        public bool isRelative = true;

        [TitledGroup("Animation")]
        [Tooltip("Use time from animation clip.")]
        public bool useAnimationTime = true;

        [TitledGroup("Animation")] 
        public bool isReverse = false;
    }
}