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

        [NonSerialized]
        private List<DashController> _controllers = new List<DashController>();

        [NonSerialized]
        private Dictionary<string, List<Action<NodeFlowData>>> _listeners =
            new Dictionary<string, List<Action<NodeFlowData>>>();

        [NonSerialized]
        private DashVariables _globalVariables;

        public DashVariables globalVariables => _globalVariables;

        public void SetGlobalVariables(DashVariables p_variables)
        {
            _globalVariables = p_variables;
        }

        public void Bind(DashController p_controller)
        {
            _controllers.Add(p_controller);
        }

        public void SendEvent(string p_name, NodeFlowData p_flowData)
        {
            //Debug.Log("DashCore.SendEvent: "+p_name);
            _controllers.ForEach(dc => dc.SendEvent(p_name, p_flowData));
            
            if (_listeners.ContainsKey(p_name))
            {
                _listeners[p_name].ForEach(c => c.Invoke(p_flowData));
            }
        }

        public void AddListener(string p_name, Action<NodeFlowData> p_callback)
        {
            if (!string.IsNullOrWhiteSpace(p_name))
            {
                if (!_listeners.ContainsKey(p_name))
                    _listeners[p_name] = new List<Action<NodeFlowData>>();

                if (!_listeners[p_name].Contains(p_callback))
                    _listeners[p_name].Add(p_callback);
            }
            else
            {
                Debug.LogWarning("Invalid event name, cannot be null or whitespace.");
            }
        }
        
        public void RemoveListener(string p_name, Action<NodeFlowData> p_callback)
        {
            if (_listeners.ContainsKey(p_name))
            {
                _listeners[p_name].Remove(p_callback);
                if (_listeners[p_name].Count == 0)
                    _listeners.Remove(p_name);
            }
        }
    }
}