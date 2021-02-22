/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;

namespace Dash
{
    public class DebugNodeModel : NodeModelBase
    {
        [Dependency("debugFlowData", false)]
        public string text;
        
        [Dependency("debugFlowData", false)]
        public string variable;

        public bool debugFlowData = false;
    }
}