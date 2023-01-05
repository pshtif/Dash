/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;

namespace Dash
{
    [Serializable]
    public class BranchNodeModel : NodeModelBase
    {
        public Parameter<bool> expression = new Parameter<bool>(false);
    }
}