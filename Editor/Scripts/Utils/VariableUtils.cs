/*
 *	Created by:  Peter @sHTiF Stefcek
 */

#if UNITY_EDITOR

using System.Collections.Generic;

namespace Dash.Editor
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
    }
}
#endif