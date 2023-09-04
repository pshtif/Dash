/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;
using UnityEngine;
using System.Runtime.Serialization;

namespace Dash
{
    public class ForLoopNodeModel : NodeModelBase
    {
        [UnityEngine.Tooltip("Delay in execution after each iteration.")]
        public Parameter<float> OnIterationDelay = new Parameter<float>(0);
        
        [UnityEngine.Tooltip("Delay in execution after last child executed.")]
        [HideInInspector]
        public float OnFinishedDelay = 0;
        
        [Label("On Finished Delay")]
        [UnityEngine.Tooltip("Delay in execution after last child executed.")]
        public Parameter<float> OnFinishedDelayP = new Parameter<float>(0);
        
        //public Parameter<float>

        public Parameter<int> firstIndex = new Parameter<int>(0);
        public Parameter<int> lastIndex = new Parameter<int>(0);

        public bool addIndexAttribute = false;
        
        [Dependency("addIndexAttribute", true)]
        public string indexAttribute = "index";
        
        [OnDeserialized]
        void OnDeserialized()
        {
#pragma warning disable 612, 618
            
            ParameterUtils.MigrateParameter(ref OnFinishedDelay, ref OnFinishedDelayP);

#pragma warning restore 612, 618 
        }
    }
}