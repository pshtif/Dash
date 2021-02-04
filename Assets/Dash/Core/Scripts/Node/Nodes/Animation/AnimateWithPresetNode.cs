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
    [Category(NodeCategoryType.HIDDEN)]
    [OutputCount(1)]
    [InputCount(1)]
    [Size(200,85)]
    [Serializable]
    public class AnimateWithPresetNode : AnimationNodeBase<AnimateWithPresetNodeModel>
    {
        protected override void ExecuteOnTarget(Transform p_target, NodeFlowData p_flowData)
        {
       
        }
    }
}
