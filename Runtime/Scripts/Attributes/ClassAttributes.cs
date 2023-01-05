/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;

namespace Dash.Attributes
{
    public class ClassAttributes
    {
        [AttributeUsage(AttributeTargets.Class)]
        public class ExpressionFunctionsAttribute : Attribute
        {
        }
    }
}