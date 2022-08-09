/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System.Collections.Generic;
using UnityEngine;

namespace Dash
{
    public class VariableUtils
    {
        static public List<Variable> copiedVariables = new List<Variable>();
        
        public static void CopyVariables(DashVariables p_fromVariables)
        {
            copiedVariables.Clear();
            foreach (var variable in p_fromVariables)
            {
                copiedVariables.Add(variable);
            }
        }
        
        public static void CopyVariable(Variable p_variable)
        {
            copiedVariables.Clear();
            copiedVariables.Add(p_variable);
        }

        public static void PasteVariables(DashVariables p_toVariables, IVariableBindable p_target)
        {
            copiedVariables.ForEach(v => p_toVariables.PasteVariable(v.Clone(), p_target));
        }
        
        // public static void FetchGlobalVariables()
        // {
        //     var components = GameObject.FindObjectsOfType<DashGlobalVariables>();
        //     if (components.Length > 1)
        //     {
        //         Debug.LogWarning("Multiple global variables found, only first instance used.");
        //     }
        //     
        //     if (components.Length > 0)
        //     {
        //         DashCore.Instance.SetGlobalVariables(components[0]);
        //     }
        //     else
        //     {
        //         DashCore.Instance.SetGlobalVariables(null);
        //     }
        // }
    }
}