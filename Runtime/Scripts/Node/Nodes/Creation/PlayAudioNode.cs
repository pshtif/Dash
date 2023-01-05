/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System.Runtime.InteropServices;
using Dash.Attributes;
using Dash.Editor;
using UnityEngine;

namespace Dash
{
    [Attributes.Tooltip("Creates audio source and plays an audio on controller.")]
    [Category(NodeCategoryType.CREATION, "Creation/Audio")]
    [OutputCount(1)]
    [InputCount(1)]
    public class PlayAudioNode : NodeBase<PlayAudioNodeModel>
    {
        private AudioSource _audioSource;

        private void InvalidateAudioSource()
        {
            if (_audioSource == null)
            {
                _audioSource = Controller.GetComponent<AudioSource>();
                if (_audioSource == null)
                {
                    _audioSource = Controller.gameObject.AddComponent<AudioSource>();
                }
            }
        }
        
        protected override void OnExecuteStart(NodeFlowData p_flowData)
        {
            if (Model.useAudioManager)
            {
                PlayUsingSoundManager(p_flowData);
            }
            else
            {
                PlayUsingAudioSource(p_flowData);
            }

            OnExecuteEnd();
            OnExecuteOutput(0, p_flowData);
        }

        protected void PlayUsingAudioSource(NodeFlowData p_flowData)
        {
            InvalidateAudioSource();

            float volume = GetParameterValue(Model.audioVolume, p_flowData);
            if (Model.audioClip != null)
            {
#if UNITY_EDITOR
                if (DashEditorCore.EditorConfig.enableSoundInPreview || !Application.isEditor || Application.isPlaying)
#endif
                    _audioSource.PlayOneShot(Model.audioClip, volume);
            }
        }
        
        protected void PlayUsingSoundManager(NodeFlowData p_flowData)
        {
            float volume = GetParameterValue(Model.audioVolume, p_flowData);
            string audioName = GetParameterValue(Model.audioName, p_flowData);

            IAudioManager audioManager = DashCore.Instance.GetAudioManager();
            if (audioManager != null)
            {
                audioManager.PlayAudio(audioName, volume);
            }
            else
            {
#if UNITY_EDITOR
                if (DashEditorCore.EditorConfig.enableSoundInPreview && !Application.isPlaying)
                {
                    Model.GetAudioManagerEditorInstance().PlayAudioPreview(audioName, volume);
                }
#endif
            }
            // TODO debug in editor time fail
        }
    }
}