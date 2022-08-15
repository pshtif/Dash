/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Linq;
using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    [CustomInspector]
    public class PlaySoundNodeModel : NodeModelBase
    {
        public bool useSoundManager = false;
        
        [Dependency("useSoundManager", false)]
        public AudioClip audioClip;

        private ISoundManager _soundManager;

        public Parameter<float> audioVolume = new Parameter<float>(1);
        
#if UNITY_EDITOR
        protected override void DrawCustomInspector()
        {
            var style = new GUIStyle();
            style.alignment = TextAnchor.UpperCenter;
            style.normal.textColor = Color.white;
            GUILayout.Label("Method");
        }
#endif

        private ISoundManager GetSoundManager()
        {
            if (_soundManager == null)
            {
                // var type = typeof(ISoundManager);
                // var types = AppDomain.CurrentDomain.GetAssemblies()
                //     .SelectMany(s => s.GetTypes())
                //     .Where(p => type.IsAssignableFrom(p) && p.IsClass);
                // _soundManager = ;
            }

            return _soundManager;
        }
    }
}