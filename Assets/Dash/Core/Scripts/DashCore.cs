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

        public DashVariables globalVariables => _globalVariables;

        public DashCore()
        {
            FetchGlobalVariables();
        }

        public void FetchGlobalVariables() 
        {
            var components = GameObject.FindObjectsOfType<DashGlobalVariables>();
            if (components.Length > 1)
            {
                Debug.LogWarning("Multiple global variables found, only first instance used.");
            }
            
            if (components.Length > 0)
            {
                _globalVariables = components[0].variables;
            }
            else
            {
                _globalVariables = null;
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