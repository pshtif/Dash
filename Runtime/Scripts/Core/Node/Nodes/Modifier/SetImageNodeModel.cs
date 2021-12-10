/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    public class SetImageNodeModel : RetargetNodeModel
    {
        public bool setSprite = false;
        [Dependency("setSprite", true)]
        public Parameter<Sprite> sprite = new Parameter<Sprite>(null);

        public Parameter<bool> setNativeSize = new Parameter<bool>(false);
        
        public bool setIsMaskable = false;
        [Dependency("setIsMaskable", true)]
        public Parameter<bool> isMaskable = new Parameter<bool>(true);
        
        public bool setIsRaycastTarget = false;
        [Dependency("setIsRaycastTarget", true)]
        public Parameter<bool> isRaycastTarget = new Parameter<bool>(true);
    }
}