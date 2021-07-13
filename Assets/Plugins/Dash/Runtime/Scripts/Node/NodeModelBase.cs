/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Dash.Attributes;
using OdinSerializer;
using UnityEngine;

namespace Dash
{
    public class NodeModelBase : IReferencable, ISerializationCallbackReceiver
    {
        public string Id => id;
        
        [TitledGroup("Advanced", 1000, true)]
        public string id;

        [TitledGroup("Advanced", 1000, true)]
        public string comment;
        
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            var fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public).ToList();
            fields = fields.FindAll(fi => fi.FieldType.IsGenericType && fi.FieldType.GetGenericTypeDefinition() == typeof(Parameter<>));

            if (fields.Count == 0)
                return;
            
            foreach (var field in fields)
            {
                if (field.GetValue(this) != null)
                    continue;

                var initializeFrom = field.GetCustomAttribute<InitializeFromAttribute>();
                if (initializeFrom != null)
                {
                    var initializationField = GetType().GetField(initializeFrom.FieldName);
                    
                    field.SetValue(this,
                        Activator.CreateInstance(field.FieldType,
                            new object[] {initializationField == null ? true : initializationField.GetValue(this)}));
                }
                else
                {
                    field.SetValue(this, Activator.CreateInstance(field.FieldType, new object[] {field.FieldType.GenericTypeArguments[0].GetDefaultValue()}));
                }
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            
        }

#if UNITY_EDITOR

        private int groupsMinized = -1;
        
        public NodeModelBase Clone()
        {
            // Doing a shallow copy
            var clone = (NodeModelBase)SerializationUtility.CreateCopy(this);

            // Exposed references are not copied in serialization as they are external Unity references so they will refer to the same exposed reference instance not just the unity object reference, we need to copy them additionally
            FieldInfo[] fields = clone.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
            fields.ToList().FindAll(f => f.FieldType.IsGenericType && f.FieldType.GetGenericTypeDefinition() == typeof(ExposedReference<>)).ForEach(f =>
            {
                Type exposedRefType = typeof(ExposedReference<>).MakeGenericType(f.FieldType.GenericTypeArguments[0]);

                if (DashEditorCore.Config.editingGraph.Controller != null)
                {
                    IExposedPropertyTable propertyTable = DashEditorCore.Config.editingGraph.Controller;
                    var curExposedRef = f.GetValue(clone);
                    UnityEngine.Object exposedValue = (UnityEngine.Object) curExposedRef.GetType().GetMethod("Resolve")
                        .Invoke(curExposedRef, new object[] {propertyTable});

                    var clonedExposedRef = Activator.CreateInstance(exposedRefType);
                    PropertyName newExposedName = new PropertyName(UnityEditor.GUID.Generate().ToString());
                    propertyTable.SetReferenceValue(newExposedName, exposedValue);
                    clonedExposedRef.GetType().GetField("exposedName")
                        .SetValue(clonedExposedRef, newExposedName);
                    f.SetValue(clone, clonedExposedRef);
                }
            });

            return clone;
        }
        
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
            minStyle.fontStyle = FontStyle.Bold;
            minStyle.fontSize = 16;
            
            var fields = this.GetType().GetFields();
            Array.Sort(fields, GUIPropertiesUtils.GroupSort);
            string lastGroup = "";
            bool lastGroupMinimized = false;
            bool invalidate = false;
            int groupIndex = 0;

            if (GetType().GetCustomAttribute<CustomInspectorAttribute>() != null)
            {
                lastGroupMinimized = DrawGroupTitle(null, ref groupIndex, ref lastGroup, initializeMinimization, minStyle);
                if (!lastGroupMinimized) DrawCustomInspector();
            }

            foreach (var field in fields)
            {
                if (field.IsConstant() || !GUIPropertiesUtils.MeetsDependencies(field, this) || GUIPropertiesUtils.IsHidden(field)) 
                    continue;
                
                lastGroupMinimized = DrawGroupTitle(field, ref groupIndex, ref lastGroup, initializeMinimization, minStyle);

                if (lastGroupMinimized)
                    continue;

                invalidate = invalidate || GUIPropertiesUtils.PropertyField(field, this, this);
            }

            return invalidate;
        }

        protected bool DrawGroupTitle(FieldInfo p_fieldInfo, ref int p_groupIndex, ref string p_lastGroup, bool p_initializeMinimized, GUIStyle p_style)
        {
            TitledGroupAttribute ga = p_fieldInfo?.GetCustomAttribute<TitledGroupAttribute>();
            string currentGroup = ga != null ? ga.Group : "Properties";
            if (currentGroup != p_lastGroup)
            {
                int groupMask = 1 << p_groupIndex;
                p_groupIndex++;
                if (p_initializeMinimized && ga != null && ga.Minimized && (groupsMinized & groupMask) == 0)
                {
                    groupsMinized += groupMask;
                }

                GUIPropertiesUtils.Separator(16, 2, 4, new Color(0.1f, 0.1f, 0.1f));
                GUILayout.Label(currentGroup, DashEditorCore.Skin.GetStyle("PropertyGroup"));
                    
                Rect lastRect = GUILayoutUtility.GetLastRect();

                bool isMinimized = (groupsMinized & groupMask) != 0;
                if (GUI.Button(new Rect(lastRect.x, lastRect.y - 25, lastRect.width, 20), "", p_style))
                {
                    groupsMinized = isMinimized ? groupsMinized - groupMask : groupsMinized + groupMask;
                }
                    
                GUI.Label(new Rect(lastRect.x + 316 + (isMinimized ? 0 : 2), lastRect.y - 25, 20, 20), isMinimized ? "+" : "-", p_style);
            
                p_lastGroup = currentGroup;
            }
            
            return (groupsMinized & (1 << (p_groupIndex - 1))) != 0;
        }

        protected virtual void DrawCustomInspector() { }

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