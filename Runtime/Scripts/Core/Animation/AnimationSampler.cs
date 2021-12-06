/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System.Collections.Generic;
using UnityEngine;

namespace Dash
{
    public class AnimationSampler
    {
        static public AnimationStartsCache CacheStarts(Transform p_target, DashAnimation p_animation, bool p_isReverse, float p_duration) 
        {
            AnimationStartsCache cache = new AnimationStartsCache();
            
            // A lot of sampler mapping targets rect so we prestore it
            RectTransform rect = p_target.GetComponent<RectTransform>();
            
            foreach (string property in p_animation.AnimationCurves.Keys)
            {
                AnimationCurve curve = p_animation.AnimationCurves[property];
                cache.SetCurveStartCache(property, p_isReverse ? curve.Evaluate(p_duration) : curve.Evaluate(0));
                
                if (property.StartsWith("m_AnchoredPosition"))
                {
                    if (property.EndsWith(".x"))
                        cache.SetTargetStartCache(property, rect.anchoredPosition.x);
                    if (property.EndsWith(".y"))
                        cache.SetTargetStartCache(property, rect.anchoredPosition.y);
                }
            }

            return cache;
        }

        static public void ApplyFromCurves(Transform p_target, AnimationStartsCache p_cache,
            DashAnimation p_animation, float p_time, bool p_isRelative = false)
        {
            RectTransform rect = p_target.GetComponent<RectTransform>();

            foreach (string property in p_animation.AnimationCurves.Keys)
            {
                AnimationCurve curve = p_animation.AnimationCurves[property];
                float val = curve.Evaluate(p_time);
                
                if (p_isRelative && p_cache.HasTargetStartCache(property))
                    val = p_cache.GetTargetStartCache(property) + val - p_cache.GetCurveStartCache(property);
                
                if (property.StartsWith("m_AnchoredPosition"))
                {
                    if (property.EndsWith(".x")) 
                        rect.anchoredPosition = new Vector2(val, rect.anchoredPosition.y); 
                    if (property.EndsWith(".y")) 
                        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, val);
                }

                if (property.StartsWith("localEulerAnglesRaw"))
                {
                    if (property.EndsWith(".x")) 
                        rect.localRotation = Quaternion.Euler(val, rect.localRotation.y, rect.localRotation.z); 
                    if (property.EndsWith(".y")) 
                        rect.localRotation = Quaternion.Euler(rect.localRotation.x, val, rect.localRotation.z);
                    if (property.EndsWith(".z")) 
                        rect.localRotation = Quaternion.Euler(rect.localRotation.x, rect.localRotation.z, val);
                }
            }
        }
    }
}