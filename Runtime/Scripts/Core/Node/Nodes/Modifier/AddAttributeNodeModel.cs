/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;

namespace Dash
{
    [Serializable]
    public class AddAttributeNodeModel : NodeModelBase
    {
        public string attributeName;
        public Type attributeType = typeof(int);
        public string expression;
    }
}