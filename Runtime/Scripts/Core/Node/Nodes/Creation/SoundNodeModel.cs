/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    public class SoundNodeModel : NodeModelBase
    {
        public AudioClip audioClip;

        public Parameter<float> audioVolume = new Parameter<float>(1);
    }
}