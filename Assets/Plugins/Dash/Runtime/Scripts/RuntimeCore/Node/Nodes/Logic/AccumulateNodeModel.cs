/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Accumulate;
using Dash.Attributes;

namespace Dash
{
    public class AccumulateNodeModel : NodeModelBase
    {
        public AccumulateType accumulationType = AccumulateType.ALL;

        [DependencySingle("accumulationType", AccumulateType.ANY_COUNT)]
        [DependencySingle("accumulationType", AccumulateType.UNIQUE_COUNT)]
        public Parameter<int> accumulationCount = new Parameter<int>(1);

        public Parameter<bool> repeatAccumulation = new Parameter<bool>(false);
    }
}