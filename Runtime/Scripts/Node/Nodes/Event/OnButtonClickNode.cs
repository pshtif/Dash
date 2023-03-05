/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System.Collections.Generic;
using Dash.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace Dash
{
    [Attributes.Tooltip("Executes on button mouse click.")]
    [Category(NodeCategoryType.EVENT)]
    [OutputCount(1)]
    public class OnButtonClickNode : NodeBase<OnButtonClickNodeModel>
    {
        internal override void Initialize()
        {
            List<Button> buttons = new List<Button>();
            Button button;
            Transform transform;
            if (Model.useReference)
            {
                button = Model.buttonReference.Resolve(Controller);
                if (button != null) buttons.Add(button);
            }
            else
            {
                if (Model.isChild)
                {
                    if (Model.useFind)
                    {
                        if (Model.findAll)
                        {
                            List<Transform> transforms = Controller.transform.DeepFindAll(Model.button);
                            foreach (Transform t in transforms)
                            {
                                button = t.GetComponent<Button>();
                                if (button != null) buttons.Add(button);
                            }
                        }
                        else
                        {
                            transform = Controller.transform.DeepFind(Model.button);
                            button = transform == null ? null : transform.GetComponent<Button>();
                            if (button != null) buttons.Add(button);
                        }
                    }
                    else
                    {
                        transform = Controller.transform.Find(Model.button);
                        button = transform == null ? null : transform.GetComponent<Button>();
                        if (button != null) buttons.Add(button);
                    }
                }
                else
                {
                    if (Model.useFind)
                    {
                        if (Model.findAll)
                        {
                            List<Transform> transforms = Controller.transform.root.DeepFindAll(Model.button);
                            foreach (Transform t in transforms)
                            {
                                button = t.GetComponent<Button>();
                                if (button != null) buttons.Add(button);
                            }
                        }
                        else
                        {
                            transform = Controller.transform.root.DeepFind(Model.button);
                            button = transform == null ? null : transform.GetComponent<Button>();
                            if (button != null) buttons.Add(button);
                        }
                    }
                    else
                    {
                        GameObject go = GameObject.Find(Model.button);
                        button = go == null ? null : go.GetComponent<Button>();
                        if (button != null) buttons.Add(button);
                    }
                }
            }

            foreach (Button b in buttons)
            {
                if (Model.retargetToButton)
                {
                    b.onClick.AddListener(() => Execute(NodeFlowDataFactory.Create(b.transform)));
                }
                else
                {
                    b.onClick.AddListener(() => Execute(NodeFlowDataFactory.Create(Controller.transform)));
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