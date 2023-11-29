/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using Dash.Attributes;

namespace Dash
{
    [Tooltip("Creates for loop per item execution.")]
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
            float onIterationDelay = GetParameterValue(Model.OnIterationDelay, p_flowData);
            float onFinishedDelay = GetParameterValue(Model.OnFinishedDelayP, p_flowData);

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

                if (onIterationDelay == 0)
                {
                    OnExecuteOutput(0, data);
                }
                else
                {
                    float time = onIterationDelay * i;
                    DashTween tween = DashTween.To(Graph.Controller, 0, 1, time);
                    
                    _activeTweens.Add(tween);
                    tween.OnComplete(() =>
                    {
                        _activeTweens.Remove(tween);
                        OnExecuteOutput(0, data);
                    });
                    
                    tween.Start();
                }
            }
            
            if (onFinishedDelay == 0 && onIterationDelay == 0)
            {
                EndLoop(p_flowData);
            }
            else
            {
                float time = onFinishedDelay + onIterationDelay * Math.Abs(length);
                DashTween tween = DashTween.To(Graph.Controller, 0, 1, time);
                
                _activeTweens.Add(tween);
                tween.OnComplete(() =>
                {
                    _activeTweens.Remove(tween);
                    EndLoop(p_flowData);
                });
                
                tween.Start();
            }
        }

        protected override void Stop_Internal()
        {
            _activeTweens?.ForEach(t => t.Kill(false));
            _activeTweens = new List<DashTween>();
        }
        
        public override bool IsSynchronous()
        {
            return !Model.OnIterationDelay.isExpression && Model.OnIterationDelay.GetValue(null) == 0 &&
                   !Model.OnFinishedDelayP.isExpression && Model.OnFinishedDelayP.GetValue(null) == 0;
        }
        
        void EndLoop(NodeFlowData p_flowData)
        {
            OnExecuteEnd();
            OnExecuteOutput(1, p_flowData);
        }
    }
}