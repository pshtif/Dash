/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System.Collections.Generic;
using Dash.Attributes;

namespace Dash
{
    public class OnCustomEventNodeModel : NodeModelBase
    {
        public const string defaultEventName = "CustomEvent";

        public string eventName = defaultEventName;

        public int priority = 0;

        public bool useSequencer = false;

        [Dependency("useSequencer", true)]
        public Parameter<string> sequencerId = new Parameter<string>("");

        [Dependency("useSequencer", true)]
        public Parameter<int> sequencerPriority = new Parameter<int>(0);
    }

}