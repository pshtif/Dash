/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;

namespace Dash
{
    public class ForLoopNodeModel : NodeModelBase
    {
        public Parameter<int> firstIndex = new Parameter<int>(0);
        public Parameter<int> lastIndex = new Parameter<int>(0);

        public bool addIndexVariable = false;
        
        [Dependency("addIndexVariable", true)]
        public string indexVariable = "index";
    }
}