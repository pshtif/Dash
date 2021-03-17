/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEngine;

namespace Dash
{
    public class RetargetAdvancedNodeModel : NodeModelBase
    {
        [Tooltip("Retarget to child of current.")]
        public bool isChild;

        [Tooltip("Look up for all instances.")]
        public bool findAll = false;
        
        [Tooltip("Name of a target or child if relative, use / for hierachy.")]
        public string target;
        
        [Tooltip("Delay in execution after each target.")]
        public Parameter<float> delay = new Parameter<float>(0);

        [Tooltip("Iterates targets in reverse order.")]
        public bool inReverse = false;
    }
}