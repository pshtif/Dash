/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;

namespace Dash
{
    public class SetTextNodeModel : RetargetNodeModel
    {
        //[TitledGroup("Properties", 2)]
        public Parameter<string> text = new Parameter<string>("");
    }
}