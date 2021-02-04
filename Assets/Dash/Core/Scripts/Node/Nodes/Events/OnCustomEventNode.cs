/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.ComponentModel;
using Dash.Attributes;
using UnityEditor;
using UnityEngine;

namespace Dash
{
    [Attributes.Category(NodeCategoryType.EVENTS)]
    [OutputCount(1)]
    public class OnCustomEventNode : NodeBase<OnCustomEventNodeModel>
    {
        #if UNITY_EDITOR
        public override Vector2 Size => new Vector2(10+CustomName.Length * 10, 85);
        #endif
        
        protected override void Initialize()
        {
            _graph.AddListener(Model.eventName, this);
        }

        override protected void OnExecuteStart(NodeFlowData p_flowData)
        {
            OnExecuteEnd();
            OnExecuteOutput(0, p_flowData);
        }

#if UNITY_EDITOR
        public override string CustomName => "Event " + Model.eventName;

        protected override void DrawCustomGUI(Rect p_rect)
        {
            Rect offsetRect = new Rect(rect.x + _graph.viewOffset.x, rect.y + _graph.viewOffset.y, rect.width, rect.height);
            
            GUI.Label(new Rect(new Vector2(offsetRect.x + offsetRect.width*.5f-50, offsetRect.y+offsetRect.height/2), new Vector2(100, 20)), Model.eventName , DashEditorCore.Skin.GetStyle("NodeText"));
        }
        
        public override bool Invalidate()
        {
            //skinWidth = customNodeName.Length * 10;

            return true;
        }
#endif
    }
}
