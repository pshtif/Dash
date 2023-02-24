/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;

namespace Dash
{
    [Serializable]
    public class AttributeDefinition
    {
        public Parameter<string> name = new Parameter<string>("attribute");
        public Parameter<string> expression = new Parameter<string>("");
        
        public bool specifyType = false;
        public Type type = typeof(int);
    }
}