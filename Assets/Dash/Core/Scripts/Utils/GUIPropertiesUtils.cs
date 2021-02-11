/*
 *	Created by:  Peter @sHTiF Stefcek
 */

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dash.Attributes;
using UnityEditor;
using UnityEngine;
using Object = System.Object;

namespace Dash
{
    public class GUIPropertiesUtils
    {
        private static Dictionary<Type, List<Type>> cachedTypes = new Dictionary<Type, List<Type>>();
        
        static public void Separator(int p_thickness, int p_paddingTop, int p_paddingBottom, Color p_color)
        {
            Rect rect = EditorGUILayout.GetControlRect(GUILayout.Height(p_paddingTop+p_paddingBottom+p_thickness));
            rect.height = p_thickness;
            rect.y+=p_paddingTop;
            rect.x-=2;
            rect.width +=6;
            EditorGUI.DrawRect(rect, p_color);
        }

        static public bool PropertyField(FieldInfo p_fieldInfo, Object p_object, string p_name = null, bool p_drawLabel = true)
        {
            HideInInspector hideInInspectorAttribute = p_fieldInfo.GetCustomAttribute<HideInInspector>();
            if (hideInInspectorAttribute != null)
                return false;
            
            if (!MeetsDependencies(p_fieldInfo, p_object))
                return false;
            
            string nameString = String.IsNullOrEmpty(p_name) ? p_fieldInfo.Name : p_name;
            nameString = nameString.Substring(0, 1).ToUpper() + nameString.Substring(1);

            TooltipAttribute tooltipAttribute = p_fieldInfo.GetCustomAttribute<TooltipAttribute>();
            var name = tooltipAttribute == null ? new GUIContent(nameString) : new GUIContent(nameString, tooltipAttribute.tooltip);

            if (IsPopupProperty(p_fieldInfo))
                return PopupProperty(p_fieldInfo, p_object, name);

            if (p_fieldInfo.FieldType == typeof(Type))
            {
                return SupportedTypeProperty(p_fieldInfo, p_object, name);
            }
            
            if (IsEnumProperty(p_fieldInfo))
                return EnumProperty(p_fieldInfo, p_object, name);

            if (IsUnityObjectProperty(p_fieldInfo))
                return UnityObjectProperty(p_fieldInfo, p_object, name);

            if (IsExposedReferenceProperty(p_fieldInfo))
                return ExposedReferenceProperty(p_fieldInfo, p_object, name, p_drawLabel);

            if (IsParameterProperty(p_fieldInfo))
                return ParameterProperty(p_fieldInfo, p_object, name);

            return ValueProperty(p_fieldInfo, p_object, name);
        }

        static bool IsPopupProperty(FieldInfo p_fieldInfo)
        {
            PopupAttribute popupAttribute = p_fieldInfo.GetCustomAttribute<PopupAttribute>();
            return popupAttribute != null;
        }
        
        static bool PopupProperty(FieldInfo p_fieldInfo, Object p_object, GUIContent p_name)
        {
            if (!IsPopupProperty(p_fieldInfo))
                return false;
            
            PopupAttribute popupAttribute = p_fieldInfo.GetCustomAttribute<PopupAttribute>();

            // We are caching assembly domain lookups as it is heavy operation
            // TODO need to enable option to recache later since users can implement new types
            if (!cachedTypes.ContainsKey(popupAttribute.ClassType))
            {
                cachedTypes[popupAttribute.ClassType] = ReflectionUtils.GetAllTypes(popupAttribute.ClassType);
            }

            List<string> options = cachedTypes[popupAttribute.ClassType].Select(c => c.ToString()).ToList();
            options.Insert(0, "NONE");
            object value = p_fieldInfo.GetValue(p_object);
            int index = value == null ? 0 : options.IndexOf(value.ToString());

            EditorGUI.BeginChangeCheck();
            
            int newIndex = EditorGUILayout.Popup(p_name, index, options.ToArray());

            if (EditorGUI.EndChangeCheck())
            {
                if (newIndex != index)
                {
                    p_fieldInfo.SetValue(p_object,
                        index == 0 ? null : Activator.CreateInstance(cachedTypes[popupAttribute.ClassType][index - 1]));
                }

                return true;
            }

            return false;
        }

