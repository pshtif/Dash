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
    public class AnimateColorNodeModel : AnimationNodeModelBase
    {
        [TitledGroup("Properties")]
        public AlphaTargetType targetType = AlphaTargetType.CANVASGROUP;
        
        [TitledGroup("Properties")]
        [Dependency("sourceType", AlphaTargetType.CANVASGROUP)]
        public Parameter<float> toAlpha = new Parameter<float>(1);
        
        [TitledGroup("Properties")]
        [Dependency("sourceType", AlphaTargetType.CANVASGROUP)]
        public bool isToRelative = true;

        [DependencySingle("sourceType", AlphaTargetType.IMAGE)]
        [DependencySingle("sourceType", AlphaTargetType.TEXTMESHPRO)]
        [TitledGroup("Properties")]
        public Parameter<Color> toColor = new Parameter<Color>(Color.white);
    }
}