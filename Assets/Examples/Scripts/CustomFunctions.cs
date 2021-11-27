/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash;
using NCalc;
using UnityEngine;

namespace Examples.Scripts
{
    public class CustomFunctions
    {
        private static bool Test(FunctionArgs p_args)
        {
            if (p_args.Parameters.Length != 0)
            {
                ExpressionFunctions.errorMessage = "Invalid parameters in Test function.";
                return false;
            }

            p_args.HasResult = true;
            p_args.Result = 0;
            Debug.Log("HERE");
            return true;
        }
    }
}