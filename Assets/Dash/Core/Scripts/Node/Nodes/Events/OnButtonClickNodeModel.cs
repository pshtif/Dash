/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace Dash
{
    public class OnButtonClickNodeModel : NodeModelBase
    {
        [Tooltip("Use scene reference as retarget.")]
        public bool useReference = false;
        
        [Dependency("retarget", true)]
        [Dependency("useReference", false)]
        [Tooltip("Retarget to child of controller.")]
        public bool isChild;
        
        [Dependency("useReference", false)]
        [Tooltip("Lookup for name match instead of path.")]
        public bool useFind;
        
        [Dependency("useReference", false)]
        [Dependency("useFind", true)]
        [Tooltip("Look up for all instances.")]
        public bool findAll;
        
        [Dependency("retarget", true)]
        [Dependency("useReference", false)]
        [Tooltip("Name of a button or child if relative, use / for hierachy.")]
        public string button;
        
        [Dependency("retarget", true)]
        [Dependency("useReference", true)]
        [Tooltip("Reference of a button.")]
        public ExposedReference<Button> buttonReference;

        public bool retargetToButton = true;
    }
}