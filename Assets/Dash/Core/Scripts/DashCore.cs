/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

[assembly: Preserve]

namespace Dash
{
    public class DashCore
    {
        private static DashCore _instance = null;

        public static DashCore Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DashCore();
                }
                
                return _instance;
            }
        }

        private List<DashController> _controllers = new List<DashController>();

        [NonSerialized]
        private DashVariables _globalVariables;
        
        public DashVariables variables
        {
            get
            {
                if (_globalVariables == null)
                {
                    var component = GameObject.FindObjectOfType<DashGlobalVariables>();
                    if (component != null)
                    {
                        _globalVariables = component.variables;
                    }
                }

                return _globalVariables;
            }
        }

        public void Bind(DashController p_controller)
        {
            _controllers.Add(p_controller);
        }

        public void SendEvent(string p_name)
        {
            _controllers.ForEach(g => g.SendEvent(p_name));
        }
        
        public void SendEvent(string p_name, NodeFlowData p_flowData)
        {
            //Debug.Log("DashCore.SendEvent: "+p_name);
            _controllers.ForEach(g => g.SendEvent(p_name, p_flowData));
        }
    }
}