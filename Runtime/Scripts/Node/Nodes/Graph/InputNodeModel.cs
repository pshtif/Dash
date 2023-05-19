/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using Dash.Attributes;

namespace Dash
{
    public class InputNodeModel : NodeModelBase
    {
        [DisallowWhitespace]
        public string inputName = "Input";
    }
}