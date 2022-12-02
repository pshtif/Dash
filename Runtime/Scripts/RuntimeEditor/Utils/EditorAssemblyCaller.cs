/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;

namespace Dash
{
    public class EditorAssemblyCaller
    {
        public static void Call(string p_className, string p_methodName, [CanBeNull] object[] p_parameters)
        {
            // Debug.Log("Call: "+p_className+", "+p_methodName+", "+p_parameters.Length);
            
            Assembly editorAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .First(a => a.FullName.StartsWith("DashEditor"));

            Type utilityType = editorAssembly.GetTypes()
                .FirstOrDefault(t => t.FullName.Contains(p_className));
            
            utilityType.GetMethod(p_methodName,
                    BindingFlags.Public | BindingFlags.Static)
                .Invoke(obj: null, parameters: p_parameters);
        }
    }
}