/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using Dash.Attributes;
using DG.Tweening;
using Dash.Enums;
using UnityEngine;

namespace Dash
{
    [Serializable]
    public class AnimateColorNodeModel : AnimationNodeBaseModel
    {
        [TitledGroup("Properties")]
        public AlphaSourceType sourceType = AlphaSourceType.CANVASGROUP;
        
        [TitledGroup("Properties")]
        [Dependency("sourceType", AlphaSourceType.CANVASGROUP)]
        public Parameter<float> toAlpha = new Parameter<float>(1);
        
        [TitledGroup("Properties")]
        [Dependency("sourceType", AlphaSourceType.CANVASGROUP)]
        public bool isToRelative = true;

        [DependencySingle("sourceType", AlphaSourceType.IMAGE)]
        [DependencySingle("sourceType", AlphaSourceType.TEXTMESHPRO)]
        [TitledGroup("Properties")]
        public Parameter<Color> toColor = new Parameter<Color>(Color.white);
    }
}