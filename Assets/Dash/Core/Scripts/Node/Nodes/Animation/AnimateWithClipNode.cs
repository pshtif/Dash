/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using Dash.Attributes;
using DG.Tweening;
using UnityEngine;

namespace Dash
{
    [Category(NodeCategoryType.ANIMATION)]
    [OutputCount(1)]
    [InputCount(1)]
    [Serializable]
    public class AnimateWithClipNode : RetargetNodeBase<AnimateWithClipNodeModel>
    {
        protected Dictionary<Transform, AnimationStartsCache> _startsCaches;

        public ExtractedClip clip => Graph.ExtractedClipCache.GetExtractedClip(Model.sourceClip);

        override protected void ExecuteOnTarget(Transform p_target, NodeFlowData p_flowData)
        {
            CacheStarts(p_target);
            
            // Change time if we are using custom one
            float time = Model.useAnimationTime ? clip.length : Model.time;

            // Virtual tween to update from sampler
            Tween tween = DOTween.To((f) => UpdateFromClip(p_target, f), 0, clip.length, time)
                .SetDelay(Model.delay)
                .SetEase(Model.easing)
                .OnComplete(() => ExecuteEnd(p_flowData));
            
            DOPreview.StartPreview(tween);
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
                AnimationSampler.CacheStarts(p_target, clip, Model.isReverse, clip.length);

            // Create cache if it doesn't exist
            if (_startsCaches == null)
                _startsCaches = new Dictionary<Transform, AnimationStartsCache>();
            
            // Store animation cache for this target
            _startsCaches[p_target] = cache;
        }

        private void UpdateFromClip(Transform p_target, float p_time)
        {
            // If we are in reverse we need to go back in time
            if (Model.isReverse)
                p_time = clip.length - p_time;

            // Apply from cache and prestored animation curves
            AnimationSampler.ApplyFromCurves(p_target, _startsCaches[p_target], clip, p_time,
                Model.isRelative);
        }
        
#if UNITY_EDITOR
        public override bool Invalidate()
        {
            if (Model.sourceClip != null)
                Graph.ExtractedClipCache.CacheClip(Model.sourceClip);

            return true;
        }
        
        protected override void DrawCustomGUI(Rect p_rect)
        {
            base.DrawCustomGUI(p_rect);

            float time = Model.useAnimationTime ?
                clip != null ? clip.length : 0 :
                Model.time;
            
            GUI.Label(new Rect(p_rect.x + p_rect.width / 2 - 50, p_rect.y + p_rect.height - 32, 100, 20),
                "Time: " + time + "s", DashEditorCore.Skin.GetStyle("NodeText"));
        }
#endif
    }
}
