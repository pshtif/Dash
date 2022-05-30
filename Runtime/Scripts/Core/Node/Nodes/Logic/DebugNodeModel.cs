/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;

namespace Dash
{
    public class DebugNodeModel : NodeModelBase
    {
        public bool debugFlowData = false;
        
        public int test2 { get; set; }

        [Dependency("debugFlowData", false)]
        public Parameter<string> debug;
        
        /*[Dependency("debugFlowData", false)]
        public string text;

        [Dependency("debugFlowData", false)] 
        public string variable;*/
    }
}