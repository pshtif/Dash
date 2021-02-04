/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;

namespace Dash
{
    [Serializable]
    public class AddIntVariableNodeModel : NodeModelBase
    {
        public string variableName;
        public Parameter<int> expression = new Parameter<int>(0);
    }
}