/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System.Collections.Generic;
using Dash.Attributes;

namespace Dash
{
    public class StopNodeModel : NodeModelBase
    {
        public Parameter<bool> continueSelf = new Parameter<bool>(true);

        public Parameter<StopMode> stopMode = new Parameter<StopMode>(StopMode.NONE);
    }

}