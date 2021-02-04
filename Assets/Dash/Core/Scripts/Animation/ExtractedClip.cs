/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Dash
{
    [Serializable]
    public class ExtractedClip
    {
        public float length { get; private set; }

        protected Dictionary<string, AnimationCurve> _animationCurves;

        public Dictionary<string, AnimationCurve> AnimationCurves => _animationCurves;
      
        #if UNITY_EDITOR
        public void Extract(AnimationClip p_clip)
        {
            _animationCurves = new Dictionary<string, AnimationCurve>();
            
            if (p_clip == null)
                return;

            length = p_clip.length;
            
            EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(p_clip);
            foreach (EditorCurveBinding binding in bindings)
            {
                _animationCurves[binding.propertyName] = AnimationUtility.GetEditorCurve(p_clip, binding);
            }
        }
        #endif
    }
}