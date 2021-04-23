/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    [Help("Animate RectTransform using an AnimationClip or DashAnimationClip instead of tweening.")]
    [Category(NodeCategoryType.ANIMATION)]
    [OutputCount(1)]
    [InputCount(1)]
    [Serializable]
    public class AnimateWithClipNode : AnimationNodeBase<AnimateWithClipNodeModel>
    {
        protected Dictionary<Transform, AnimationStartsCache> _startsCaches;

        override protected DashTween AnimateOnTarget(Transform p_target, NodeFlowData p_flowData)
        {
            float time = GetParameterValue(Model.time, p_flowData);
            float delay = GetParameterValue(Model.delay, p_flowData);
            EaseType easeType = GetParameterValue(Model.easeType, p_flowData);
            
            DashTween tween;
            if (Model.useDashAnimation)
            {
                if (Model.source == null)
                {
                    SetError("Animation source cannot be null");
                    return null;
                }

                CacheStarts(p_target);

                // Change time if we are using custom one
                time = Model.useAnimationTime ? Model.source.Duration : time; 

                // Virtual tween to update from sampler
                tween = DashTween.To(p_target, 0, Model.source.Duration, time)
                    .OnUpdate(f => UpdateFromAnimation(p_target, f))
                    .SetDelay(delay)
                    .SetEase(easeType);
            }
            else
            {
                if (Model.clip == null)
                {
                    SetError("Animation clip cannot be null");
                    return null;
                }
                
                // Change time if we are using custom one
                time = Model.useAnimationTime ? Model.clip.length : time;

                // Virtual tween to update from sampler
                tween = DashTween.To(p_target, 0, Model.clip.length, time)
                    .OnUpdate(f => UpdateFromClip(p_target, f))
                    .SetDelay(delay)
                    .SetEase(easeType);
            }

            return tween;
        }

        void ExecuteEnd(NodeFlowData p_flowData)
        {
            OnExecuteEnd();
            OnExecuteOutput(0,p_flowData);
        }
        
        private void CacheStarts(Transform p_target)
        {
            // Generate starts cache
            AnimationStartsCache cache =
                AnimationSampler.CacheStarts(p_target, Model.source, Model.isReverse, Model.source.Duration);

            // Create cache if it doesn't exist
            if (_startsCaches == null)
                _startsCaches = new Dictionary<Transform, AnimationStartsCache>();
            
            // Store animation cache for this target
            _startsCaches[p_target] = cache;
        }

        private void UpdateFromAnimation(Transform p_target, float p_time)
        {
            // If we are in reverse we need to go back in time
            if (Model.isReverse)
                p_time = Model.source.Duration - p_time;

            // Apply from cache and prestored animation curves
            AnimationSampler.ApplyFromCurves(p_target, _startsCaches[p_target], Model.source, p_time,
                Model.isRelative);
        }
        
        private void UpdateFromClip(Transform p_target, float p_time)
        {
            // If we are in reverse we need to go back in time
            if (Model.isReverse)
                p_time = Model.clip.length - p_time;

            Model.clip.SampleAnimation(p_target.gameObject, p_time);
        }
        
#if UNITY_EDITOR

        protected override void DrawCustomGUI(Rect p_rect)
        {
            base.DrawCustomGUI(p_rect);

            float time = Model.useAnimationTime ? Model.source != null ? Model.source.Duration : 0 :
                Model.time.isExpression ? 0 : Model.time.GetValue(null);
            
            if (Model.time.isExpression && !Model.useAnimationTime)
            {
                GUI.Label(new Rect(p_rect.x + p_rect.width / 2 - 50, p_rect.y + p_rect.height - 32, 100, 20),
                    "Time: [Exp]", DashEditorCore.Skin.GetStyle("NodeText"));
            }
            else
            {
                GUI.Label(new Rect(p_rect.x + p_rect.width / 2 - 50, p_rect.y + p_rect.height - 32, 100, 20),
                    "Time: " + time + "s", DashEditorCore.Skin.GetStyle("NodeText"));
            }
        }
#endif
    }
}
