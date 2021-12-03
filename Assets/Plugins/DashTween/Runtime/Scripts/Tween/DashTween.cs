/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;
using Random = UnityEngine.Random;

namespace Dash
{
    public class DashTween : IInternalTweenAccess
    {
        static internal List<DashTween> _activeTweens = new List<DashTween>();

        static private Queue<DashTween> _pooledTweens = new Queue<DashTween>();
        
        public Object target { get; private set; }
        public float delay { get; private set; }
        public float from { get; private set; }
        public float to { get; private set; }
        public float duration { get; private set; }

        public EaseType easeType = EaseType.LINEAR;
        
        public float current { get; private set; }

        public bool running { get; private set; } = false;

        private Action<float> _updateCallback;
        private Action _completeCallback;

        //private bool _justStarted = true;
        private bool _active = true;
        public int Id { get; private set; } = 0;

        public static DashTween To(Object p_target, float p_from, float p_to, float p_time)
        {
            DashTweenCore.Initialize();
            
            DashTween tween = Create(p_target, p_from, p_to, p_time);
            return tween;
        }

        public static void DelayedCall(float p_time, Action p_callback)
        {
            DashTweenCore.Initialize();

            Create(null, 0, 1, p_time).OnComplete(p_callback).Start();
        }

        private static DashTween Create(Object p_target, float p_from, float p_to, float p_duration)
        {
            DashTween tween;
            if (_pooledTweens.Count == 0)
            {
                tween = new DashTween();
                //tween.Id = Random.Range(100000, 999999);
            }
            else
            {
                tween = _pooledTweens.Dequeue();
                tween.current = 0;
                tween.running = false;
                tween.delay = 0;
                tween.easeType = EaseType.LINEAR;
                tween._updateCallback = null;
                tween._completeCallback = null;
                //tween._justStarted = true;
                tween._active = true;
            }
            
            tween.target = p_target;
            tween.from = p_from;
            tween.to = p_to;
            tween.duration = p_duration;
            
            _activeTweens.Add(tween);

            return tween;
        }

        public DashTween SetDelay(float p_delay)
        {
            delay = p_delay;
            return this;
        }

        public DashTween SetEase(EaseType p_easeType)
        {
            easeType = p_easeType;
            return this;
        }

        public DashTween OnUpdate(Action<float> p_callback)
        {
            _updateCallback = p_callback;
            return this;
        }

        public DashTween OnComplete(Action p_callback)
        {
            _completeCallback = p_callback;
            return this;
        }
        
        public void Start()
        {
            if (duration == 0 && delay == 0)
            {
                _updateCallback?.Invoke(EaseValue(from, to, 1, easeType));
                _completeCallback?.Invoke();
                Clean();
            }
            else
            {
                _updateCallback?.Invoke(from);
                running = true;
            }
        }

        public void Kill(bool p_complete)
        {
            if (p_complete)
            {
                current = duration+delay;
                _updateCallback?.Invoke(EaseValue(from, to, (current - delay) / duration, easeType));
                _completeCallback?.Invoke();
            }
            
            Clean();
        }

        void IInternalTweenAccess.Update(float p_time)
        {
            if (!running)
                return;

            // if (_justStarted)
            // {
            //     _updateCallback?.Invoke(from);
            //     _justStarted = false;
            //     return;
            // }
            
            current += p_time;
            if (current >= duration + delay)
            {
                current = duration + delay;
                
                _updateCallback?.Invoke(EaseValue(from, to, (current - delay) / duration, easeType));
                _completeCallback?.Invoke();
                Clean();

                return;
            }
            
            if (current > delay)
            {
                _updateCallback?.Invoke(EaseValue(from, to, (current - delay) / duration, easeType));
            }
        }

        void Clean()
        {
            if (!_active)
                return;
            
            _active = false;
            running = false;
            _activeTweens.Remove(this);
            _pooledTweens.Enqueue(this);
        }

        public static void CleanAll()
        {
            _activeTweens.Clear();
            _pooledTweens.Clear();
        }
        
        public static float EaseValue(float p_from, float p_to, float p_delta, EaseType p_easeType)
        {
            return p_from + (p_to - p_from) * EaseFunctions.Evaluate(p_delta, 1f, p_easeType);
        }
    }
}