/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System.Collections.Generic;

namespace Dash
{
    public class AnimationStartsCache
    {
        private Dictionary<string, float> _cachedCurveStarts;
        private Dictionary<string, float> _cachedTargetStarts;

        public AnimationStartsCache()
        {
            _cachedCurveStarts = new Dictionary<string, float>();
            _cachedTargetStarts = new Dictionary<string, float>();
        }

        public void SetCurveStartCache(string p_property, float p_value)
        {
            _cachedCurveStarts[p_property] = p_value;
        }
        
        public float GetCurveStartCache(string p_property)
        {
            return _cachedCurveStarts[p_property];
        }

        public bool HasTargetStartCache(string p_property)
        {
            return _cachedTargetStarts.ContainsKey(p_property);
        }
        
        public void SetTargetStartCache(string p_property, float p_value)
        {
            _cachedTargetStarts[p_property] = p_value;
        }

        public float GetTargetStartCache(string p_property)
        {
            return _cachedTargetStarts[p_property];
        }
    }
}