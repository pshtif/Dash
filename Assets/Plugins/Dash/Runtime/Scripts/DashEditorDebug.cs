/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dash
{
    public class DashEditorDebug
    {
        public static Action OnDebugUpdate;
        
        static public List<DebugItem> DebugList { get; private set; }
        
        static public void Debug(DebugType p_type, double p_time, DashController p_controller, string p_graphPath, string p_nodeId, Transform p_target, string p_msg)
        {
            if (DebugList == null)
            {
                DebugList = new List<DebugItem>();
            }
            
            DebugList.Add(new DebugItem(p_type, p_time, p_controller, p_graphPath, p_nodeId, p_target, p_msg));
            
            OnDebugUpdate?.Invoke();
        }
    }
}