/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Dash
{
    public class ReservedVariables
    {
        // Using MethodInfo directly without compiling actions using Expression trees as it is slow and problematic on IL2CPP
        private static Dictionary<string, MethodInfo> _methodCache;
        
        private static DashGraph _graph;
        
        private static Vector2 mousePosition
        {
            get { return new Vector2(Input.mousePosition.x, Input.mousePosition.y); }
        }

        private static Transform controller
        {
            get { return _graph.Controller.transform; }
        }
        
        private static string[] names = { "controller", "mousePosition" };

        public static bool IsGlobalVariable(string p_name)
        {
            return names.Contains(p_name);
        }
        
        public static bool Resolve(DashGraph p_graph, string p_name, out object p_result)
        {
            // Set resolving graph
            _graph = p_graph;

            if (_methodCache == null) _methodCache = new Dictionary<string, MethodInfo>();

            // If we have cached getter lets use it
            if (_methodCache.ContainsKey(p_name))
            {
                p_result = _methodCache[p_name].Invoke(null, null);
                return true;
            }

            PropertyInfo property = typeof(ReservedVariables).GetProperty(p_name, BindingFlags.NonPublic | BindingFlags.Static);
            
            if (property == null)
            {
                p_result = null;
                return false;
            }
            
            MethodInfo getter = property.GetGetMethod(true);

            if (getter != null)
            {
                // Cache it
                _methodCache.Add(p_name, getter);
                // Execute it
                p_result = getter.Invoke(null, null);
                return true;
            }
            
            p_result = null;
            return false;
        }
    }
}