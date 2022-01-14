/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System.Reflection;
using Dash.Attributes;
using OdinSerializer.Utilities;
using UnityEngine;

namespace Dash
{
    [Attributes.Tooltip("Sets a custom attribute to NodeFlowData.")]
    [Category(NodeCategoryType.MODIFIER)]
    [OutputCount(1)]
    [InputCount(1)]
    public class SetAttributeNode : NodeBase<SetAttributeNodeModel>
    {
        override protected void OnExecuteStart(NodeFlowData p_flowData)
        {
            string attributeName = GetParameterValue(Model.attributeName, p_flowData);
            
            if (!attributeName.IsNullOrWhitespace() && !Model.expression.IsNullOrWhitespace())
            {
                if (!p_flowData.HasAttribute(attributeName) || (!Model.specifyType || 
                    p_flowData.GetAttributeType(attributeName) == Model.attributeType))
                {
                    var value = ExpressionEvaluator.EvaluateTypedExpression(Model.expression, Model.attributeType,
                        ParameterResolver, p_flowData);
                    
                    if (ExpressionEvaluator.hasErrorInEvaluation)
                    {
                        SetError(ExpressionEvaluator.errorMessage);
                        return;
                    }
                    
                    p_flowData.SetAttribute(attributeName, value);
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

            if (Model.attributeName.isExpression)
            {
                GUI.Label(
                    new Rect(
                        new Vector2(offsetRect.x + offsetRect.width * .5f - 50, offsetRect.y + offsetRect.height / 2),
                        new Vector2(100, 20)), "[EXP]", DashEditorCore.Skin.GetStyle("NodeText"));
            }
            else
            {
                GUI.Label(
                    new Rect(
                        new Vector2(offsetRect.x + offsetRect.width * .5f - 50, offsetRect.y + offsetRect.height / 2),
                        new Vector2(100, 20)), Model.attributeName.GetValue(null), DashEditorCore.Skin.GetStyle("NodeText"));
            }
        }
#endif
    }
}