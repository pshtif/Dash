/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    public class SetImageNodeModel : RetargetNodeModel
    {
        [Order(10)]
        public bool setSprite = false;
        
        [Order(11)]
        [Dependency("setSprite", true)]
        public Parameter<Sprite> sprite = new Parameter<Sprite>(null);

        [Order(12)]
        public Parameter<bool> setNativeSize = new Parameter<bool>(false);
        
        [Order(13)]
        public bool setIsMaskable = false;
        
        [Order(14)]
        [Dependency("setIsMaskable", true)]
        public Parameter<bool> isMaskable = new Parameter<bool>(true);
        
        [Order(15)]
        public bool setIsRaycastTarget = false;
        
        [Order(16)]
        [Dependency("setIsRaycastTarget", true)]
        public Parameter<bool> isRaycastTarget = new Parameter<bool>(true);
        
        [Order(17)]
        public bool storeSpriteToAttribute = false;
        
        [Order(18)]
        [Dependency("storeSpriteToAttribute", true)]
        public Parameter<string> storeSpriteAttributeName = new Parameter<string>("sprite");
    }
}