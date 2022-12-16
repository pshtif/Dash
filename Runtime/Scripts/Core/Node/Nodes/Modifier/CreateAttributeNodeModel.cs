/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using Dash.Attributes;
using UnityEngine;

namespace Dash
{

    [CustomInspector("Attributes")]
    public class CreateAttributeNodeModel : NodeModelBase
    {
        [HideInInspector]
        public List<AttributeDefinition> attributes;
        
        protected override bool DrawCustomInspector()
        {
            GUILayout.Space(2);
            
            if (attributes != null)
            {
                foreach (var attribute in attributes)
                {
                    GUIPropertiesUtils.PropertyField(attribute.GetType().GetField("name"), attribute, null);
                    GUIPropertiesUtils.PropertyField(attribute.GetType().GetField("expression"), attribute, null);
                    GUIPropertiesUtils.PropertyField(attribute.GetType().GetField("specifyType"), attribute, null);
                    if (attribute.specifyType)
                    {
                        GUIPropertiesUtils.PropertyField(attribute.GetType().GetField("type"), attribute, null);
                    }
                }
            }

            if (GUILayout.Button("Add Attribute"))
            {
                if (attributes == null)
                    attributes = new List<AttributeDefinition>();
                
                attributes.Add(new AttributeDefinition());
                return true;
            }

            return false;
        }
    }
}