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
    [Serializable]
    public class AttributeDefinition
    {
        [DisallowWhitespace]
        public Parameter<string> name = new Parameter<string>("attribute");
        public Type attributeType = typeof(int);
        public Parameter attributeValue;
        
        [HideInInspector]
        public bool specifyType = false;
        [HideInInspector]
        public Parameter<string> expression = new Parameter<string>("");
        
#if UNITY_EDITOR

        public static bool DrawAttributes(IViewOwner p_owner, ref List<AttributeDefinition> p_attributes)
        {
            bool changed = false;
            if (p_attributes != null)
            {
                foreach (var attribute in p_attributes)
                {
                    if (DrawAttribute(p_owner, p_attributes, attribute))
                    {
                        changed = true;
                        break;
                    }
                }
            }
            
            GUI.color = new Color(1, 0.75f, 0.5f);
            if (GUILayout.Button("Add Attribute"))
            {
                if (p_attributes == null)
                    p_attributes = new List<AttributeDefinition>();
                
                p_attributes.Add(new AttributeDefinition());
                return true;
            }
            GUI.color = Color.white;

            return changed;
        }
        public static bool DrawAttribute(IViewOwner p_owner, List<AttributeDefinition> p_attributes, AttributeDefinition p_attribute)
        {
            bool changed = false;
            var style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.MiddleLeft;
            style.fontStyle = FontStyle.Bold;
            style.normal.background = Texture2D.whiteTexture;
            style.fontSize = 11;
            GUI.backgroundColor = new Color(0, 0, 0, .5f);
            GUILayout.Label("  Attribute", style, GUILayout.Height(22));
            GUI.backgroundColor = Color.white;
            
            var rect = GUILayoutUtility.GetLastRect();

            style = new GUIStyle();
            style.fontSize = 15;
            style.normal.textColor = Color.white;

            GUI.Label(new Rect(rect.x + rect.width - 14, rect.y, 24, 24), "x", style);

            if (GUI.Button(new Rect(rect.x + rect.width - 16, rect.y, 16, rect.height), "", GUIStyle.none)) 
            {
                DeleteAttribute(p_attributes, p_attribute);
                return true;
            }

            Parameter<string> currentName = (Parameter<string>)p_attribute.GetType().GetField("name").GetValue(p_attribute);
            GUI.color = !currentName.isExpression && ReservedParameters.IsReservedParameter(currentName.GetValue(null)) ? Color.red : Color.white;
            changed = changed || GUIPropertiesUtils.PropertyField(p_attribute.GetType().GetField("name"), p_attribute, null, null, null);
            GUI.color = Color.white;
            //changed = changed || GUIPropertiesUtils.PropertyField(p_attribute.GetType().GetField("expression"), p_attribute, null);
            //changed = changed || GUIPropertiesUtils.PropertyField(p_attribute.GetType().GetField("specifyType"), p_attribute, null);
            // if (p_attribute.specifyType)
            // {
            //     changed = changed || GUIPropertiesUtils.PropertyField(p_attribute.GetType().GetField("type"), p_attribute, null);
            // }
            changed = changed || GUIPropertiesUtils.PropertyField(p_attribute.GetType().GetField("attributeType"), p_attribute, null,  null, null);

            if (p_attribute.attributeType == null)
            {
                p_attribute.attributeType = typeof(string);
            }
            
            if (p_attribute.attributeValue == null ||
                p_attribute.attributeValue.GetValueType() != p_attribute.attributeType)
            {
                p_attribute.attributeValue = Parameter.Create(p_attribute.attributeType);
            }

            changed = changed || GUIPropertiesUtils.PropertyField(p_attribute.GetType().GetField("attributeValue"), p_attribute, null,  p_owner.GetConfig().editingController, null);

            return changed;
        }
        
        static void DeleteAttribute(List<AttributeDefinition> p_attributes, AttributeDefinition p_attribute)
        {
            p_attributes.Remove(p_attribute);
        }
#endif
    }
}