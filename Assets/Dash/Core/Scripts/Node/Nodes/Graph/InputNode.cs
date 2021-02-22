/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.ComponentModel;
using Dash.Attributes;
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

#if UNITY_EDITOR
        // protected override Color NodeBackgroundColor => new Color(0.8f, 0.6f, 0f);
        // protected override Color TitleBackgroundColor => new Color(0.8f, 0.5f, 0f);
        // protected override Color TitleTextColor => new Color(1f, 0.8f, 0);
        public override Vector2 Size => new Vector2(DashEditorCore.Skin.GetStyle("NodeTitle").CalcSize(new GUIContent(Name)).x + 35, 85);
        public override string CustomName => "Input " + Model.inputName;

        protected override void DrawCustomGUI(Rect p_rect)
        {
            Rect offsetRect = new Rect(rect.x + _graph.viewOffset.x, rect.y + _graph.viewOffset.y, rect.width, rect.height);
            
            GUI.Label(new Rect(new Vector2(offsetRect.x + offsetRect.width*.5f-50, offsetRect.y+offsetRect.height/2), new Vector2(100, 20)), Model.inputName , DashEditorCore.Skin.GetStyle("NodeText"));
        }
#endif
    }
}