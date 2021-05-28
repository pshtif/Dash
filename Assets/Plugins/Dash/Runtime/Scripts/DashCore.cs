/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Scripting;

[assembly: Preserve]

namespace Dash
{
    public class DashCore
    {
        public const string VERSION = "0.4.6RC2";
        
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

        public DashController GetControllerByName(string p_name)
        {
            #if UNITY_EDITOR
            if (Application.isPlaying)
            {
                return _controllers.Find(dc => dc.gameObject.name == p_name);
            }
            else
            {
                var found = GameObject.FindObjectsOfType<DashController>().ToList()
                    .FindAll(dc => dc.gameObject.name == p_name);
                if (found.Count == 0)
                    return null;
                
                if (found.Count > 1)
                    Debug.LogWarning("Using GetControllerByName with multiple DashControllers with the same name: "+p_name);

                return found[0];
            }
            #else
            return _controllers.Find(dc => dc.gameObject.name == p_name);
            #endif
        }
        
//         public DashController GetControllerById(string p_id)
//         {
// #if UNITY_EDITOR
//             if (Application.isPlaying)
//             {
//                 return _controllers.Find(dc => dc.Id == p_id);
//             }
//             else
//             {
//                 var found = GameObject.FindObjectsOfType<DashController>(); 
//                 return found.Length== 0 ? null : found.ToList().Find(dc => dc.Id == p_id);
//             }
// #else
//             return _controllers.Find(dc => dc.Id == p_id);
// #endif
//         }

        public void Bind(DashController p_controller)
        {
            _controllers.Add(p_controller);
        }
        
        public void Unbind(DashController p_controller)
        {
            _controllers.Remove(p_controller);
        }

        public void SendEvent(string p_name, NodeFlowData p_flowData)
        {
            //Debug.Log("DashCore.SendEvent: "+p_name);
            _controllers.ForEach(dc => dc?.SendEvent(p_name, p_flowData));
            
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
        
        public static int GetVersionNumber() 
        {
            var split = VERSION.Split('.');
            int result = 0;
            for (int i = 0; i < split.Length; i++)
            {
                string number = string.Concat(split[i].TakeWhile(char.IsNumber));
                result += Int32.Parse(number) * (int) Mathf.Pow(1000, split.Length - i - 1);
            }

            return result;
        }
        
        public static string GetVersionString(int p_number)
        {
            string result = "";
            int number = p_number;
            while (number > 0)
            {
                result = "." + (number % 1000) + result;
                number /= 1000;
            }
            
            result = p_number <= 1000000 ? "0" + result : result.Substring(1);

            return result;
        }
    }
}