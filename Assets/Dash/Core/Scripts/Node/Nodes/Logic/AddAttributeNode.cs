/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System.Reflection;
using Dash.Attributes;
using OdinSerializer.Utilities;
using UnityEngine;

namespace Dash
{
    [Category(NodeCategoryType.LOGIC)]
    [OutputCount(1)]
    [InputCount(1)]
    public class AddAttributeNode : NodeBase<AddAttributeNodeModel>
    {
        override protected void OnExecuteStart(NodeFlowData p_flowData)
        {
            if (!Model.attributeName.IsNullOrWhitespace())
            {
                if (!p_flowData.HasAttribute(Model.attributeName) ||
                    p_flowData.GetAttributeType(Model.attributeName) == Model.attributeType)
                {
                    MethodInfo method = typeof(ExpressionEvaluator).GetMethod("EvaluateExpression", BindingFlags.Public | BindingFlags.Static);
                    MethodInfo generic = method.MakeGenericMethod(Model.attributeType);
                    var value = generic.Invoke(null, new object[] { Model.expression, ParameterResolver, p_flowData });
                    
                    Debug.Log(ExpressionEvaluator.hasErrorInExecution);
                    if (ExpressionEvaluator.hasErrorInExecution)
                    {
                        hasErrorsInExecution = true;
                    }
                    
                    p_flowData.SetAttribute(Model.attributeName, value);
                }
                else
                {
                    Debug.LogWarning("Changing flow data attribute type at runtime not allowed.");
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
                    new Vector2(100, 20)), Model.attributeName, DashEditorCore.Skin.GetStyle("NodeText"));
        }
#endif
    }
}