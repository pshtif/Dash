/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    [Help("Send a custom event.")]
    [Category(NodeCategoryType.EVENT)]
    [InputCount(1)]
    [OutputCount(1)]
    [Size(170,85)]
    public class SendCustomEventNode : NodeBase<SendCustomEventNodeModel>
    {
        override protected void OnExecuteStart(NodeFlowData p_flowData)
        {
            string eventName = GetParameterValue(Model.eventName, p_flowData);
            bool global = GetParameterValue(Model.global, p_flowData);
            bool sendData = GetParameterValue(Model.sendData, p_flowData);

            if (global)
            {
                DashCore.Instance.SendEvent(eventName, sendData ? p_flowData : NodeFlowDataFactory.Create());
            }
            else
            {
                _graph.SendEvent(eventName, sendData ? p_flowData : NodeFlowDataFactory.Create());
            }

            OnExecuteEnd();
            OnExecuteOutput(0, p_flowData);
        }

        #region EDITOR_CODE
#if UNITY_EDITOR
        protected override void DrawCustomGUI(Rect p_rect)
        {
            Rect offsetRect = new Rect(rect.x + _graph.viewOffset.x, rect.y + _graph.viewOffset.y, rect.width, rect.height);

            // Need to do this check for older versions
            if (Model.eventName == null)
                return;
            
            if (!Model.eventName.isExpression)
            {
                GUI.Label(
                    new Rect(
                        new Vector2(offsetRect.x + offsetRect.width * .5f - 50, offsetRect.y + offsetRect.height / 2),
                        new Vector2(100, 20)), Model.eventName.GetValue(null), DashEditorCore.Skin.GetStyle("NodeText"));
            }
            else
            {
                GUI.Label(
                    new Rect(
                        new Vector2(offsetRect.x + offsetRect.width * .5f - 50, offsetRect.y + offsetRect.height / 2),
                        new Vector2(100, 20)), "[EXP]", DashEditorCore.Skin.GetStyle("NodeText"));
            }
        }
#endif
        #endregion
    }
}