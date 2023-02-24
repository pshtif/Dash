/*
 *	Created by:  Peter @sHTiF Stefcek
 */
#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Dash.Editor
{
    [CustomEditor(typeof(DashAnimation))]
    public class DashAnimationAssetInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DashAnimation animation = (DashAnimation) target;

            AnimationClip clip = (AnimationClip)EditorGUILayout.ObjectField(animation.clip, typeof(AnimationClip), false);
            animation.ExtractClip(clip);

            if (animation.clip != null)
            {
                GUILayout.Label("Animation Curves: " + animation.AnimationCurves.Count);
                GUILayout.Label("Duration: " + animation.Duration);
            }
        }
    }
}
#endif