/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.ComponentModel;
using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    [Attributes.Category(NodeCategoryType.EVENTS)]
    [OutputCount(1)]
    [Settings(false, true)]
    [Size(85,85)]
    public class EnterNode : NodeBase<EnterNodeModel>
    {
        #if UNITY_EDITOR
        protected override Color NodeBackgroundColor => new Color(0.8f, 0.6f, 0f);
        protected override Color TitleBackgroundColor => new Color(0.8f, 0.5f, 0f);

        protected override Color TitleTextColor => new Color(1f, 0.8f, 0);
        #endif

        protected override void OnExecuteStart(NodeFlowData p_flowData)
        {
            OnExecuteEnd();
            OnExecuteOutput(0, p_flowData);
        }

        // protected override void UpdateCustomGUI(Event p_event, Rect p_rect)
        // {
        //     Rect offsetRect = new Rect(nodeRect.x + _graph.viewOffset.x, nodeRect.y + _graph.viewOffset.y, skinWidth, skinHeight);
        //     var color = IsExecuting ? DashEditorCore.instance.NODE_EXECUTING_COLOR :
        //         IsSelected ? DashEditorCore.instance.NODE_SELECTED_COLOR : defaultBgColor;
        //     GUI.color = color;
        //     GUI.Box(offsetRect, "", DashEditorCore.config.skin.GetStyle(skinId));
        //     
        //     GUI.color = new Color(0,0,0,.25f);
        //     GUI.DrawTexture(new Rect(offsetRect.x+3, offsetRect.y+3, skinWidth, skinHeight), iconTexture);
        //     GUI.color = color;
        //     GUI.DrawTexture(new Rect(offsetRect.x, offsetRect.y, skinWidth, skinHeight), iconTexture);
        //     
        //     GUI.color = Color.white;
        //     if (String.IsNullOrEmpty(_model.id))
        //     {
        //         GUI.Label(
        //             new Rect(new Vector2(offsetRect.x, offsetRect.y - 20), new Vector2(skinWidth - 5, 20)),
        //             _model.id, DashEditorCore.config.skin.GetStyle("NodeId"));
        //     }
        // }
    }
}