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
        public const string VERSION = "0.6.4";
        
        public DashRuntimeConfig Config { get; private set; }
        
        private static DashCore _instance = null;

        public static DashCore Instance
        {
            get
            {
                if (_instance == null)
                {
#if UNITY_EDITOR
                    DashEditorDebug.Debug(new CoreDebugItem(CoreDebugItem.CoreDebugItemType.INITIALIZE));
#endif
                    _instance = new DashCore();
                }
                
                return _instance;
            }
        }

        [NonSerialized]
        private List<DashController> _controllers = new List<DashController>();

        [NonSerialized] 
        private Dictionary<string,EventSequencer> _sequencers = new Dictionary<string, EventSequencer>();
        
        [NonSerialized]
        private Dictionary<string, PrefabPool> _prefabPools = new Dictionary<string, PrefabPool>();

        public EventSequencer GetOrCreateSequencer(string p_id)
        {
            if (!_sequencers.ContainsKey(p_id))
            {
                var sequencer = new EventSequencer(p_id);
                _sequencers.Add(p_id, sequencer);
            }

            return _sequencers[p_id];
        }

        public void CleanSequencers()
        {
            _sequencers = new Dictionary<string, EventSequencer>();
        }

        public void CleanPrefabPools()
        {
            foreach (var pair in _prefabPools)
            {
                _prefabPools[pair.Key].Clean();
            }

            _prefabPools = new Dictionary<string, PrefabPool>();
        }
        
        public PrefabPool GetOrCreatePrefabPool(string p_id, Transform p_prefab)
        {
            if (!_prefabPools.ContainsKey(p_id))
            {
                _prefabPools.Add(p_id, new PrefabPool(p_prefab));
            }
            
            return _prefabPools[p_id]; 
        }
        
        public PrefabPool GetPrefabPool(string p_id)
        {
            return _prefabPools.ContainsKey(p_id) ? _prefabPools[p_id] : null;
        }

        [NonSerialized]
        private Dictionary<string, List<EventHandler>> _listeners =
            new Dictionary<string, List<EventHandler>>();

        [NonSerialized]
        private DashGlobalVariables _globalVariables;
        
        public DashGlobalVariables globalVariables => _globalVariables;

        public void SetGlobalVariables(DashGlobalVariables p_globalVariables)
        {
            _globalVariables = p_globalVariables;
        }

        public DashCore()
        {
            LoadConfig();
            //Debug.Log("Config Loaded: "+Config);
        }

        void LoadConfig()
        {
            Config = Resources.Load<DashRuntimeConfig>("DashRuntimeConfig");
        }

        public DashController GetControllerByName(string p_name)
        {
            #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                var found = GameObject.FindObjectsOfType<DashController>().ToList()
                    .FindAll(dc => dc.gameObject.name == p_name);
                if (found.Count == 0)
                    return null;
                
                if (found.Count > 1)
                    Debug.LogWarning("Using GetControllerByName with multiple DashControllers with the same name: "+p_name);

                return found[0];
            }
            #endif
            
            return _controllers.Find(dc => dc.gameObject.name == p_name);
        }

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
            _controllers.ToList().ForEach(dc => dc?.SendEvent(p_name, p_flowData));
            
            if (_listeners.ContainsKey(p_name))
            {
                _listeners[p_name].ToList().ForEach(c => c.Invoke(p_flowData));
            }
        }

        public void AddListener(string p_name, Action<NodeFlowData> p_callback, int p_priority = 0)
        {
            if (!string.IsNullOrWhiteSpace(p_name))
            {
                if (!_listeners.ContainsKey(p_name))
                    _listeners[p_name] = new List<EventHandler>();

                if (!_listeners[p_name].Exists(e => e.Callback == p_callback))
                {
                    _listeners[p_name].Add(new EventHandler(p_callback, p_priority));
                    _listeners[p_name] = _listeners[p_name].OrderBy(e => e.Priority).ToList();
                }
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
                _listeners[p_name].RemoveAll(e => e.Callback == p_callback);
                
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