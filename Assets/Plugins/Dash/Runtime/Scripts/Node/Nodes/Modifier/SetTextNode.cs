/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Globalization;
using Dash.Attributes;
using TMPro;
using UnityEngine;

namespace Dash
{
    [Help("Sets text on a TextMeshPro component.")]
    [Category(NodeCategoryType.MODIFIER)]
    [InputCount(1)]
    [OutputCount(1)]
    public class SetTextNode : RetargetNodeBase<SetTextNodeModel>
    {
        protected override void ExecuteOnTarget(Transform p_target, NodeFlowData p_flowData)
        {
            TMP_Text tmp = p_target.GetComponent<TMP_Text>();

            if (CheckException(tmp, "Target doesn't contain TMP_Text component"))
                return;

            tmp.text = GetParameterValue(Model.text, p_flowData);
            
            OnExecuteOutput(0, p_flowData);
            OnExecuteEnd();
        }
    }
}