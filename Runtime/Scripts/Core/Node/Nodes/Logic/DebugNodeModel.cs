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
        public Parameter<string> debug = new Parameter<string>("");

        public Parameter<bool> outputToUnityConsole = new Parameter<bool>(false);
    }
}