/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using Dash.Attributes;

namespace Dash
{
    public class OutputNodeModel : NodeModelBase
    {
        [DisallowWhitespace]
        public string outputName = "Output";
    }
}