/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;

namespace Dash
{
    [Serializable]
    public class SetVariableNodeModel : NodeModelBase
    {
        public string variableName;
        public Type variableType = typeof(int);
        public string expression;
        public bool enableCreate = false;
        public bool isGlobal = false;
    }
}