/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    public class NodeModelBase
    {
        [TitledGroup("Advanced", true)]
        public string id;

        [TitledGroup("Advanced", true)] 
        [Tooltip("Execute node even if target is not valid.")]
        public bool executeOnNull;

#if UNITY_EDITOR
        private Dictionary<string, bool> groupsMinized;
        
        public virtual bool DrawInspector()
        {
            bool initializeMinimization = false;
            if (groupsMinized == null)
            {
                initializeMinimization = true;
                groupsMinized = new Dictionary<string, bool>();
            }
            
            GUILayout.Space(5);
            
            GUIStyle minStyle = GUIStyle.none;
            minStyle.normal.textColor = Color.white;
            minStyle.fontSize = 16;
            
            var fields = this.GetType().GetFields();
            Array.Sort(fields, GroupSort);
            string lastGroup = "";
            bool lastGroupMinimized = false;
            bool invalidate = false;
            foreach (var field in fields)
            {
                if (field.IsConstant()) continue;

                TitledGroupAttribute ga = field.GetCustomAttribute<TitledGroupAttribute>();
                string currentGroup = ga != null ? ga.Group : "Other";
                if (currentGroup != lastGroup)
                {
                    if (initializeMinimization || !groupsMinized.ContainsKey(currentGroup))
                    {
                        groupsMinized[currentGroup] = ga != null ? ga.Minimized : false;
                    }

                    GUIPropertiesUtils.Separator(16, 2, 4, new Color(0.1f, 0.1f, 0.1f));
                    GUILayout.Label(currentGroup, DashEditorCore.Skin.GetStyle("PropertyGroup"),
                        GUILayout.Width(120));
                    Rect lastRect = GUILayoutUtility.GetLastRect();


                    if (GUI.Button(new Rect(lastRect.x + 302, lastRect.y - 25, 20, 20), groupsMinized[currentGroup] ? "+" : "-",
                        minStyle))
                    {
                        groupsMinized[currentGroup] = !groupsMinized[currentGroup];
                    }

                    lastGroup = currentGroup;
                    lastGroupMinimized = groupsMinized[currentGroup];
                }

                if (lastGroupMinimized)
                    continue;

                invalidate = GUIPropertiesUtils.PropertyField(field, this);
            }

            return invalidate;
        }

        protected int OrderSort(FieldInfo p_field1, FieldInfo p_field2)
        {
            OrderAttribute attribute1 = p_field1.GetCustomAttribute<OrderAttribute>();
            OrderAttribute attribute2 = p_field2.GetCustomAttribute<OrderAttribute>();
            
            if (attribute1 == null && attribute2 == null)
                return 0;

            if (attribute1 != null && attribute2 == null)
                return -1;
            
            if (attribute1 == null && attribute2 != null)
                return 1;

            return attribute1.Order.CompareTo(attribute2.Order);
        }
        
        protected int GroupSort(FieldInfo p_field1, FieldInfo p_field2)
        {
            TitledGroupAttribute ga1 = p_field1.GetCustomAttribute<TitledGroupAttribute>();
            TitledGroupAttribute ga2 = p_field2.GetCustomAttribute<TitledGroupAttribute>();
            if (ga1 == null && ga2 == null)
                return OrderSort(p_field1, p_field2);

            if (ga1 != null && ga2 == null)
                return ga1.Group == "Advanced" ? 1 : -1;

            if (ga1 == null && ga2 != null)
                return ga2.Group == "Advanced" ? -1 : 1;

            if (ga1.Group == "Advanced")
                return 1;

            if (ga2.Group == "Advanced")
                return -1;

            if (ga1.Group == ga2.Group)
                return OrderSort(p_field1, p_field2);
            
            return ga1.Group.CompareTo(ga2.Group);
        }

        public List<string> GetExposedGUIDs()
        {
            return GetType().GetFields().ToList().FindAll(f => f.FieldType.IsGenericType && f.FieldType.GetGenericTypeDefinition() == typeof(ExposedReference<>)).Select(
                    (f, i) => f.GetValue(this).GetType().GetField("exposedName").GetValue(f.GetValue(this)).ToString())
                .ToList();
        }
        
        public void ValidateSerialization()
        {
            var fields = this.GetType().GetFields();
            foreach (var field in fields)
            {
                if (!GUIPropertiesUtils.IsParameterProperty(field))
                    continue;
                
                if ((Parameter)field.GetValue(this) == null)
                {
                    if (!RecreateParameter(field))
                    {
                        Debug.LogWarning("Recreation of parameter property failed.");
                    }
                    else
                    {
                        Debug.LogWarning("Recreation of parameter property succeeded.");
                    }
                }
            }
        }
        
        bool RecreateParameter(FieldInfo p_fieldInfo)
        {
            Debug.LogWarning("Serialization error on parametrized property "+p_fieldInfo.Name+" encountered on model "+this+", recreating parameter to default values.");
            var genericType = p_fieldInfo.FieldType.GenericTypeArguments[0];
            var parameterType = typeof(Parameter<>).MakeGenericType(genericType);
            var parameter = Activator.CreateInstance(parameterType, genericType.GetDefaultValue());

            p_fieldInfo.SetValue(this, parameter);

            return p_fieldInfo.GetValue(this) != null;
        }
#endif
    }
}