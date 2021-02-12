/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace Dash
{
    public class OnMouseClickNodeModel : NodeModelBase
    {
        [Tooltip("Use scene reference as retarget.")]
        public bool useReference = false;
        
        [Dependency("retarget", true)]
        [Dependency("useReference", false)]
        [Tooltip("Retarget to child of controller.")]
        public bool isChild;
        
        [Dependency("retarget", true)]
        [Dependency("useReference", false)]
        [Tooltip("Name of a button or child if relative, use / for hierachy.")]
        public string buttonPath;
        
        [Dependency("retarget", true)]
        [Dependency("useReference", true)]
        [Tooltip("Reference of a button.")]
        public ExposedReference<Button> buttonReference;

        public bool retargetToButton = true;
    }
}