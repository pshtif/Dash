/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using Dash.Attributes;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Dash
{
    [Category(NodeCategoryType.HIDDEN)]
    [OutputCount(1)]
    [InputCount(1)]
    [Size(160,85)]
    [InspectorHeight(380)]
    public abstract class AnimationNodeBase<T> : RetargetNodeBase<T> where T:AnimationNodeModelBase, new()
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
            
            DashTween tween = AnimateOnTarget(p_target, p_flowData);

            if (tween == null)
            {
                ExecuteEnd(p_flowData);
            } else {
                _activeTweens.Add(tween);
                tween.OnComplete(() =>
                {
                    ExecuteEnd(p_flowData, tween);
                }).Start();
            }
        }

        protected abstract DashTween AnimateOnTarget(Transform p_target, NodeFlowData p_flowData);
        
        protected void ExecuteEnd(NodeFlowData p_flowData, DashTween p_tween = null)
        {
            if (p_tween != null)
            {
                _activeTweens.Remove(p_tween);
            }

            OnExecuteEnd();
            OnExecuteOutput(0,p_flowData);
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
            base.DrawCustomGUI(p_rect);

            string text = Model.time.isExpression ? "Time: [Exp]" : "Time: " + Model.time.GetValue(null) + "s";
            text = Model.delay.isExpression ? text + "   Delay: [Exp]" :
                Model.delay.GetValue(null) > 0 ? text + "   Delay: " + Model.delay.GetValue(null) + "s" : text;

            GUI.Label(new Rect(p_rect.x + p_rect.width / 2 - 50, p_rect.y + p_rect.height - 32, 100, 20),
                text, DashEditorCore.Skin.GetStyle("NodeText"));
        }

        public override void SelectEditorTarget()
        {
            Transform target = ResolveEditorTarget();
            
            if (target != null)
            {
                Selection.activeGameObject = target.gameObject;
                Tools.current = Tool.Rect;
            }
        }
#endif
    }
}