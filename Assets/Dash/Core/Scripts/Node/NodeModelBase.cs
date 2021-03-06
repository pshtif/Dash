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
        [TitledGroup("Advanced", 1000, true)]
        public string id;

        [TitledGroup("Advanced", 1000, true)]
        public string comment;


#if UNITY_EDITOR
        private int groupsMinized = -1;
        
        public virtual bool DrawInspector()
        {
            bool initializeMinimization = false;
            if (groupsMinized == -1)
            {
                initializeMinimization = true;
                groupsMinized = 0;
            }
            
            GUILayout.Space(5);
            
            GUIStyle minStyle = GUIStyle.none;
            minStyle.normal.textColor = Color.white;
            minStyle.fontSize = 16;
            
            var fields = this.GetType().GetFields();
            Array.Sort(fields, GUIPropertiesUtils.GroupSort);
            string lastGroup = "";
            bool lastGroupMinimized = false;
            bool invalidate = false;
            int groupIndex = 0;
            foreach (var field in fields)
            {
                if (field.IsConstant()) continue;

                TitledGroupAttribute ga = field.GetCustomAttribute<TitledGroupAttribute>();
                string currentGroup = ga != null ? ga.Group : "Properties";
                if (currentGroup != lastGroup)
                {
                    int groupMask = (int)Math.Pow(2, groupIndex);
                    groupIndex++;
                    if (initializeMinimization && ga != null && ga.Minimized && (groupsMinized & groupMask) == 0)
                    {
                        groupsMinized += groupMask;
                    }

                    GUIPropertiesUtils.Separator(16, 2, 4, new Color(0.1f, 0.1f, 0.1f));
                    GUILayout.Label(currentGroup, DashEditorCore.Skin.GetStyle("PropertyGroup"),
                        GUILayout.Width(120));
                    
                    Rect lastRect = GUILayoutUtility.GetLastRect();

                    if (GUI.Button(new Rect(lastRect.x + 302, lastRect.y - 25, 20, 20), (groupsMinized & groupMask) != 0 ? "+" : "-",
                        minStyle))
                    {
                        groupsMinized = (groupsMinized & groupMask) == 0
                            ? groupsMinized + groupMask
                            : groupsMinized - groupMask;
                    }

                    lastGroup = currentGroup;
                    lastGroupMinimized = (groupsMinized & groupMask) != 0;
                }

                if (lastGroupMinimized)
                    continue;

                invalidate = invalidate || GUIPropertiesUtils.PropertyField(this, field, this);
            }

            return invalidate;
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