/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Dash.Attributes;
using Dash.Editor;

namespace Dash
{
    public class NodeModelBase : IReferencable, ISerializationCallbackReceiver
    {
        private int _version = 1;
        
        public string Id => id;

        [TitledGroup("Advanced", 1000, true)]
        public string id;

        [TitledGroup("Advanced", 1000, true)]
        public string comment;

        [NonSerialized]
        [HideInInspector] 
        private List<string> migrationList = new List<string>();

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            var v = OnMigrateSerializedData(_version);
            _version = v;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            
        }

        int OnMigrateSerializedData(int p_version)
        {
            return 1;
        }

#if UNITY_EDITOR

        private int groupsMinized = -1;
        
        public NodeModelBase Clone()
        {
            // Doing a shallow copy
            var clone = (NodeModelBase)OdinSerializer.SerializationUtility.CreateCopy(this);

            // Exposed references are not copied in serialization as they are external Unity references so they will refer to the same exposed reference instance not just the unity object reference, we need to copy them additionally
            FieldInfo[] fields = clone.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
            fields.ToList().FindAll(f => f.FieldType.IsGenericType && f.FieldType.GetGenericTypeDefinition() == typeof(ExposedReference<>)).ForEach(f =>
            {
                Type exposedRefType = typeof(ExposedReference<>).MakeGenericType(f.FieldType.GenericTypeArguments[0]);

                if (DashEditorCore.EditorConfig.editingController != null)
                {
                    IExposedPropertyTable propertyTable = DashEditorCore.EditorConfig.editingController;
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
            
            GUILayout.Space(3);
            
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

            CustomInspectorAttribute customInspector = GetType().GetCustomAttribute<CustomInspectorAttribute>();
            if (customInspector != null && IsCustomInspectorActive())
            {
                lastGroupMinimized = DrawGroupTitle(customInspector.name, false, ref groupIndex, ref lastGroup, initializeMinimization, minStyle);
                if (!lastGroupMinimized)
                {
                    invalidate = invalidate || DrawCustomInspector();
                }
            }
            
            foreach (var field in fields)
            {
                if (field.IsConstant() || !GUIPropertiesUtils.MeetsDependencies(field, this) || GUIPropertiesUtils.IsHidden(field)) 
                    continue;
                
                TitledGroupAttribute ga = field?.GetCustomAttribute<TitledGroupAttribute>();
                lastGroupMinimized = DrawGroupTitle(ga != null ? ga.Group : "Properties", ga != null ? ga.Minimized : false, ref groupIndex, ref lastGroup, initializeMinimization, minStyle);

                if (lastGroupMinimized)
                    continue;

                GUILayout.Space(3);
                invalidate = invalidate || GUIPropertiesUtils.PropertyField(field, this, this);
            }

            return invalidate;
        }

        protected bool DrawGroupTitle(string p_title, bool p_minimizedDefault, ref int p_groupIndex, ref string p_lastGroup, bool p_initializeMinimized, GUIStyle p_style)
        {
            if (p_title != p_lastGroup)
            {
                int groupMask = 1 << p_groupIndex;
                p_groupIndex++;
                if (p_initializeMinimized && p_minimizedDefault && (groupsMinized & groupMask) == 0)
                {
                    groupsMinized += groupMask;
                }
                bool isMinimized = (groupsMinized & groupMask) != 0;
                
                GUIPropertiesUtils.Separator(20, 2, 2, new Color(0.1f, 0.1f, 0.1f));
                GUILayout.Label("   "+p_title, DashEditorCore.Skin.GetStyle("PropertyGroup"));
                Rect lastRect = GUILayoutUtility.GetLastRect();

                if (GUI.Button(new Rect(lastRect.x, lastRect.y - 25, lastRect.width, 20), "", p_style))
                {
                    groupsMinized = isMinimized ? groupsMinized - groupMask : groupsMinized + groupMask;
                }

                GUI.color = DashEditorCore.EditorConfig.theme.InspectorSectionTitleColor * 2f / 3;
                GUI.Label(new Rect(lastRect.x + 5 + (isMinimized ? 0 : 2), lastRect.y - 25, 20, 20), isMinimized ? "+" : "-", p_style);
                GUI.color = Color.white;
                    //GUI.Label(new Rect(lastRect.x + 356 + (isMinimized ? 0 : 2), lastRect.y - 25, 20, 20), isMinimized ? "+" : "-", p_style);
            
                p_lastGroup = p_title;
            }
            
            return (groupsMinized & (1 << (p_groupIndex - 1))) != 0;
        }

        protected virtual bool DrawCustomInspector()
        {
            return false;
        }

        protected virtual bool IsCustomInspectorActive()
        {
            return true;
        }

        public List<string> GetExposedGUIDs()
        {
            return GetType().GetFields().ToList().FindAll(f =>
                    f.FieldType.IsGenericType && f.FieldType.GetGenericTypeDefinition() == typeof(ExposedReference<>))
                .Select(
                    (f, i) => f.GetValue(this).GetType().GetField("exposedName").GetValue(f.GetValue(this)).ToString())
                .ToList();
        }
        
        public List<string> GetExposedNodeIDs(List<PropertyName> p_properties)
        {
            return GetType().GetFields().ToList().FindAll(f =>
                    f.FieldType.IsGenericType && f.FieldType.GetGenericTypeDefinition() == typeof(ExposedReference<>) && p_properties.Contains((PropertyName)f.GetValue(this).GetType().GetField("exposedName").GetValue(f.GetValue(this))))
                .Select(
                    (f, i) => id.ToString())
                .ToList();
        }
        
        // public void ValidateSerialization()
        // {
        //     var fields = this.GetType().GetFields();
        //     foreach (var field in fields)
        //     {
        //         if (!GUIPropertiesUtils.IsParameterProperty(field))
        //             continue;
        //         
        //         if ((Parameter)field.GetValue(this) == null)
        //         {
        //             RecreateParameter(field);
        //         }
        //     }
        // }
        
        bool RecreateParameter(FieldInfo p_fieldInfo)
        {
            var genericType = p_fieldInfo.FieldType.GenericTypeArguments[0];
            var parameterType = typeof(Parameter<>).MakeGenericType(genericType);
            var parameter = Activator.CreateInstance(parameterType, genericType.GetDefaultValue());

            p_fieldInfo.SetValue(this, parameter);

            return p_fieldInfo.GetValue(this) != null;
        }
#endif
    }
}