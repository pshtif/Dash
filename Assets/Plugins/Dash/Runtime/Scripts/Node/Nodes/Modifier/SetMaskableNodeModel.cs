/*
 *	Created by:  Peter @sHTiF Stefcek
 */

namespace Dash
{
    public class SetMaskableNodeModel : RetargetNodeModelBase
    {
        public Parameter<bool> maskable = new Parameter<bool>(true);

        public Parameter<bool> wholeHierarchy = new Parameter<bool>(false);
    }
}