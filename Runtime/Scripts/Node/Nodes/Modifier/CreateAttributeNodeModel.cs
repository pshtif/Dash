/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using Dash.Attributes;
using Dash.Editor;
using UnityEngine;

namespace Dash
{

    [CustomInspector("Attributes")]
    public class CreateAttributeNodeModel : NodeModelBase
    {
        [HideInInspector]
        public List<AttributeDefinition> attributes;
        
#if UNITY_EDITOR
        protected override bool DrawCustomInspector()
        {
            GUILayout.Space(2);

            bool changed = AttributeDefinition.DrawAttributes(attributes);

            return changed;
        }
#endif
    }
}