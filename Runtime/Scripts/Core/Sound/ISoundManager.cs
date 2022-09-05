/*
 *	Created by:  Peter @sHTiF Stefcek
 */

namespace Dash
{
    public interface IAudioManager
    {
        void PlayAudio(string p_audioName, float p_volume);

        bool HasAudio(string p_audioName);
    }
}