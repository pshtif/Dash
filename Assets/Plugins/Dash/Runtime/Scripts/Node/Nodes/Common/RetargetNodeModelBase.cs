/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using Dash.Attributes;
using JetBrains.Annotations;
using UnityEngine;

#if UNITY_EDITOR

#endif

namespace Dash
{
    [Serializable]
    public class RetargetNodeModelBase : NodeModelBase
    {
        [Order(0)]
        [TitledGroup("Retargeting", -1)]
        [Tooltip("Change current target.")]
        public bool retarget = false;

        [Order(1)]
        [TitledGroup("Retargeting", -1)]
        [Dependency("retarget", true)]
        [Tooltip("Use scene reference as retarget.")]
        public bool useReference = false;
        
        [Order(2)]
        [TitledGroup("Retargeting", -1)]
        [Dependency("retarget", true)]
        [Dependency("useReference", false)]
        [Tooltip("Retarget to child of current.")]
        public bool isChild = true;
        
        [Order(3)]
        [TitledGroup("Retargeting", -1)]
        [Dependency("retarget", true)]
        [Dependency("useReference", false)]
        [Tooltip("Name of a transform to retarget or child if relative, use / for hierachy.")]
        [CanBeNull]
        public Parameter<string> target = new Parameter<string>("");

        [Order(4)]
        [TitledGroup("Retargeting", -1)]
        [Expression("useExpression", "targetExpression")]
        [Dependency("retarget", true)]
        [Dependency("useReference", true)]
        [Tooltip("Reference of transform to retarget to.")]
        public ExposedReference<Transform> targetReference;
        
        [TitledGroup("Retargeting", -1)]
        [HideInInspector]
        public bool useExpression = false;
        
        [TitledGroup("Retargeting", -1)]
        [HideInInspector]
        public string targetExpression = "";
    }
}