        static bool SupportedTypeProperty(FieldInfo p_fieldInfo, Object p_object, GUIContent p_name)
        {
            Type[] options = Variable.SupportedTypes;
            Type value = (Type)p_fieldInfo.GetValue(p_object);
            int index = value == null ? 0 : Array.IndexOf(options, value);

            EditorGUI.BeginChangeCheck();
            
            int newIndex = EditorGUILayout.Popup(p_name, index, options.Select(t => Variable.ConvertToTypeName(t)).ToArray());

            if (EditorGUI.EndChangeCheck())
            {
                if (newIndex != index)
                {
                    p_fieldInfo.SetValue(p_object, options[newIndex]);
                }

                return true;
            }

            return false;
        }

        static bool IsEnumProperty(FieldInfo p_fieldInfo)
        {
            return p_fieldInfo.FieldType.IsEnum;
        }

        static bool EnumProperty(FieldInfo p_fieldInfo, Object p_object, GUIContent p_name)
        {
            if (!IsEnumProperty(p_fieldInfo))
                return false;
            
            EditorGUI.BeginChangeCheck();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label(p_name, GUILayout.Width(120));
            var newValue = EditorGUILayout.EnumPopup((Enum) p_fieldInfo.GetValue(p_object));
            GUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {
                p_fieldInfo.SetValue(p_object, newValue);
                return true;
            }

            return false;
        }

        static bool IsUnityObjectProperty(FieldInfo p_fieldInfo)
        {
            return typeof(UnityEngine.Object).IsAssignableFrom(p_fieldInfo.FieldType);
        }
        
        static bool UnityObjectProperty(FieldInfo p_fieldInfo, Object p_object, GUIContent p_name)
        {
            if (!IsUnityObjectProperty(p_fieldInfo))
                return false;
            
            EditorGUI.BeginChangeCheck();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label(p_name, GUILayout.Width(120));

            var newValue = EditorGUILayout.ObjectField((UnityEngine.Object) p_fieldInfo.GetValue(p_object),
                p_fieldInfo.FieldType, false, GUILayout.Width(198));
            
            GUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {
                p_fieldInfo.SetValue(p_object, newValue);
                return true;
            }

            return false;
        }

        static bool IsExposedReferenceProperty(FieldInfo p_fieldInfo)
        {
            return p_fieldInfo.FieldType.IsGenericType &&
                   p_fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(ExposedReference<>);
        }

        static bool ExposedReferenceProperty(FieldInfo p_fieldInfo, Object p_object, GUIContent p_name, bool p_drawLabel)
        {
            if (!IsExposedReferenceProperty(p_fieldInfo))
                return false;
            
            IExposedPropertyTable propertyTable = DashEditorCore.Config.editingGraph.Controller;
            var exposedReference = p_fieldInfo.GetValue(p_object);
            
            PropertyName exposedName = (PropertyName)exposedReference.GetType().GetField("exposedName").GetValue(exposedReference);
            bool isDefault = PropertyName.IsNullOrEmpty(exposedName);
            
            if (p_drawLabel)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(p_name, GUILayout.Width(120));   
            }
            
            EditorGUI.BeginChangeCheck();

            UnityEngine.Object exposedValue = (UnityEngine.Object)exposedReference.GetType().GetMethod("Resolve")
                .Invoke(exposedReference, new object[] {propertyTable});
            var newValue = EditorGUILayout.ObjectField(exposedValue, p_fieldInfo.FieldType.GetGenericArguments()[0], true);
            
            if (p_drawLabel)
            {
                GUILayout.EndHorizontal();
            }

            if (EditorGUI.EndChangeCheck())
            {
                if (propertyTable != null)
                {
                    Undo.RecordObject(propertyTable as UnityEngine.Object, "Set Exposed Property");
                }

                if (!isDefault)
                {
                    if (newValue == null)
                    {
                        propertyTable.ClearReferenceValue(exposedName);   
                        exposedReference.GetType().GetField("exposedName").SetValue(exposedReference, null);
                        p_fieldInfo.SetValue(p_object, exposedReference);
                    }
                    else
                    {
                        propertyTable.SetReferenceValue(exposedName, newValue);
                    }
                }
                else
                {
                    if (newValue != null)
                    {
                        PropertyName newExposedName = new PropertyName(GUID.Generate().ToString());
                        exposedReference.GetType().GetField("exposedName")
                            .SetValue(exposedReference, newExposedName);
                        propertyTable.SetReferenceValue(newExposedName, newValue);
                        p_fieldInfo.SetValue(p_object, exposedReference);
                    }
                }

                return true;
            }

