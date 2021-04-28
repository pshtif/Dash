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
    public class AnimateWithPresetNode : AnimationNodeBase<AnimateWithPresetNodeModel>
    {
        protected override DashTween AnimateOnTarget(Transform p_target, NodeFlowData p_flowData)
        {
            float time = GetParameterValue(Model.time, p_flowData);
            float delay = GetParameterValue(Model.delay, p_flowData);
            EaseType easeType = GetParameterValue(Model.easeType, p_flowData);

            return Model.preset.Execute(p_target, time, delay, easeType);
        }
    }
}
