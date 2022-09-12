/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using Dash.Attributes;
using UnityEditor;
using UnityEngine;

namespace Dash
{
    [CustomInspector("AudioManager")]
    public class PlayAudioNodeModel : NodeModelBase
    {
        public Parameter<float> audioVolume = new Parameter<float>(1);
        
        public bool useAudioManager = false;
        
        [Dependency("useAudioManager", false)]
        public AudioClip audioClip;

        [Dependency("useAudioManager", true)] 
        public Parameter<string> audioName = new Parameter<string>("");

        private IAudioManager _soundManager;

#if UNITY_EDITOR

        [NonSerialized] 
        private IAudioManager _audioManager;

        protected override bool IsCustomInspectorActive()
        {
            return (useAudioManager && !audioName.isExpression);
        }
        
        protected override bool DrawCustomInspector()
        {
            if (useAudioManager && !audioName.isExpression)
            {
                if (_audioManager == null)
                {
                    Type[] audioManagers = ReflectionUtils.GetAllTypesImplementingInterface(typeof(IAudioManager));

                    if (audioManagers.Length == 0)
                    {
                        EditorGUILayout.HelpBox("No audio manager implementation found.", MessageType.Warning);
                        return false;
                    }

                    if (audioManagers.Length > 1)
                    {
                        EditorGUILayout.HelpBox("Multiple audio managers implementatiuons.", MessageType.Warning);
                    }

                    _audioManager = (IAudioManager)Activator.CreateInstance(audioManagers[0]);
                }

                if (_audioManager == null)
                {
                    EditorGUILayout.HelpBox("Could not instantiate audio manager.", MessageType.Warning);
                    return false;
                }

                var audioNameList = _audioManager.GetAudioNames();
                int index = Array.IndexOf(audioNameList, audioName.GetValue(null));

                EditorGUI.BeginChangeCheck();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Audio Name", GUILayout.Width(160));
                var newIndex = EditorGUILayout.Popup(index, audioNameList);
                GUILayout.EndHorizontal();

                if (index == -1)
                {
                    EditorGUILayout.HelpBox("Current value is not a valid audio name", MessageType.Warning);
                }

                if (EditorGUI.EndChangeCheck())
                {
                    audioName.SetValue(audioNameList[newIndex]);
                    return true;
                }
            }

            return false;
        }

        public IAudioManager GetAudioManagerEditorInstance()
        {
            if (_audioManager == null)
            {
                Type[] audioManagers = ReflectionUtils.GetAllTypesImplementingInterface(typeof(IAudioManager));

                if (audioManagers.Length != 1)
                    return null;

                _audioManager = (IAudioManager)Activator.CreateInstance(audioManagers[0]);
            }

            return _audioManager;
        }
#endif
    }
}