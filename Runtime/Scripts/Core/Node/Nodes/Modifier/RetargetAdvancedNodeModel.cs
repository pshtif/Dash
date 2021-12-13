/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    public class RetargetAdvancedNodeModel : NodeModelBase
    {
        [UnityEngine.Tooltip("Retarget to child of current.")]
        public bool isChild;

        [UnityEngine.Tooltip("Look up for all instances.")]
        public bool findAll = false;
        
        public string target;
        
        [UnityEngine.Tooltip("Delay in execution after each target.")]
        [Dependency("findAll", true)]
        public Parameter<float> delay = new Parameter<float>(0);

        [UnityEngine.Tooltip("Iterates targets in reverse order.")]
        [Dependency("findAll", true)]
        public bool inReverse = false;
    }
}