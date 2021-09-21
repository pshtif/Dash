/*
 *	Created by:  Peter @sHTiF Stefcek
 */

namespace Dash
{
    public class OnCustomEventNodeModel : NodeModelBase
    {
        public const string defaultEventName = "CustomEvent";

        public string eventName = defaultEventName;

        public int priority = 0;

        public Parameter<string> sequencerId = new Parameter<string>("");
    }

}