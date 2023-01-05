/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Linq;
using Dash.Attributes;
using Dash.Editor;
using UnityEngine;

namespace Dash
{
    [Category(NodeCategoryType.GRAPH)]
    [InputCount(1)]
    [Size(85,85)]
    public class OutputNode : NodeBase<OutputNodeModel>
    {
        protected override void OnExecuteStart(NodeFlowData p_flowData)
        {
            OnExecuteEnd();
            Graph.OutputExecuted(this, p_flowData);
        }
        
        protected override void Invalidate()
        {
            base.Invalidate();
            ValidateUniqueOutputName();
        }
        
        protected void ValidateUniqueOutputName()
        {
            string name = Model.outputName;
            if (string.IsNullOrEmpty(name))
            {
                name = "Output1";
            }

            while (Graph.Nodes.FindAll(n => n.GetType() == typeof(OutputNode)).Select(n => (OutputNode)n).ToList().Exists(n => n != this && n.Model.outputName == name))
            {
                string number = string.Concat(name.Reverse().TakeWhile(char.IsNumber).Reverse());
                name = name.Substring(0,name.Length-number.Length) + (string.IsNullOrEmpty(number) ? 1 : (Int32.Parse(number)+1));
            }

            Model.outputName = name;
        }

#if UNITY_EDITOR
        public override Vector2 Size => new Vector2(DashEditorCore.Skin.GetStyle("NodeTitle").CalcSize(new GUIContent(Name)).x + 35, 85);
        public override string CustomName => "Output " + Model.outputName;

        protected override void DrawCustomGUI(Rect p_rect)
        {
            Rect offsetRect = new Rect(rect.x + _graph.viewOffset.x, rect.y + _graph.viewOffset.y, rect.width, rect.height);
            
            GUI.Label(new Rect(new Vector2(offsetRect.x + offsetRect.width*.5f-50, offsetRect.y+offsetRect.height/2), new Vector2(100, 20)), Model.outputName , DashEditorCore.Skin.GetStyle("NodeText"));
        }

        public override NodeBase Clone(DashGraph p_graph)
        {
            OutputNode node = base.Clone(p_graph) as OutputNode;
            node.ValidateUniqueOutputName();
            return node;
        }
#endif
    }
}