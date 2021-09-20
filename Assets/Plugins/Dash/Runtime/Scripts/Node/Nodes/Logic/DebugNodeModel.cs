/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;

namespace Dash
{
    public class DebugNodeModel : NodeModelBase
    {
        public bool debugFlowData = false;

        [Dependency("debugFlowData", false)]
        public Parameter<string> debugText;
        
        [Dependency("debugFlowData", false)]
        public string text;

        [Dependency("debugFlowData", false)] 
        public string variable;
    }
}