/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    [Help("Changes a current target within NodeFlowData to each child of target and executes on it.")]
    [Category(NodeCategoryType.MODIFIER)]
    [OutputCount(2)]
    [InputCount(1)]
    [Size(200,110)]
    [OutputLabels("OnChild", "OnFinished")]
    public class RetargetToChildrenNode : RetargetNodeBase<RetargetToChildrenNodeModel>
    {
        [NonSerialized] 
        protected List<DashTween> _activeTweens;
        
        protected override void ExecuteOnTarget(Transform p_target, NodeFlowData p_flowData)
        {
            if (_activeTweens == null) _activeTweens = new List<DashTween>();
            
            if (p_target == null)
            {
                ExecuteEnd(p_flowData);
                return;
            }
            
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
                    float time = GetParameterValue(Model.onChildDelay, p_flowData) * i;
                    DashTween tween = DashTween.To(Graph.Controller, 0, 1, time);
                    tween.OnComplete(() =>
                    {
                        _activeTweens.Remove(tween);
                        OnExecuteOutput(0, childData);
                    });
                    _activeTweens.Add(tween);
                    tween.Start();
                }
            }

            if (Model.onFinishDelay == 0 && GetParameterValue(Model.onChildDelay,p_flowData) == 0)
            {
                ExecuteEnd(p_flowData);
            }
            else
            {
                float time = Model.onFinishDelay + GetParameterValue(Model.onChildDelay, p_flowData) * p_target.childCount;
                DashTween tween = DashTween.To(Graph.Controller, 0, 1, time);
                tween.OnComplete(() =>
                {
                    _activeTweens.Remove(tween);
                    ExecuteEnd(p_flowData);
                });
                _activeTweens.Add(tween);
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
            return !Model.onChildDelay.isExpression && Model.onChildDelay.GetValue(null) == 0 &&
                   Model.onFinishDelay == 0;
        }
        
        void ExecuteEnd(NodeFlowData p_flowData)
        {
            OnExecuteEnd();
            OnExecuteOutput(1, p_flowData);
        }
        
        #if UNITY_EDITOR
        internal override Transform ResolveNodeRetarget(Transform p_transform, NodeConnection p_connection)
        {
            if (p_connection.outputIndex == 1)
            {
                return p_transform;
            }
         
            Debug.Log("HTF "+p_transform);
            Transform retarget = base.ResolveNodeRetarget(p_transform, null);
            Debug.Log("WTF "+(retarget == null));
            if (retarget != null && retarget.childCount > 0)
            {
                return retarget.GetChild(0);
            }

            return null;
        }
        #endif
    }
}