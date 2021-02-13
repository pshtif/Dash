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
    public class RetargetNodeBaseModel : NodeModelBase
    {
        [TitledGroup("Retargeting")]
        [Tooltip("Change current target.")]
        public bool retarget = false;
        
        [TitledGroup("Retargeting")]
        [Dependency("retarget", true)]
        [Tooltip("Use scene reference as retarget.")]
        public bool useReference = false;

        [TitledGroup("Retargeting")]
        [Dependency("retarget", true)]
        [Dependency("useReference", false)]
        [Tooltip("Retarget to child of current.")]
        public bool isChild;
        
        [TitledGroup("Retargeting")]
        [Dependency("retarget", true)]
        [Dependency("useReference", false)]
        [Tooltip("Name of a transform to retarget or child if relative, use / for hierachy.")]
        public string target;

        [TitledGroup("Retargeting")]
        [Dependency("retarget", true)]
        [Dependency("useReference", true)]
        [Tooltip("Reference of transform to retarget to.")]
        public ExposedReference<Transform> targetReference;
    }
}