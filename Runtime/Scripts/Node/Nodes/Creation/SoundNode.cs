/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using Dash.Attributes;
using Dash.Editor;
using UnityEngine;

namespace Dash
{
    [Attributes.Tooltip("Creates audio source and plays a sound on controller.")]
    [Category(NodeCategoryType.CREATION)]
    [OutputCount(1)]
    [InputCount(1)]
    [Obsolete]
    public class SoundNode : NodeBase<SoundNodeModel>
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
            InvalidateAudioSource();

            float volume = GetParameterValue(Model.audioVolume, p_flowData);
            if (Model.audioClip != null)
            {
#if UNITY_EDITOR
                if (DashEditorCore.EditorConfig.enableSoundInPreview || !Application.isEditor || Application.isPlaying)
#endif
                    _audioSource.PlayOneShot(Model.audioClip, volume);
            }

            OnExecuteEnd();
            OnExecuteOutput(0, p_flowData);
        }
    }
}