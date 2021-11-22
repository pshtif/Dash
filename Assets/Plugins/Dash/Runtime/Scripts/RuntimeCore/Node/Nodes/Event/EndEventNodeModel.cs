/*
 *	Created by:  Peter @sHTiF Stefcek
 */

namespace Dash
{
    public class EndEventNodeModel : NodeModelBase
    {
        public Parameter<string> eventName = new Parameter<string>("");
        public Parameter<string> sequencerId = new Parameter<string>("");
    }
}