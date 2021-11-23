/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    [Category(NodeCategoryType.LOGIC)]
    [OutputCount(1)]
    [InputCount(1)]
    public class DelayNode : NodeBase<DelayNodeModel>
    {
        [NonSerialized]
        protected List<DashTween> _activeTweens;
        
        override protected void OnExecuteStart(NodeFlowData p_flowData)
        {
            if (_activeTweens == null) _activeTweens = new List<DashTween>();
            
            float time = GetParameterValue(Model.time, p_flowData);

            if (time == 0)
            {
                OnExecuteEnd();
                OnExecuteOutput(0, p_flowData);
            }
            else
            {
                DashTween tween = DashTween.To(Graph.Controller, 0, 1, time);
                
                tween.OnComplete(() =>
                {
                    OnExecuteEnd();
                    OnExecuteOutput(0, p_flowData);
                    _activeTweens.Remove(tween);
                });
                
                _activeTweens.Add(tween);
                tween.Start();
            }
        }
        
        public override bool IsSynchronous()
        {
            return !Model.time.isExpression && Model.time.GetValue(null) == 0;
        }

        protected override void Stop_Internal()
        {
            _activeTweens?.ForEach(t => t.Kill(false));
            _activeTweens = new List<DashTween>();
        }
        
        #if UNITY_EDITOR
        protected override void DrawCustomGUI(Rect p_rect)
        {
            GUI.Label(new Rect(p_rect.x + p_rect.width / 2 - 50, p_rect.y + p_rect.height - 32, 100, 20),
                "Time: " + Model.time.ToString() + "s", DashEditorCore.Skin.GetStyle("NodeText"));
        }
        #endif
    }
}