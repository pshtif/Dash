/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace Dash
{
    [Category(NodeCategoryType.EVENTS)]
    [OutputCount(1)]
    public class OnMouseClickNode : NodeBase<OnMouseClickNodeModel>
    {
        protected override void Initialize()
        {
            Button button;
            if (Model.useReference)
            {
                button = Model.buttonReference.Resolve(Controller);
            }
            else
            {
                if (Model.isChild)
                {
                    var t = Controller.transform.Find(Model.buttonPath);
                    button = t == null ? null : t.GetComponent<Button>();
                }
                else
                {
                    GameObject go = GameObject.Find(Model.buttonPath);
                    button = go == null ? null : go.GetComponent<Button>();
                }
            }

            if (button != null)
            {
                if (Model.retargetToButton)
                {
                    button.onClick.AddListener(() => Execute(NodeFlowDataFactory.Create(button.transform)));
                }
                else
                {
                    button.onClick.AddListener(() => Execute(NodeFlowDataFactory.Create(Controller.transform)));
                }
            }
        }

        protected override void OnExecuteStart(NodeFlowData p_flowData)
        {
            OnExecuteEnd();
            OnExecuteOutput(0, p_flowData);
        }
    }
}