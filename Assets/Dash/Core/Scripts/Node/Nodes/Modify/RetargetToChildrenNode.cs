/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System.Reflection;
using DG.Tweening;
using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    [Category(NodeCategoryType.MODIFIERS)]
    [OutputCount(2)]
    [InputCount(1)]
    [Size(200,110)]
    [OutputLabels("OnChild", "OnFinished")]
    public class RetargetToChildrenNode : RetargetNodeBase<RetargetToChildrenNodeModel>
    {
        protected override void ExecuteOnTarget(Transform p_target, NodeFlowData p_flowData)
        {
            for (int i = 0; i < p_target.childCount; i++) 
            {
                NodeFlowData childData = p_flowData.Clone();
                childData.SetAttribute("target", p_target.GetChild(Model.inReverse ? p_target.childCount - 1 - i : i));

                if (GetParameterValue(Model.onChildDelay,p_flowData) == 0)
                {
                    OnExecuteOutput(0, childData);
                }
                else
                {
                    Tween call = DOVirtual.DelayedCall(GetParameterValue(Model.onChildDelay, p_flowData) * i, () => OnExecuteOutput(0, childData));
                    DOPreview.StartPreview(call);
                }
            }

            if (Model.onFinishDelay == 0 && GetParameterValue(Model.onChildDelay,p_flowData) == 0)
            {
                ExecuteEnd(p_flowData);
            }
            else
            {
                Tween call = DOVirtual.DelayedCall(Model.onFinishDelay + GetParameterValue(Model.onChildDelay,p_flowData) * p_target.childCount, () => ExecuteEnd(p_flowData));
                DOPreview.StartPreview(call);
            }
        }
            
        void ExecuteEnd(NodeFlowData p_flowData)
        {
            OnExecuteEnd();
            OnExecuteOutput(1, p_flowData);
        }
    }
}