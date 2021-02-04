/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;
using UnityEngine.UI;

namespace Dash
{
    [Category(NodeCategoryType.EVENTS)]
    [OutputCount(1)]
    public class OnMouseClickNode : NodeBase<OnMouseClickNodeModel>
    {
        protected override void Initialize()
        {
            Button button = Model.buttonReference.Resolve(Controller);
            if (button != null)
            {
                button.onClick.AddListener(() => Execute(NodeFlowDataFactory.Create(button.transform)));
            }
        }

        protected override void OnExecuteStart(NodeFlowData p_flowData)
        {
            OnExecuteEnd();
            OnExecuteOutput(0, p_flowData);
        }
    }
}