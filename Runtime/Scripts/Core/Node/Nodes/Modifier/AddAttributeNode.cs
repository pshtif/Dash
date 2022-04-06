/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Linq;
using System.Reflection;
using Dash.Attributes;
using OdinSerializer.Utilities;
using UnityEngine;

namespace Dash
{
    [Obsolete]
    [Attributes.Tooltip("Adds a custom attribute to NodeFlowData.")]
    [Category(NodeCategoryType.MODIFIER)]
    [OutputCount(1)]
    [InputCount(1)]
    public class AddAttributeNode : NodeBase<AddAttributeNodeModel>
        #if UNITY_EDITOR
        , INodeMigratable
        #endif
    {
        override protected void OnExecuteStart(NodeFlowData p_flowData)
        {
            if (!Model.attributeName.IsNullOrWhitespace() && !Model.expression.IsNullOrWhitespace())
            {
                if (!p_flowData.HasAttribute(Model.attributeName) ||
                    p_flowData.GetAttributeType(Model.attributeName) == Model.attributeType)
                {
                    var value = ExpressionEvaluator.EvaluateTypedExpression(Model.expression, Model.attributeType,
                        ParameterResolver, p_flowData);
                    
                    if (ExpressionEvaluator.hasErrorInEvaluation)
                    {
                        SetError(ExpressionEvaluator.errorMessage);
                        return;
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
        public void Migrate()
        {
            bool selected = SelectionManager.IsSelected(this);
            
            SetAttributeNode newNode = NodeUtils.CreateNode(DashEditorCore.EditorConfig.editingGraph, typeof(SetAttributeNode), rect.position) as SetAttributeNode;
            
            newNode.Model.attributeName.SetValue(Model.attributeName);
            newNode.Model.expression = Model.expression;
            newNode.Model.specifyType = true;
            newNode.Model.attributeType = Model.attributeType;

            Graph.Connections.FindAll(c => c.inputNode == this).ToArray().ForEach(c =>
            {
                NodeConnection nc = new NodeConnection(c.inputIndex, newNode, c.outputIndex, c.outputNode);
                Graph.Connections.Add(nc);
                Graph.Connections.Remove(c);
            });
            
            Graph.Connections.FindAll(c => c.outputNode == this).ToArray().ForEach(c =>
            {
                NodeConnection nc = new NodeConnection(c.inputIndex, c.inputNode, c.outputIndex, newNode);
                Graph.Connections.Add(nc);
                Graph.Connections.Remove(c);
            });
            
            Graph.DeleteNode(this);

            if (selected)
            {
                SelectionManager.SelectNode(newNode, Graph);
            }
        }

        public Type GetMigrateType()
        {
            return typeof(SetAttributeNode);
        }

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