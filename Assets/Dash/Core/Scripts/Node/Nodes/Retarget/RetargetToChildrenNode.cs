/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System.Reflection;
using DG.Tweening;
using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    [Category(NodeCategoryType.RETARGET)]
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

                if (Model.onChildDelay.GetValue(ParameterResolver) == 0)
                {
                    OnExecuteOutput(0, childData);
                }
                else
                {
                    DOPreview.DelayedCall(Model.onChildDelay.GetValue(ParameterResolver) * i, () =>
                    {
                        OnExecuteOutput(0, childData);
                    });
                }
            }

            if (Model.onFinishDelay == 0 && Model.onChildDelay.GetValue(ParameterResolver) == 0)
            {
                ExecuteEnd(p_flowData);
            }
            else
            {
                DOPreview.DelayedCall(Model.onFinishDelay + Model.onChildDelay.GetValue(ParameterResolver) * p_target.childCount, () =>
                {
                    ExecuteEnd(p_flowData);
                });
            }
        }
            
        void ExecuteEnd(NodeFlowData p_flowData)
        {
            OnExecuteEnd();
            OnExecuteOutput(1, p_flowData);
        }
    }
}