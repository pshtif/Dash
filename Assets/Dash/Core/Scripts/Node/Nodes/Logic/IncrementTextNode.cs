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
    [Category(NodeCategoryType.LOGIC)]
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
            text = value.ToString();

            if (Model.useDotFormating)
            {
                int i = text.Length;
                while (i > 3)
                {
                    i -= 3;
                    text = text.Insert(i, ".");
                }
            }

            tmp.text = text;
            
            OnExecuteOutput(0, p_flowData);
            OnExecuteEnd();
        }
    }
}