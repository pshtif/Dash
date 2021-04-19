/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using DG.Tweening;
using UnityEngine;

namespace Dash
{
    public interface IAnimationPreset
    {
        void Execute(Transform p_transform, float p_duration, float p_delay, Ease p_ease, Action p_onComplete);
    }
}