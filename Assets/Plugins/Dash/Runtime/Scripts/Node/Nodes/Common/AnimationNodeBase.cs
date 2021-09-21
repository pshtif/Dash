/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using Dash.Attributes;
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
        protected override void ExecuteOnTarget(Transform p_target, NodeFlowData p_flowData)
        {
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
                tween.OnComplete(() => ExecuteEnd(p_flowData, tween)).Start();
                ((IInternalGraphAccess)Graph).AddActiveTween(tween);
            }
        }

        protected abstract DashTween AnimateOnTarget(Transform p_target, NodeFlowData p_flowData);
        
        protected void ExecuteEnd(NodeFlowData p_flowData, DashTween p_tween = null)
        {
            if (p_tween != null)
            {
                ((IInternalGraphAccess) Graph).RemoveActiveTween(p_tween);
            }

            OnExecuteEnd();
            OnExecuteOutput(0,p_flowData);
        }
        
        public override bool IsSynchronous()
        {
            return !Model.time.isExpression && Model.time.GetValue(null) == 0;
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
#endif
    }
}