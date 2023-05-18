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
        protected override bool DrawCustomInspector(IViewOwner p_owner)
        {
            GUILayout.Space(2);

            bool changed = AttributeDefinition.DrawAttributes(p_owner, attributes);

            return changed;
        }
#endif
    }
}