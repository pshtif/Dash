/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using Dash.Attributes;

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

                if (Model.addIndexAttribute)
                {
                    data.SetAttribute(Model.indexAttribute, i);
                }

                if (GetParameterValue(Model.OnIterationDelay, p_flowData) == 0)
                {
                    OnExecuteOutput(0, data);
                }
                else
                {
                    float time = GetParameterValue(Model.OnIterationDelay, p_flowData) * i;
                    DashTween tween = DashTween.To(Graph.Controller, 0, 1, time);
                    tween.OnComplete(() =>
                    {
                        OnExecuteOutput(0, data);
                    });
                    tween.Start();
                    ((IInternalGraphAccess)Graph).AddActiveTween(tween);
                }
            }
            
            if (Model.OnFinishedDelay == 0 && GetParameterValue(Model.OnIterationDelay, p_flowData) == 0)
            {
                EndLoop(p_flowData);
            }
            else
            {
                float time = Model.OnFinishedDelay + GetParameterValue(Model.OnIterationDelay, p_flowData) * length;
                DashTween tween = DashTween.To(Graph.Controller, 0, 1, time);
                tween.OnComplete(() =>
                {
                    EndLoop(p_flowData);
                });
                tween.Start();
                ((IInternalGraphAccess)Graph).AddActiveTween(tween);
            }
        }

        void EndLoop(NodeFlowData p_flowData)
        {
            OnExecuteEnd();
            OnExecuteOutput(1, p_flowData);
        }
    }
}