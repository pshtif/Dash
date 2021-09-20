/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using Dash.Attributes;
using UnityEngine;
#if UNITY_EDITOR

#endif

namespace Dash
{
    [Help("Animate RectTransform using an IAnimationPreset implementation. Useful to write custom animation sequences in code for reuse.")]
    [Category(NodeCategoryType.ANIMATION)]
    [OutputCount(1)]
    [InputCount(1)]
    [Size(200,85)]
    [Serializable]
    public class AnimateWithPresetNode : RetargetNodeBase<AnimateWithPresetNodeModel>
    {
        protected override void ExecuteOnTarget(Transform p_target, NodeFlowData p_flowData)
        {
            if (p_target == null)
            {
                ExecuteEnd(p_flowData);
                return;
            }
            
            DashTween tween = Model.preset.Execute(p_target, ParameterResolver, p_flowData);

            if (tween == null)
            {
                ExecuteEnd(p_flowData);
            } else {
                tween.OnComplete(() => ExecuteEnd(p_flowData, tween)).Start();
                ((IInternalGraphAccess)Graph).AddActiveTween(tween);
            }
        }
        
        protected void ExecuteEnd(NodeFlowData p_flowData, DashTween p_tween = null)
        {
            if (p_tween != null)
            {
                ((IInternalGraphAccess) Graph).RemoveActiveTween(p_tween);
            }

            OnExecuteEnd();
            OnExecuteOutput(0,p_flowData);
        }
    }
}
