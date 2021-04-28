/*
 *	Created by:  Peter @sHTiF Stefcek
 */

namespace Dash
{
    public class SendCustomEventNodeModel : NodeModelBase
    {
        public Parameter<string> eventName = new Parameter<string>("");

        public Parameter<bool> global = new Parameter<bool>(false);

        public Parameter<bool> sendData = new Parameter<bool>(true);
    }
}