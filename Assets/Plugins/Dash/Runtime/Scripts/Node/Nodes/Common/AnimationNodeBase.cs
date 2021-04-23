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
            DashTween tween = AnimateOnTarget(p_target, p_flowData);

            if (tween == null)
            {
                ExecuteEnd(p_flowData);
            } else {
                tween.OnComplete(() => ExecuteEnd(p_flowData, tween)).Start();
                //((IInternalGraphAccess)Graph).AddActiveTween(p_target, tween);
            }
        }

        protected abstract DashTween AnimateOnTarget(Transform p_target, NodeFlowData p_flowData);
        
        protected void ExecuteEnd(NodeFlowData p_flowData, DashTween p_tween = null)
        {
            // if (p_tween != null)
            // {
            //     ((IInternalGraphAccess) Graph).RemoveActiveTween(p_tween);
            // }

            OnExecuteEnd();
            OnExecuteOutput(0,p_flowData);
        }

#if UNITY_EDITOR
        protected override void DrawCustomGUI(Rect p_rect)
        {
            base.DrawCustomGUI(p_rect);
            
            if (Model.time.isExpression)
            {
                GUI.Label(new Rect(p_rect.x + p_rect.width / 2 - 50, p_rect.y + p_rect.height - 32, 100, 20),
                    "Time: [Exp]", DashEditorCore.Skin.GetStyle("NodeText"));
            }
            else
            {
                GUI.Label(new Rect(p_rect.x + p_rect.width / 2 - 50, p_rect.y + p_rect.height - 32, 100, 20),
                    "Time: " + Model.time.GetValue(null) + "s", DashEditorCore.Skin.GetStyle("NodeText"));
            }
        }
#endif
    }
}