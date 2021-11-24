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
        internal override Transform ResolveEditorTarget(string p_path = "", int p_outputIndex = 0)
        {
            // If we don't have controller no point in resolving
            if (DashEditorCore.EditorConfig.editingController == null)
                return null;

            var connections = Graph.GetInputConnections(this);
            if (p_outputIndex == 1)
            {
                return connections[0].outputNode
                    .ResolveEditorTarget(p_path, connections[0].outputIndex);
            }
            
            object target = ResolveRetargetedEditorTarget(p_path, p_outputIndex, false);
            
            if (target is string)
            {
                if (connections.Count > 0)
                {
                    return connections[0].outputNode
                        .ResolveEditorTarget(target + "/{0}/" + p_path, connections[0].outputIndex);
                }
                
                return DashEditorCore.EditorConfig.editingController.transform.ResolvePathWithFind(
                    target + "/{0}/" + p_path);
            }

            if (target != null)
            {
                return (target as Transform).GetChild(0).ResolvePathWithFind(p_path);
            }

            return null;
        }
        #endif
    }
}