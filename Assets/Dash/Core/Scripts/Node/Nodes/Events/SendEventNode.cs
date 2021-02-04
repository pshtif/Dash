/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    [Category(NodeCategoryType.EVENTS)]
    [OutputCount(1)]
    [InputCount(1)]
    public class SendEventNode : NodeBase<SendEventNodeModel>
    {
        override protected void OnExecuteStart(NodeFlowData p_flowData)
        {
            _graph.SendEvent(Model.eventName, p_flowData);
            
            OnExecuteEnd();
            OnExecuteOutput(0, p_flowData);
        }

        #region EDITOR_CODE
#if UNITY_EDITOR
        protected override void DrawCustomGUI(Rect p_rect)
        {
            Rect offsetRect = new Rect(rect.x + _graph.viewOffset.x, rect.y + _graph.viewOffset.y, rect.width, rect.height);

            GUI.Label(
                new Rect(new Vector2(offsetRect.x + offsetRect.width * .5f - 50, offsetRect.y + offsetRect.height / 2),
                    new Vector2(100, 20)), Model.eventName, DashEditorCore.Skin.GetStyle("NodeText"));
        }
#endif
        #endregion
    }
}