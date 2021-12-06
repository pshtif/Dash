/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using UnityEngine;

namespace Dash
{
    public interface IAnimationPreset
    {
        DashTween Execute(Transform p_target, IParameterResolver p_resolver, NodeFlowData p_flowData);
    }
}