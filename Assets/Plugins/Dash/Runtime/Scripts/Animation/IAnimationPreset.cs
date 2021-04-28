/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using UnityEngine;

namespace Dash
{
    public interface IAnimationPreset
    {
        DashTween Execute(Transform p_transform, float p_duration, float p_delay, EaseType p_easeType);
    }
}