/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using Dash.Attributes;

namespace Dash
{
    [Serializable]
    public class SetAttributeNodeModel : NodeModelBase
    {
        public Parameter<string> attributeName = new Parameter<string>("attribute");
        public string expression;
        
        public bool specifyType = false;
        [Dependency("specifyType", true)]
        public Type attributeType = typeof(int);
    }
}