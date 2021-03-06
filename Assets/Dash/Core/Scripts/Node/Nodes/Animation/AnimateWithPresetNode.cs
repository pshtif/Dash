﻿/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Xml;
using DG.Tweening;
using Dash.Attributes;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
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
        protected override void ExecuteOnTarget(Transform p_target, NodeFlowData p_flowData)
        {
            float time = GetParameterValue(Model.time);
            float delay = GetParameterValue(Model.delay);
            Model.preset.Execute(p_target, time, delay, Model.easing, () =>
            {
                OnExecuteEnd();
                OnExecuteOutput(0, p_flowData);
            });
        }
    }
}
