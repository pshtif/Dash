/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Dash
{
    public class ParameterMenu
    {
#if UNITY_EDITOR
        public static void Show(Parameter p_parameter)
        {
            GenericMenu menu = new GenericMenu();

            if (p_parameter.isExpression)
            {
                menu.AddItem(new GUIContent("Direct Value"), false, () =>
                {
                    p_parameter.isExpression = false;
                    p_parameter.expression = "";
                });   
            }
            else
            {
                menu.AddItem(new GUIContent("Custom Expression"), false, () =>
                {
                    p_parameter.ClearValue();
                    p_parameter.isExpression = true;
                });
            }

            Type type = p_parameter.GetValueType();
            Variable[] variables = DashEditorCore.EditorConfig.editingGraph.variables.GetAllVariablesOfType(type);

            foreach (var variable in variables)
            {
                menu.AddItem(new GUIContent("Variable/"+variable.Name), false, () =>
                {
                    p_parameter.isExpression = true;
                    p_parameter.expression = variable.Name;
                });
            }

            menu.ShowAsContext();
        }
#endif
    }
}