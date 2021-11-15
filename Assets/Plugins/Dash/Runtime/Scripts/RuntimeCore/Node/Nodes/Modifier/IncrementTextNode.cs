/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Globalization;
using Dash.Attributes;
using Dash.Runtime;
using TMPro;
using UnityEngine;

namespace Dash
{
    [Help("Tries to increment a number inside a TextMeshPro text.")]
    [Category(NodeCategoryType.MODIFIER)]
    [InputCount(1)]
    [OutputCount(1)]
    public class IncrementTextNode : RetargetNodeBase<IncrementTextNodeModel>
    {
        protected override void ExecuteOnTarget(Transform p_target, NodeFlowData p_flowData)
        {
            TMP_Text tmp = p_target.GetComponent<TMP_Text>();

            if (CheckException(tmp, "Target doesn't contain TMP_Text component"))
                return;

            string text = tmp.text;
            if (Model.useDotFormating)
            {
                text = text.Replace(".", "");
            }

            int value;
            if (!Int32.TryParse(text, out value))
            {
                Debug.LogWarning("Not a valid Int value in target text");
                return;
            }

            value += Model.increment;
            tmp.text = Model.useDotFormating ? StringUtils.GetDotFormat(value) : value.ToString();
            
            OnExecuteOutput(0, p_flowData);
            OnExecuteEnd();
        }
    }
}