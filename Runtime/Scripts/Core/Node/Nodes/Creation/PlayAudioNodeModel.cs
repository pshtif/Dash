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
    public class PlayAudioNodeModel : NodeModelBase
    {
        public Parameter<float> audioVolume = new Parameter<float>(1);
        
        public bool useSoundManager = false;
        
        [Dependency("useSoundManager", false)]
        public AudioClip audioClip;

        [Dependency("useSoundManager", true)] 
        public Parameter<string> audioName = new Parameter<string>("");

        private IAudioManager _soundManager;

#if UNITY_EDITOR
        protected override void DrawCustomInspector()
        {
            var style = new GUIStyle();
            style.alignment = TextAnchor.UpperCenter;
            style.normal.textColor = Color.white;
            GUILayout.Label("Method");
        }
#endif
    }
}