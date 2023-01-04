/*
 *	Created by:  Peter @sHTiF Stefcek
 */

#if UNITY_EDITOR
using System;
using System.Collections.Generic;

namespace Dash
{
    public class DashEditorDebug
    {
        public static Action OnDebugUpdate;
        
        static public List<DebugItemBase> DebugList { get; private set; }
        
        static public void Debug(DebugItemBase p_debugItem)
        {
            if (DebugList == null)
            {
                DebugList = new List<DebugItemBase>();
            }
            
            DebugList.Add(p_debugItem);
            
            OnDebugUpdate?.Invoke();
        }
    }
}
#endif