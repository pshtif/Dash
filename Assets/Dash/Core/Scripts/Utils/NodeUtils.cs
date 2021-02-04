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
            SettingsAttribute attribute = (SettingsAttribute) Attribute.GetCustomAttribute(p_nodeType, typeof(SettingsAttribute));
            return attribute == null ? true : attribute.canHaveMultiple;
        }
    }
}