/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using DG.Tweening;
using UnityEngine;

namespace Dash
{
    public class DOPreview
    {
//         public static void DelayedCall(float p_delay, TweenCallback p_callback, bool p_ignoreTimeScale = true)
//         {
//             Tween tween = DOVirtual.DelayedCall(p_delay, p_callback, p_ignoreTimeScale);
//             
// #if UNITY_EDITOR
//             if (!Application.isPlaying)
//                 EditorAssemblyCaller.Call("TweenPreviewer", "StartPreview", new object[]{ tween });
// #endif
//         }

        public static void StartPreview(Tween p_tween)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                EditorAssemblyCaller.Call("TweenPreviewer", "StartPreview", new object[]{ p_tween });
#endif
        }
        
        public static void StopPreview()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                EditorAssemblyCaller.Call("TweenPreviewer", "StopPreview", new object[]{});
#endif
        }
    }
}