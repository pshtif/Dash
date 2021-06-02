/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Dash
{
    public class DashTweenCore : MonoBehaviour
    {
        [NonSerialized]
        private static bool _initialized = false;

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

        void Update()
        {
            UpdateInternal(DashCore.Instance.useScaledTime ? Time.deltaTime : Time.unscaledDeltaTime);
        }

        static void UpdateInternal(float p_delta)
        {
            for (int i = 0; i < DashTween._activeTweens.Count; i++)
            {
                if (((IInternalTweenAccess) DashTween._activeTweens[i]).Update(p_delta))
                {
                    // In place removing of completed tweens
                    i--;
                }
            }
        }
        
        #if UNITY_EDITOR
        internal static void Reset()
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