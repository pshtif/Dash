/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEngine;

namespace Dash
{
    public class ParameterUtils
    {
        public static void MigrateParameter<T>(ref Parameter<T> p_parameterFrom, ref Parameter<T> p_parameterTo)
        {
            if (p_parameterTo == null)
            {
                p_parameterTo = new Parameter<T>(default(T));
                
                if (p_parameterFrom != null)
                {
                    if (p_parameterFrom.isExpression)
                    {
                        p_parameterTo.expression = p_parameterFrom.expression;
                        p_parameterTo.isExpression = true;
                    }
                    else
                    {
                        p_parameterTo.SetValue(p_parameterFrom.GetValue(null));
                        p_parameterTo.isExpression = false;
                    }
                }
            }
        } 
    }
}