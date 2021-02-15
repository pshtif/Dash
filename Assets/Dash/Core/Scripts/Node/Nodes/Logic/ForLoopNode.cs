/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using Dash.Attributes;

namespace Dash
{
    [Category(NodeCategoryType.LOGIC)]
    [OutputCount(2)]
    [InputCount(1)]
    [OutputLabels("OnIteration", "OnFinished")]
    public class ForLoopNode : NodeBase<ForLoopNodeModel>
    {
        protected override void OnExecuteStart(NodeFlowData p_flowData)
        {
            int firstIndex = Model.firstIndex.GetValue(ParameterResolver, p_flowData);
            int lastIndex = Model.lastIndex.GetValue(ParameterResolver, p_flowData);
            int length = lastIndex - firstIndex;
            
            if (length == 0)
                EndLoop(p_flowData);

            for (int i = firstIndex; i != lastIndex; i += Math.Abs(length) / length)
            {
                NodeFlowData data = p_flowData.Clone();
                data.SetAttribute(Model.indexVariable, i);
                if (Model.OnIterationDelay.GetValue(ParameterResolver) == 0)
                {
                    OnExecuteOutput(0, data);
                }
                else
                {
                    DOPreview.DelayedCall(Model.OnIterationDelay.GetValue(ParameterResolver) * i, () =>
                    {
                        OnExecuteOutput(0, data);
                    });
                }
            }
            
            if (Model.OnFinishedDelay == 0 && Model.OnIterationDelay.GetValue(ParameterResolver) == 0)
            {
                EndLoop(p_flowData);
            }
            else
            {
                DOPreview.DelayedCall(Model.OnFinishedDelay + Model.OnIterationDelay.GetValue(ParameterResolver) * length, () =>
                {
                    EndLoop(p_flowData);
                });
            }
        }

        void EndLoop(NodeFlowData p_flowData)
        {
            OnExecuteEnd();
            OnExecuteOutput(1, p_flowData);
        }
    }
}