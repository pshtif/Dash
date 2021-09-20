/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dash
{
    public class ShakePreset : IAnimationPreset
    {
        public int shakeCount = 5;
        public float shakeStrength = 1;
        public bool fade = false;

        public DashTween Execute(Transform p_transform, IParameterResolver p_resolver, NodeFlowData p_flowData)
        {
            // RectTransform rect = p_transform as RectTransform;
            //
            // Vector2 startPosition = rect.anchoredPosition;
            //
            // Sequence sequence = DOTween.Sequence();
            //
            // for (int i = 0; i < shakeCount; i++)
            // {
            //     float f = fade ? ((shakeCount - i) / (float)shakeCount) : 1;
            //     Tween tween1 = DOTween
            //         .To(() => rect.anchoredPosition-startPosition, v2 => rect.anchoredPosition = startPosition+v2,
            //             new Vector2(Random.Range(-shakeStrength*f, shakeStrength*f), Random.Range(-shakeStrength*f, shakeStrength*f)),
            //             p_duration / (shakeCount * 2))
            //         .SetEase(Ease.OutQuad);
            //     sequence.Append(tween1);
            //     
            //     Tween tween2 = DOTween
            //         .To(() => rect.anchoredPosition-startPosition, v2 => rect.anchoredPosition = startPosition+v2,
            //             Vector2.zero, p_duration / (shakeCount * 2))
            //         .SetEase(Ease.InQuad);
            //     sequence.Append(tween2);
            // }
            //
            // return sequence;
            // TODO DashTween implementation
            return null;
        }
    }
}