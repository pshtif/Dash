/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.CodeDom;
using System.Runtime.CompilerServices;
using Dash.Attributes;
using DG.Tweening;
using Dash.Enums;
using OdinSerializer;
using UnityEngine;

namespace Dash
{
    [Serializable]
    public class AnimateWithPresetNodeModel : AnimationNodeBaseModel
    {
        [Popup(PopupType.CLASS, typeof(IAnimationPreset))]
        public IAnimationPreset preset;
    }
}