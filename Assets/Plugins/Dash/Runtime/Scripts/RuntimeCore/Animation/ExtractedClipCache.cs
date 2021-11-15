/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dash
{
    [Serializable]
    public class ExtractedClipCache
    {
        private Dictionary<AnimationClip, ExtractedClip> _clips;

        public ExtractedClipCache()
        {
            _clips = new Dictionary<AnimationClip, ExtractedClip>();
        }

        public ExtractedClip GetExtractedClip(AnimationClip p_sourceClip)
        {
            if (p_sourceClip == null)
                return null;
            
            return _clips[p_sourceClip];
        }
        
        
#if UNITY_EDITOR
        public ExtractedClip CacheClip(AnimationClip p_sourceClip)
        {
            if (!_clips.ContainsKey(p_sourceClip))
            {
                ExtractedClip clip = new ExtractedClip();
                clip.Extract(p_sourceClip);
                _clips.Add(p_sourceClip, clip);
            }

            return _clips[p_sourceClip];
        }
#endif
    }
}