/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;
using OdinSerializer.Utilities;
using UnityEngine;

namespace Dash
{
    [Category(NodeCategoryType.LOGIC)]
    [OutputCount(1)]
    [InputCount(1)]
    public class AddIntVariableNode : NodeBase<AddIntVariableNodeModel>
    {
        override protected void OnExecuteStart(NodeFlowData p_flowData)
        {
            if (!Model.variableName.IsNullOrWhitespace())
            {
                if (!p_flowData.HasAttribute(Model.variableName) ||
                    p_flowData.GetAttributeType(Model.variableName) == typeof(int))
                {
                    p_flowData.SetAttribute(Model.variableName,
                        Model.expression.GetValue(ParameterResolver, p_flowData));
                }
                else
                {
                    Debug.LogWarning("Changing flow data variable type at runtime not allowed.");
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