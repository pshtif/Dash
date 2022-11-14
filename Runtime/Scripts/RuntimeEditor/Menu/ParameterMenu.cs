/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Reflection;
using Dash.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

namespace Dash
{
    public class ParameterMenu
    {
#if UNITY_EDITOR
        public static void Show(Parameter p_parameter, string p_name, object p_object)
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
            
            menu.AddItem(new GUIContent("Promote to Variable"), false, () =>
            {
                p_parameter.isExpression = true;
                var variable = DashEditorCore.EditorConfig.editingGraph.variables.AddNewVariable(type);
                p_parameter.expression = variable.Name;
            });
            
            menu.AddItem(new GUIContent("Expression Editor"), false, () =>
            {
                ExpressionEditorWindow.InitExpressionEditorWindow(p_parameter);
            });

            menu.AddSeparator("");
            
            if (p_parameter.isExpression)
            {
                if (p_parameter.IsDebug())
                {
                    menu.AddItem(new GUIContent("Disable Debug"), false,
                        () => { p_parameter.SetDebug(false); });
                }
                else
                {
                    NodeModelBase model = p_object as NodeModelBase;
                    menu.AddItem(new GUIContent("Enable Debug"), false, () => { p_parameter.SetDebug(true, p_name, model?.id); });
                }
            }

            menu.ShowAsContext();
        }
        
        public static void Show(FieldInfo p_useExpressionField, FieldInfo p_expressionField, FieldInfo p_directField, Object p_object, Type p_type)
        {
            GenericMenu menu = new GenericMenu();

            if ((bool)p_useExpressionField.GetValue(p_object))
            {
                menu.AddItem(new GUIContent("Direct Value"), false, () =>
                {
                    p_expressionField.SetValue(p_object, "");
                    p_useExpressionField.SetValue(p_object, false);
                });   
            }
            else
            {
                menu.AddItem(new GUIContent("Custom Expression"), false, () =>
                {
                    p_directField.SetValue(p_object, p_type.GetDefaultValue());
                    p_useExpressionField.SetValue(p_object, true);
                });
            }
            
            Variable[] variables = DashEditorCore.EditorConfig.editingGraph.variables.GetAllVariablesOfType(p_type);

            foreach (var variable in variables)
            {
                menu.AddItem(new GUIContent("Variable/"+variable.Name), false, () =>
                {
                    p_useExpressionField.SetValue(p_object, true);
                    p_expressionField.SetValue(p_object, variable.Name);
                });
            }
            
            menu.AddItem(new GUIContent("Promote to Variable"), false, () =>
            {
                p_useExpressionField.SetValue(p_object, true);
                var variable = DashEditorCore.EditorConfig.editingGraph.variables.AddNewVariable(p_type);
                p_expressionField.SetValue(p_object, variable.Name);
            });

            menu.ShowAsContext();
        }
#endif
    }
}