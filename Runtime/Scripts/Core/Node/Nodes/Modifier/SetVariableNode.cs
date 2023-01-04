/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;
using OdinSerializer.Utilities;
using UnityEngine;
using Dash.Editor;

namespace Dash
{
    [Attributes.Tooltip("Adds a custom attribute to NodeFlowData.")]
    [Category(NodeCategoryType.MODIFIER)]
    [OutputCount(1)]
    [InputCount(1)]
    public class SetVariableNode : NodeBase<SetVariableNodeModel>
    {
        override protected void OnExecuteStart(NodeFlowData p_flowData)
        {
            if (!Model.variableName.IsNullOrWhitespace() && Model.variableType != null && !Model.expression.IsNullOrWhitespace())
            {
                var value = ExpressionEvaluator.EvaluateTypedExpression(Model.expression, Model.variableType,
                    ParameterResolver, p_flowData);
                    
                if (ExpressionEvaluator.hasErrorInEvaluation)
                {
                    SetError(ExpressionEvaluator.errorMessage);
                    return;
                }

                if (Model.isGlobal)
                {
                    if (DashCore.Instance.GlobalVariables == null)
                    {
                        SetError("Global variables not found!");
                    } else if (DashCore.Instance.GlobalVariables.HasVariable(Model.variableName))
                    {
                        Variable variable = DashCore.Instance.GlobalVariables.GetVariable(Model.variableName);
                        if (variable.GetVariableType() != Model.variableType ||
                            (variable.GetVariableType() != value.GetType() && !variable.GetVariableType()
                                .IsImplicitlyAssignableFrom(value.GetType())))
                        {
                            Debug.Log(variable.GetVariableType().IsImplicitlyAssignableFrom(value.GetType()));
                            SetError("Cannot set existing variable of different type! Expecting " +
                                     variable.GetVariableType() + " got " + Model.variableType);
                            return;
                        }

                        variable.value = value;
                    }
                    else
                    {
                        if (Model.enableCreate)
                        {
                            DashCore.Instance.GlobalVariables.AddVariableByType(Model.variableType, Model.variableName, value);
                        }
                        else
                        {
                            SetError("Variable " + Model.variableName +
                                     " doesn't exist, if you want to create enable it.");
                        }
                    }
                } 
                else
                {
                    if (Graph.variables.HasVariable(Model.variableName))
                    {
                        var variable = Graph.variables.GetVariable(Model.variableName);
                        if (variable.GetVariableType() != Model.variableType ||
                            (variable.GetVariableType() != value.GetType() && !variable.GetVariableType()
                                .IsImplicitlyAssignableFrom(value.GetType())))
                        {
                            Debug.Log(variable.GetVariableType().IsImplicitlyAssignableFrom(value.GetType()));
                            SetError("Cannot set existing variable of different type! Expecting " +
                                     variable.GetVariableType() + " got " + Model.variableType);
                            return;
                        }

                        variable.value = value;
                    }
                    else
                    {
                        if (Model.enableCreate)
                        {
                            Graph.variables.AddVariable(Model.variableName, value);
                        }
                        else
                        {
                            SetError("Variable " + Model.variableName +
                                     " doesn't exist, if you want to create enable it.");
                        }
                    }
                }
            }

            OnExecuteEnd();
            OnExecuteOutput(0, p_flowData);
        }

#if UNITY_EDITOR
        protected override void DrawCustomGUI(Rect p_rect)
        {
            Rect offsetRect = new Rect(rect.x + _graph.viewOffset.x, rect.y + _graph.viewOffset.y, rect.width,
                rect.height);

            GUI.Label(
                new Rect(new Vector2(offsetRect.x + offsetRect.width * .5f - 50, offsetRect.y + offsetRect.height / 2),
                    new Vector2(100, 20)), Model.variableName, DashEditorCore.Skin.GetStyle("NodeText"));
        }
#endif
    }
}