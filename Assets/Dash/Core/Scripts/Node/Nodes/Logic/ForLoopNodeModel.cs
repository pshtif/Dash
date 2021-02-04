/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;

namespace Dash
{
    public class ForLoopNodeModel : NodeModelBase
    {
        public int firstIndex = 0;
        public int lastIndex = 0;

        public bool addIndexVariable = false;
        [Dependency("addIndexVariable", true)]
        public string indexVariable = "index";
    }
}