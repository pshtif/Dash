/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    [Attributes.Tooltip("Executes on any mouse click.")]
    [Category(NodeCategoryType.EVENT)]
    [OutputCount(1)]
    public class OnMouseClickNode : NodeBase<OnMouseClickNodeModel>
    {
        protected override void Initialize()
        {
            Controller.RegisterUpdateCallback(() => HandleMouseCheck());
        }

        protected void HandleMouseCheck()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Execute(NodeFlowDataFactory.Create(Controller.transform));
            }
        }

        protected override void OnExecuteStart(NodeFlowData p_flowData)
        {
            Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            RectTransform rect = Controller.GetComponent<RectTransform>();
            Vector2 point;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, mousePos, Camera.main, out point);
            p_flowData.SetAttribute("mousePosition", point);

            OnExecuteOutput(0, p_flowData);
            OnExecuteEnd();
        }
    }
}