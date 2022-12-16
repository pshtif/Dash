/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Runtime.Serialization;
using Dash.Attributes;
using JetBrains.Annotations;
using OdinSerializer;
using UnityEngine;
using UnityEngine.Serialization;

#if UNITY_EDITOR

#endif

namespace Dash
{
    [Serializable]
    public class RetargetNodeModelBase : NodeModelBase
    {
        [Order(0)]
        [TitledGroup("Retargeting", -1)]
        [UnityEngine.Tooltip("Change current target.")]
        public bool retarget = false;

        [Order(1)]
        [TitledGroup("Retargeting", -1)]
        [Dependency("retarget", true)]
        [UnityEngine.Tooltip("Use scene reference as retarget.")]
        public bool useReference = true;
        
        [Order(2)]
        [TitledGroup("Retargeting", -1)]
        [Dependency("retarget", true)]
        [Dependency("useReference", false)]
        [UnityEngine.Tooltip("Retarget to child of current.")]
        public bool isChild = true;
        
        [Order(3)]
        [TitledGroup("Retargeting", -1)]
        [Dependency("retarget", true)]
        [Dependency("useReference", false)]
        [UnityEngine.Tooltip("Name of a transform to retarget or child if relative, use / for hierachy.")]
        [HideInInspector]
        public Parameter<string> target = new Parameter<string>("");
        
        [Order(3)]
        [TitledGroup("Retargeting", -1)]
        [Dependency("retarget", true)]
        [Dependency("useReference", false)]
        [UnityEngine.Tooltip("Name of a transform to retarget or child if relative, use / for hierachy.")]
        public Parameter<string> targetName = new Parameter<string>("");

        [Order(4)]
        [TitledGroup("Retargeting", -1)]
        [Expression("useExpression", "targetExpression")]
        [Dependency("retarget", true)]
        [Dependency("useReference", true)]
        [UnityEngine.Tooltip("Reference of transform to retarget to.")]
        public ExposedReference<Transform> targetReference;
        
        [TitledGroup("Retargeting", -1)]
        [HideInInspector]
        public bool useExpression = false;
        
        [TitledGroup("Retargeting", -1)]
        [HideInInspector]
        public string targetExpression = "";
        
        [OnDeserialized]
        protected virtual void OnDeserialized()
        {
#pragma warning disable 612, 618
            
            ParameterUtils.MigrateParameter(ref target, ref targetName);
            
#pragma warning restore 612, 618
        }
    }
}