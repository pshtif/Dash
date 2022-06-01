/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.ComponentModel;
using System.Linq;
using Dash.Attributes;
using UnityEditor;
using UnityEngine;

namespace Dash
{
    [Attributes.Category(NodeCategoryType.GRAPH)]
    [OutputCount(1)]
    [Size(85,85)]
    public class InputNode : NodeBase<InputNodeModel>
    {
        // #if UNITY_EDITOR
        // protected override Color NodeBackgroundColor => new Color(0.8f, 0.6f, 0f);
        // protected override Color TitleBackgroundColor => new Color(0.8f, 0.5f, 0f);
        //
        // protected override Color TitleTextColor => new Color(1f, 0.8f, 0);
        // #endif

        protected override void OnExecuteStart(NodeFlowData p_flowData)
        {
            OnExecuteEnd();
            OnExecuteOutput(0, p_flowData);
        }
        
        protected override void Invalidate()
        {
            base.Invalidate();
            ValidateUniqueInputName();
        }
        
        protected void ValidateUniqueInputName()
        {
            string name = Model.inputName;
            if (string.IsNullOrEmpty(name))
            {
                name = "Input1";
            }

            while (Graph.Nodes.FindAll(n => n.GetType() == typeof(InputNode)).Select(n => (InputNode)n).ToList().Exists(n => n != this && n.Model.inputName == name))
            {
                string number = string.Concat(name.Reverse().TakeWhile(char.IsNumber).Reverse());
                name = name.Substring(0,name.Length-number.Length) + (string.IsNullOrEmpty(number) ? 1 : (Int32.Parse(number)+1));
            }

            Model.inputName = name;
        }

#if UNITY_EDITOR
        public override Vector2 Size => new Vector2(DashEditorCore.Skin.GetStyle("NodeTitle").CalcSize(new GUIContent(Name)).x + 35, 85);
        public override string CustomName => "Input " + Model.inputName;

        protected override void DrawCustomGUI(Rect p_rect)
        {
            Rect offsetRect = new Rect(rect.x + _graph.viewOffset.x, rect.y + _graph.viewOffset.y, rect.width, rect.height);
            
            GUI.Label(new Rect(new Vector2(offsetRect.x + offsetRect.width*.5f-50, offsetRect.y+offsetRect.height/2), new Vector2(100, 20)), Model.inputName , DashEditorCore.Skin.GetStyle("NodeText"));
        }
       
        
        public override NodeBase Clone(DashGraph p_graph)
        {
            InputNode node = base.Clone(p_graph) as InputNode;
            node.ValidateUniqueInputName();
            return node;
        }
#endif
    }
}