            return false;
        }

        public static bool IsParameterProperty(FieldInfo p_fieldInfo)
        {
            return typeof(Parameter).IsAssignableFrom(p_fieldInfo.FieldType);
        }

        static bool ParameterProperty(FieldInfo p_fieldInfo, Object p_object, GUIContent p_name)
        {
            if (!IsParameterProperty(p_fieldInfo))
                return false;

            Parameter param = (Parameter)p_fieldInfo.GetValue(p_object);
            
            // Can happen due to serialization/migration error
            if (param != null)
            {
                EditorGUI.BeginChangeCheck();
                
                EditorGUILayout.BeginHorizontal();
                
                if (param.isExpression)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(p_name, GUILayout.Width(120));
                    //param.expression = GUILayout.TextField(param.expression, GUILayout.Width(160));
                    param.expression = GUILayout.TextArea(param.expression, GUILayout.Width(170));
                    GUILayout.EndHorizontal();
                }
                else
                {
                    PropertyField(param.GetValueFieldInfo(), param, p_name.text);
                }

                
                GUI.color = param.isExpression ? Color.yellow : Color.gray;
                
                if (GUILayout.Button(IconManager.GetIcon("Settings_Icon"), GUIStyle.none, GUILayout.Height(16), GUILayout.MaxWidth(16)))
                {
                    param.isExpression = !param.isExpression;
                }
                //param.isExpression = GUILayout.Toggle(param.isExpression, "", GUILayout.MaxWidth(14));
                GUI.color = Color.white;
                
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.Space(4);

                if (EditorGUI.EndChangeCheck())
                {
                    return true;
                }
            }
            else
            {
                GUI.color = Color.red;
                GUILayout.Label("Serialization error on " + p_fieldInfo.Name+"\nYou can use SerializationInvalidation in menu to try fix this.");
                EditorGUILayout.Space(2);
                GUI.color = Color.white;
            }

