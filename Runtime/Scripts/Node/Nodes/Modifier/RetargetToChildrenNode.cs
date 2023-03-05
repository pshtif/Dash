/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    [Attributes.Tooltip("Changes a current target within NodeFlowData to each child of target and executes on it.")]
    [Category(NodeCategoryType.MODIFIER, "Modifier/Retarget")]
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

            var includeInactive = GetParameterValue(Model.targetInactive, p_flowData);
            float childDelay = GetParameterValue(Model.onChildDelay, p_flowData);
            bool inReverse = GetParameterValue(Model.inReverse, p_flowData);
            int offset = 0;
            for (int i = 0; i < p_target.childCount; i++)
            {
                Transform child = p_target.GetChild(inReverse ? p_target.childCount - 1 - i : i);
                
                if (!includeInactive && !child.gameObject.activeSelf)
                {
                    offset++;
                    continue;
                }
                
                NodeFlowData childData = p_flowData.Clone();
                childData.SetAttribute("target", child);

                if (childDelay == 0)
                {
                    OnExecuteOutput(0, childData);
                }
                else
                {
                    float time = childDelay * (i - offset);
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

            float onFinishDelay = GetParameterValue(Model.onFinishDelay, p_flowData);
            if (onFinishDelay == 0 && childDelay == 0)
            {
                ExecuteEnd(p_flowData);
            }
            else
            {
                float time = onFinishDelay + childDelay * (p_target.childCount - offset);
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
            // Should never be null but due to serialization change we need to check for now
            if (Model.onChildDelay == null || Model.onFinishDelay == null)
                return false;
            
            return !Model.onChildDelay.isExpression && Model.onChildDelay.GetValue(null) == 0 &&
                   !Model.onFinishDelay.isExpression && Model.onFinishDelay.GetValue(null) == 0;
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
         
            Transform retarget = base.ResolveNodeRetarget(p_transform, null);
            if (retarget != null && retarget.childCount > 0)
            {
                return retarget.GetChild(0);
            }

            return null;
        }
        #endif
    }
}