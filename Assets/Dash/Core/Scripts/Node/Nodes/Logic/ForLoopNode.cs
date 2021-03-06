/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using Dash.Attributes;
using DG.Tweening;

namespace Dash
{
    [Help("Creates for loop per item execution.")]
    [Category(NodeCategoryType.LOGIC)]
    [OutputCount(2)]
    [InputCount(1)]
    [OutputLabels("OnIteration", "OnFinished")]
    public class ForLoopNode : NodeBase<ForLoopNodeModel>
    {
        protected override void OnExecuteStart(NodeFlowData p_flowData)
        {
            int firstIndex = GetParameterValue(Model.firstIndex, p_flowData);
            int lastIndex = GetParameterValue(Model.lastIndex, p_flowData);
            int length = lastIndex - firstIndex;
            
            if (length == 0)
                EndLoop(p_flowData);

            for (int i = firstIndex; i != lastIndex; i += Math.Abs(length) / length)
            {
                NodeFlowData data = p_flowData.Clone();
                data.SetAttribute(Model.indexVariable, i);
                if (GetParameterValue(Model.OnIterationDelay, p_flowData) == 0)
                {
                    OnExecuteOutput(0, data);
                }
                else
                {
                    Tween call = DOVirtual.DelayedCall(GetParameterValue(Model.OnIterationDelay, p_flowData) * i, () =>
                    {
                        OnExecuteOutput(0, data);
                    });
                    DOPreview.StartPreview(call);
                }
            }
            
            if (Model.OnFinishedDelay == 0 && GetParameterValue(Model.OnIterationDelay, p_flowData) == 0)
            {
                EndLoop(p_flowData);
            }
            else
            {
                Tween call = DOVirtual.DelayedCall(Model.OnFinishedDelay + GetParameterValue(Model.OnIterationDelay, p_flowData) * length, () =>
                {
                    EndLoop(p_flowData);
                });
                DOPreview.StartPreview(call);
            }
        }

        void EndLoop(NodeFlowData p_flowData)
        {
            OnExecuteEnd();
            OnExecuteOutput(1, p_flowData);
        }
    }
}