/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    public class ForLoopNodeModel : NodeModelBase
    {
        [UnityEngine.Tooltip("Delay in execution after each iteration.")]
        public Parameter<float> OnIterationDelay = new Parameter<float>(0);
        [UnityEngine.Tooltip("Delay in execution after last child executed.")]
        public float OnFinishedDelay = 0;
        
        public Parameter<int> firstIndex = new Parameter<int>(0);
        public Parameter<int> lastIndex = new Parameter<int>(0);

        public bool addIndexAttribute = false;
        
        [Dependency("addIndexAttribute", true)]
        public string indexAttribute = "index";
    }
}