            return false;
        }

        static bool ValueProperty(FieldInfo p_fieldInfo, Object p_object, GUIContent p_name)
        {
            string type = p_fieldInfo.FieldType.ToString();
            switch (type)
            {
                case "System.String":
                    EditorGUI.BeginChangeCheck();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(p_name, GUILayout.Width(120));
                    var stringValue = GUILayout.TextField((String) p_fieldInfo.GetValue(p_object));
                    GUILayout.EndHorizontal();

                    if (EditorGUI.EndChangeCheck())
                    {
                        p_fieldInfo.SetValue(p_object, stringValue);
                        return true;
                    }

                    // // Looking for hierarchy objects
                    // if (inspectorAttribute != null && inspectorAttribute.isGameObject)
                    // {
                    //     if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                    //     {
                    //         if (Event.current.type == EventType.DragExited)
                    //         {
                    //             DragAndDrop.PrepareStartDrag(); 
                    //         }
                    //         
                    //         switch (Event.current.type) 
                    //         {
                    //             case EventType.DragUpdated:
                    //                  DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                    //                  //else DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                    //                 
                    //                  Event.current.Use();
                    //                  break;
                    //             case EventType.DragPerform:
                    //                 DragAndDrop.AcceptDrag();
                    //                
                    //                 Debug.Log(DragAndDrop.objectReferences.Length);
                    //                 Debug.Log(DragAndDrop.objectReferences[0]);
                    //
                    //                 Event.current.Use();
                    //                 break;
                    //             case EventType.MouseUp:
                    //                 // Clean up, in case MouseDrag never occurred:
                    //                 DragAndDrop.PrepareStartDrag();
                    //                 break;
                    //         }
                    //     }
                    // }
                    return false;
                case "System.Int32":
                    EditorGUI.BeginChangeCheck();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(p_name,  GUILayout.Width(120));
                    var intValue = EditorGUILayout.IntField((int) p_fieldInfo.GetValue(p_object));
                    GUILayout.EndHorizontal();

                    if (EditorGUI.EndChangeCheck())
                    {
                        p_fieldInfo.SetValue(p_object, intValue);
                        return true;
                    }

                    return false;
                case "System.Single":
                    EditorGUI.BeginChangeCheck();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(p_name,  GUILayout.Width(120));
                    var singleValue = EditorGUILayout.FloatField((float) p_fieldInfo.GetValue(p_object));
                    GUILayout.EndHorizontal();

                    if (EditorGUI.EndChangeCheck())
                    {
                        p_fieldInfo.SetValue(p_object, singleValue);
                        return true;
                    }
                    
                    return false;
                case "System.Boolean":
                    EditorGUI.BeginChangeCheck();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(p_name, GUILayout.Width(120));
                    var boolValue = EditorGUILayout.Toggle((bool) p_fieldInfo.GetValue(p_object));
                    GUILayout.EndHorizontal();

                    if (EditorGUI.EndChangeCheck())
                    {
                        p_fieldInfo.SetValue(p_object, boolValue);
                        return true;
                    }

                    return false;
                case "UnityEngine.Vector2":
                    EditorGUI.BeginChangeCheck();
                    var vector2Value = EditorGUILayout.Vector2Field(p_name, (Vector2) p_fieldInfo.GetValue(p_object));

                    if (EditorGUI.EndChangeCheck())
                    {
                        p_fieldInfo.SetValue(p_object, vector2Value);
                        return true;
                    }

                    return false;
                case "UnityEngine.Vector3":
                    EditorGUI.BeginChangeCheck();
                    var vector3Value = EditorGUILayout.Vector3Field(p_name, (Vector3) p_fieldInfo.GetValue(p_object));

                    if (EditorGUI.EndChangeCheck())
                    {
                        p_fieldInfo.SetValue(p_object, vector3Value);
                        return true;
                    }

                    return false;
                case "UnityEngine.Vector4":
                    EditorGUI.BeginChangeCheck();
                    var vector4Value = EditorGUILayout.Vector4Field(p_name, (Vector3) p_fieldInfo.GetValue(p_object));

                    if (EditorGUI.EndChangeCheck())
                    {
                        p_fieldInfo.SetValue(p_object, vector4Value);
                        return true;
                    }

                    return false;
                case "UnityEngine.Color":
                    EditorGUI.BeginChangeCheck();
                    var colorValue = EditorGUILayout.ColorField(p_name, (Color) p_fieldInfo.GetValue(p_object));

                    if (EditorGUI.EndChangeCheck())
                    {
                        p_fieldInfo.SetValue(p_object, colorValue);
                        return true;
                    }

                    return false;
                default:
                    Debug.Log(p_fieldInfo.FieldType+" : "+(p_fieldInfo.FieldType == typeof(ExposedReference<>)));
                    Debug.Log(type + " type inspection not implemented. Field: " + p_fieldInfo.Name);
                    return false;
            }
        }
        
        static bool MeetsDependencies(FieldInfo p_fieldInfo, Object p_object)
        {
            IEnumerable<DependencyAttribute> attributes = p_fieldInfo.GetCustomAttributes<DependencyAttribute>();
            foreach (DependencyAttribute attribute in attributes)
            {
                FieldInfo dependencyField = p_object.GetType().GetField(attribute.DependencyName);
                if (dependencyField != null && attribute.Value.ToString() != dependencyField.GetValue(p_object).ToString())
                    return false;
            }

            bool single = false;
            IEnumerable<DependencySingleAttribute> singleAttributes = p_fieldInfo.GetCustomAttributes<DependencySingleAttribute>();
            foreach (DependencySingleAttribute attribute in singleAttributes)
            {
                FieldInfo dependencyField = p_object.GetType().GetField(attribute.DependencyName);
                if (dependencyField != null)
                {
                    single = single || attribute.Value.ToString() == dependencyField.GetValue(p_object).ToString();
                }
            }

            if (!single && singleAttributes.Count() > 0)
                return false;

            return true;
        }
    }
}
#endif