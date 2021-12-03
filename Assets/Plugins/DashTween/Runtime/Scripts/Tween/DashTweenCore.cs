/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Dash
{
    public class DashTweenCore : MonoBehaviour
    {
        [NonSerialized]
        private static bool _initialized = false;

        public bool useScaledTime = true;

        public static void Initialize()
        {
            if (!_initialized)
            {
                if (Application.isPlaying)
                {
                    var go = new GameObject();
                    go.name = "DashTweenCore";
                    go.hideFlags = HideFlags.HideAndDontSave;
                    go.AddComponent<DashTweenCore>();
                    DontDestroyOnLoad(go);
                }
                else
                {
                    #if UNITY_EDITOR
                    _currentTime = EditorApplication.timeSinceStartup;
                    EditorApplication.update -= UpdateEditor;
                    EditorApplication.update += UpdateEditor;
                    #endif
                }

                _initialized = true;
            }
        }
        
        #if UNITY_EDITOR
        public static void Uninitialize()
        {
            _initialized = false;
            EditorApplication.update -= UpdateEditor;
        }
        #endif

        void Update()
        {
            UpdateInternal(useScaledTime ? Time.deltaTime : Time.unscaledDeltaTime);
        }

        static void UpdateInternal(float p_delta)
        {
            var cloned = DashTween._activeTweens.ToList();
            cloned.ForEach(t =>
            {
                ((IInternalTweenAccess)t).Update(p_delta);
            });
            // for (int i = 0; i < cloned.Count; i++)
            // {
            //     if (((IInternalTweenAccess) DashTween.coned[i]).Update(p_delta))
            //     {
            //         // In place removing of completed tweens
            //         i--;
            //     }
            // }
        }
        
        #if UNITY_EDITOR
        public static void Reset()
        {
            _initialized = false;
            DashTween.CleanAll();
        }
        
        private static double _currentTime = 0;
        
        private static void UpdateEditor()
        {
            float delta = (float) (EditorApplication.timeSinceStartup - _currentTime);
            
            UpdateInternal(delta);
            
            _currentTime = EditorApplication.timeSinceStartup;
        }
        #endif
    }
}