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
    public class CreateAttributeNode : NodeBase<CreateAttributeNodeModel>
    {
        override protected void OnExecuteStart(NodeFlowData p_flowData)
        {
            if (Model.attributes != null)
            {
                foreach (var attribute in Model.attributes)
                {
                    p_flowData.SetAttribute(GetParameterValue(attribute.name, p_flowData),
                        GetUntypedParameterValue(attribute.attributeValue, p_flowData));
                    // string attributeName = GetParameterValue(attribute.name, p_flowData);
                    // if (!p_flowData.HasAttribute(attributeName) ||
                    //     !attribute.specifyType ||
                    //     p_flowData.GetAttributeType(attributeName) == attribute.attributeType ||
                    //     DashCore.Instance.Config.allowAttributeTypeChange) 
                    // {
                    //     var expression = GetParameterValue(attribute.expression, p_flowData);
                    //     object value;
                    //     if (attribute.specifyType)
                    //     {
                    //         value = ExpressionEvaluator.EvaluateTypedExpression(expression, attribute.attributeType,
                    //             ParameterResolver, p_flowData);
                    //     }
                    //     else
                    //     {
                    //         value = ExpressionEvaluator.EvaluateUntypedExpression(expression, ParameterResolver, false,
                    //             p_flowData);
                    //     }
                    //
                    //     if (ExpressionEvaluator.hasErrorInEvaluation)
                    //     {
                    //         Debug.LogError(ExpressionEvaluator.errorMessage);
                    //     }
                    //
                    //     p_flowData.SetAttribute(attributeName, value);
                    // }
                    // else
                    // {
                    //     Debug.LogWarning("Changing flow data attribute type at runtime not allowed for attribute: "+attributeName);
                    // }
                }
            }

            OnExecuteEnd();
            OnExecuteOutput(0, p_flowData);
        }
    }
}