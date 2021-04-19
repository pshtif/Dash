/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEditor;
using UnityEngine;

namespace Dash
{
    [CustomEditor(typeof(DashAnimation))]
    public class DashAnimationAssetInspector : Editor
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