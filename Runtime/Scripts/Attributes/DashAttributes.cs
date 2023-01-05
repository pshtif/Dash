/*
 *	Created by:  Peter @sHTiF Stefcek
 */
#if UNITY_EDITOR

using System;

namespace Dash.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DashEditorOnlyAttribute : Attribute
    {
    }
}
#endif