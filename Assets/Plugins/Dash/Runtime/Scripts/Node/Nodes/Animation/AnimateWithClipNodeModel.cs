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
    public class AnimateWithClipNodeModel : AnimationNodeModelBase
    {
        [TitledGroup("Animation",2)]
        [Order(1)]
        [Tooltip("Use time from animation clip.")]
        public bool useAnimationTime = true;

        [TitledGroup("Animation",2)] 
        [Order(5)]
        public bool isReverse = false;
        
        [TitledGroup("Animation",2)]
        [Order(6)]
        [Dependency("useDashAnimation", true)]
        public bool isRelative = true;

        [TitledGroup("Source",1)] 
        public bool useDashAnimation = false;
        
        [TitledGroup("Source",1)]
        [Dependency("useDashAnimation", true)]
        public DashAnimation source;

        [TitledGroup("Source",1)]
        [Dependency("useDashAnimation", false)]
        public AnimationClip clip;
    }
}