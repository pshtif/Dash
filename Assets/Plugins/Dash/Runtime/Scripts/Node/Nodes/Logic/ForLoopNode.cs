/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
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
        [NonSerialized] 
        protected List<DashTween> _activeTweens;
        
        protected override void OnExecuteStart(NodeFlowData p_flowData)
        {
            if (_activeTweens == null) _activeTweens = new List<DashTween>();

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
                        _activeTweens.Remove(tween);
                    });
                    
                    _activeTweens.Add(tween);
                    tween.Start();
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
                    _activeTweens.Remove(tween);
                });
                
                _activeTweens.Add(tween);
                tween.Start();
            }
        }

        public override bool IsSynchronous()
        {
            return !Model.OnIterationDelay.isExpression && Model.OnIterationDelay.GetValue(null) == 0 &&
                   Model.OnFinishedDelay == 0;
        }
        
        void EndLoop(NodeFlowData p_flowData)
        {
            OnExecuteEnd();
            OnExecuteOutput(1, p_flowData);
        }
    }
}