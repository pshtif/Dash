﻿/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.ComponentModel;
using Dash.Attributes;
using UnityEditor;
using UnityEngine;

namespace Dash
{
    [Help("Executes on custom event callback.")]
    [Attributes.Category(NodeCategoryType.EVENTS)]
    [OutputCount(1)]
    public class OnCustomEventNode : NodeBase<OnCustomEventNodeModel>
    {
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
        public override Vector2 Size => new Vector2(DashEditorCore.Skin.GetStyle("NodeTitle").CalcSize(new GUIContent(Name)).x + 35, 85);
        
        public override string CustomName => "Event " + Model.eventName;

        protected override void DrawCustomGUI(Rect p_rect)
        {
            Rect offsetRect = new Rect(rect.x + _graph.viewOffset.x, rect.y + _graph.viewOffset.y, rect.width, rect.height);
            
            GUI.Label(new Rect(new Vector2(offsetRect.x + offsetRect.width*.5f-50, offsetRect.y+offsetRect.height/2), new Vector2(100, 20)), Model.eventName , DashEditorCore.Skin.GetStyle("NodeText"));
        }
#endif
    }
}
