/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Dash.Attributes;

namespace Dash
{
    public class NodeUtils
    {
        static public bool CanHaveMultipleInstances(Type p_nodeType)
        {
            SingleInstanceAttribute attribute = (SingleInstanceAttribute) Attribute.GetCustomAttribute(p_nodeType, typeof(SingleInstanceAttribute));
            return attribute == null;
        }

        static public string CategoryToString(NodeCategoryType p_type)
        {
            return p_type.ToString().Substring(0, 1) + p_type.ToString().Substring(1).ToLower();
        }
    }
}