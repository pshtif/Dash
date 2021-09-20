/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;

namespace Dash
{
    public class DestroyNodeModel : RetargetNodeModelBase
    {
        public bool usePooling = false;
        
        [Dependency("usePooling", false)]
        public bool immediate = false;
        
        [Dependency("usePooling", true)]
        public Parameter<string> poolId = new Parameter<string>("pool");
    }
}