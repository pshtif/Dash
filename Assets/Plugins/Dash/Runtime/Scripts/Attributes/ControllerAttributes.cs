/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;

namespace Dash.Attributes
{
    // [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    // public class BindDashButton : BindDashElement
    // {
    //     public bool autoClickCallback { get; }
    //     
    //     public BindDashButton(bool p_autoClickCallback = false, bool p_overwrite = true, string p_customName = "") : base(p_overwrite, p_customName)
    //     {
    //         autoClickCallback = p_autoClickCallback;
    //     }
    // }
    
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class BindDashVariable : Attribute
    {
        public bool overwrite { get; } 
        public string customName { get; }

        public BindDashVariable(bool p_overwrite = true, string p_customName = "")
        {
            overwrite = p_overwrite;
            customName = p_customName;
        }
    }
}