/*
 *	Created by:  Peter @sHTiF Stefcek
 */

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dash.Attributes;
using OdinSerializer.Utilities;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine;
using Object = System.Object;
using TooltipAttribute = UnityEngine.TooltipAttribute;

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
        
        public static void Separator() {
            var lastRect = GUILayoutUtility.GetLastRect();
            GUILayout.Space(7);
            GUI.color = new Color(0, 0, 0, 0.3f);
            GUI.DrawTexture(Rect.MinMaxRect(lastRect.xMin, lastRect.yMax + 4, lastRect.xMax, lastRect.yMax + 6), Texture2D.whiteTexture);
            GUI.color = Color.white;
        }

        static public bool PropertyField(FieldInfo p_fieldInfo, Object p_object, IReferencable p_reference, FieldInfo p_parentInfo = null) 
            //string p_name = null, bool p_drawLabel = true)
        {
            if (IsHidden(p_fieldInfo))
                return false;
            
            if (!MeetsDependencies(p_fieldInfo, p_object))
                return false;

            FieldInfo nameInfo = p_parentInfo != null ? p_parentInfo : p_fieldInfo;
            
            LabelAttribute labelAttribute = nameInfo.GetCustomAttribute<LabelAttribute>();
            string nameString = ObjectNames.NicifyVariableName(nameInfo.Name);
            nameString = labelAttribute == null
                ? nameString.Substring(0, 1).ToUpper() + nameString.Substring(1)
                : labelAttribute.Label;
            
            TooltipAttribute tooltipAttribute = nameInfo.GetCustomAttribute<TooltipAttribute>();
            var name = tooltipAttribute == null ? new GUIContent(nameString) : new GUIContent(nameString, tooltipAttribute.tooltip);

            if (IsParameterProperty(p_fieldInfo))
                return ParameterProperty(p_fieldInfo, p_object, name, p_reference);

            if (p_parentInfo == null && IsExpressionProperty(p_fieldInfo))
                return ExpressionProperty(p_fieldInfo, p_object, name, p_reference);

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
                return ExposedReferenceProperty(p_fieldInfo, p_object, name, p_reference);

            return ValueProperty(p_fieldInfo, p_object, name, p_reference, p_parentInfo);
        }

        static bool IsPopupProperty(FieldInfo p_fieldInfo)
        {
            ClassPopupAttribute popupAttribute = p_fieldInfo.GetCustomAttribute<ClassPopupAttribute>();
            return popupAttribute != null;
        }
        
        static bool PopupProperty(FieldInfo p_fieldInfo, Object p_object, GUIContent p_name)
        {
            if (!IsPopupProperty(p_fieldInfo))
                return false;
            
            ClassPopupAttribute popupAttribute = p_fieldInfo.GetCustomAttribute<ClassPopupAttribute>();

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

            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();
            int newIndex = EditorGUILayout.Popup(p_name, index, options.ToArray());
            
            if (index != 0)
            {
                if (GUILayout.Button(IconManager.GetIcon("Script_Icon"), GUIStyle.none, GUILayout.MaxWidth(20), GUILayout.MaxHeight(20)))
                {
                    AssetDatabase.OpenAsset(EditorUtils.GetScriptFromType(cachedTypes[popupAttribute.ClassType][index-1]), 1);
                }
            }
            EditorGUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {
                if (newIndex != index)
                {
                    p_fieldInfo.SetValue(p_object,
                        newIndex == 0 ? null : Activator.CreateInstance(cachedTypes[popupAttribute.ClassType][newIndex - 1]));
                }

                return true;
            }

            return false;
        }

        static bool SupportedTypeProperty(FieldInfo p_fieldInfo, Object p_object, GUIContent p_name)
        {
            Type value = (Type)p_fieldInfo.GetValue(p_object);

            GUILayout.BeginHorizontal();
            
            GUILayout.Label(p_name, GUILayout.Width(160));
            
            if (GUILayout.Button(value == null ? "NONE" : value.Name))
            {
                VariableTypesMenu.Show((type) =>
                {
                    if (type != value)
                    {
                        p_fieldInfo.SetValue(p_object, type);
                    }    
                });
                
                return true;
            }

            GUILayout.EndHorizontal();

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
            GUILayout.Label(p_name, GUILayout.Width(160));
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
            GUILayout.Label(p_name, GUILayout.Width(160));

            var newValue = EditorGUILayout.ObjectField((UnityEngine.Object) p_fieldInfo.GetValue(p_object),
                p_fieldInfo.FieldType, false);
            
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

        static bool ExposedReferenceProperty(FieldInfo p_fieldInfo, Object p_object, GUIContent p_name, IReferencable p_reference)
        {
            if (!IsExposedReferenceProperty(p_fieldInfo))
                return false;

            IExposedPropertyTable propertyTable = DashEditorCore.EditorConfig.editingController;

            if (propertyTable == null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(p_name, GUILayout.Width(160));
                GUILayout.Label("Assets can't store references.");
                GUILayout.EndHorizontal();
                return false;
            }

            var exposedReference = p_fieldInfo.GetValue(p_object);

            PropertyName exposedName = (PropertyName)exposedReference.GetType().GetField("exposedName").GetValue(exposedReference);
            if (PropertyName.IsNullOrEmpty(exposedName))
            {
                exposedName = new PropertyName(GUID.Generate().ToString());
                exposedReference.GetType().GetField("exposedName").SetValue(exposedReference, exposedName);
            }
            
            GUILayout.BeginHorizontal();
            GUILayout.Label(p_name, GUILayout.Width(160));
            HandleReferencing(p_reference, p_fieldInfo);
            EditorGUI.BeginChangeCheck();

            UnityEngine.Object exposedValue = (UnityEngine.Object)exposedReference.GetType().GetMethod("Resolve")
                .Invoke(exposedReference, new object[] {propertyTable});
            var newValue = EditorGUILayout.ObjectField(exposedValue, p_fieldInfo.FieldType.GetGenericArguments()[0], true);

            GUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {
                if (propertyTable != null)
                {
                    Undo.RegisterCompleteObjectUndo(propertyTable as UnityEngine.Object, "Set Exposed Property");
                }
                
                if (newValue == null)
                {
                    propertyTable.ClearReferenceValue(exposedName);   
                    //exposedReference.GetType().GetField("exposedName").SetValue(exposedReference, null);
                    p_fieldInfo.SetValue(p_object, exposedReference);
                }
                else
                {
                    propertyTable.SetReferenceValue(exposedName, newValue);
                    p_fieldInfo.SetValue(p_object, exposedReference);
                }

                return true;
            }

            return false;
        }

        public static bool IsParameterProperty(FieldInfo p_fieldInfo)
        {
            return typeof(Parameter).IsAssignableFrom(p_fieldInfo.FieldType);
        }

        static bool ParameterProperty(FieldInfo p_fieldInfo, Object p_object, GUIContent p_name, IReferencable p_reference)
        {
            if (!IsParameterProperty(p_fieldInfo))
                return false;

            Parameter param = (Parameter)p_fieldInfo.GetValue(p_object);

            if (param == null)
            {
                RecreateParameter(p_fieldInfo, p_object);
                return true;
            }
            
            // Can happen due to serialization/migration error
            if (param != null)
            {
                EditorGUI.BeginChangeCheck();
                
                EditorGUILayout.BeginHorizontal();
                
                if (param.isExpression)
                {
                    GUILayout.BeginHorizontal();
                    GUI.color = DashEditorCore.EditorConfig.theme.ParameterColor;
                    GUILayout.Label(p_name, GUILayout.Width(160));
                    HandleReferencing(p_reference, p_fieldInfo, false, param);
                    param.expression = GUILayout.TextArea(param.expression, GUILayout.ExpandWidth(true));

                    GUI.color = Color.white;
                    GUILayout.EndHorizontal();
                }
                else
                {
                    ButtonAttribute button = p_fieldInfo.GetAttribute<ButtonAttribute>();
                    if (button != null)
                    {
                        GUILayout.Label(p_name, GUILayout.Width(160));
                        if (param.IsDefault())
                        {
                            GUI.color = Color.yellow;
                            if (GUILayout.Button(button.NullLabel))
                            {
                                MethodInfo method = p_object.GetType().GetMethod(button.MethodName, BindingFlags.Instance | BindingFlags.NonPublic);
                                param.GetValueFieldInfo().SetValue(param, method.Invoke(p_object, null));
                            }
                            GUI.color = Color.white;
                        }
                        else
                        {
                            if (GUILayout.Button(button.NonNullLabel))
                            {
                                MethodInfo method = p_object.GetType().GetMethod(button.MethodName, BindingFlags.Instance | BindingFlags.NonPublic);
                                param.GetValueFieldInfo().SetValue(param, method.Invoke(p_object, null));
                            }
                        }
                    } else {
                        PropertyField(param.GetValueFieldInfo(), param, p_reference, p_fieldInfo);
                    }
                }

                
                GUI.color = param.isExpression ? DashEditorCore.EditorConfig.theme.ParameterColor : Color.gray;
                if (GUILayout.Button(IconManager.GetIcon("Settings_Icon"), GUIStyle.none, GUILayout.Height(16), GUILayout.MaxWidth(16)))
                {
                    ParameterMenu.Show(param);
                }
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
        
        static void RecreateParameter(FieldInfo p_fieldInfo, Object p_object)
        {
            var genericType = p_fieldInfo.FieldType.GenericTypeArguments[0];
            var parameterType = typeof(Parameter<>).MakeGenericType(genericType);
            var parameter = Activator.CreateInstance(parameterType, genericType.GetDefaultValue());

            p_fieldInfo.SetValue(p_object, parameter);
        }

        public static bool IsExpressionProperty(FieldInfo p_fieldInfo)
        {
            return p_fieldInfo.GetAttribute<ExpressionAttribute>() != null;
        }
        
        static bool ExpressionProperty(FieldInfo p_fieldInfo, Object p_object, GUIContent p_name, IReferencable p_reference)
        {
            ExpressionAttribute expressionAttribute = p_fieldInfo.GetAttribute<ExpressionAttribute>();
            
            if (expressionAttribute == null)
                return false;

            FieldInfo expressionField = p_object.GetType().GetField(expressionAttribute.expression);
            FieldInfo useExpressionField = p_object.GetType().GetField(expressionAttribute.useExpression);
            
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            
            if ((bool)useExpressionField.GetValue(p_object))
            {
                GUILayout.BeginHorizontal();
                GUI.color = DashEditorCore.EditorConfig.theme.ParameterColor;
                GUILayout.Label(p_name, GUILayout.Width(160));
                HandleReferencing(p_reference, expressionField, true);
                string expression = GUILayout.TextArea((string)expressionField.GetValue(p_object), GUILayout.ExpandWidth(true));
                GUI.color = Color.white;
                expressionField.SetValue(p_object, expression);
                GUILayout.EndHorizontal();
            }
            else
            {
                PropertyField(p_fieldInfo, p_object, p_reference, p_fieldInfo);
            }

            bool useExpression = (bool)useExpressionField.GetValue(p_object);
            GUI.color = useExpression ? DashEditorCore.EditorConfig.theme.ParameterColor : Color.gray;
            if (GUILayout.Button(IconManager.GetIcon("Settings_Icon"), GUIStyle.none, GUILayout.Height(16), GUILayout.MaxWidth(16)))
            {
                ParameterMenu.Show(useExpressionField, expressionField, p_object, p_fieldInfo.GetReturnType());
            }
            GUI.color = Color.white;
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(4);

            if (EditorGUI.EndChangeCheck())
            {
                return true;
            }

            return false;
        }

        static bool ValueProperty(FieldInfo p_fieldInfo, Object p_object, GUIContent p_name, IReferencable p_reference, FieldInfo p_parameterInfo = null)
        {
            FieldInfo referenceInfo = p_parameterInfo != null ? p_parameterInfo : p_fieldInfo;

            string type = p_fieldInfo.FieldType.ToString();
            switch (type)
            {
                case "System.String":
                    EditorGUI.BeginChangeCheck();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(p_name, GUILayout.Width(160));
                    HandleReferencing(p_reference, referenceInfo, false, p_parameterInfo == null ? null : (Parameter)p_object);
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
                    GUILayout.Label(p_name,  GUILayout.Width(160));
                    HandleReferencing(p_reference, referenceInfo, false, p_parameterInfo == null ? null : (Parameter)p_object);
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
                    GUILayout.Label(p_name,  GUILayout.Width(160));
                    HandleReferencing(p_reference, referenceInfo, false, p_parameterInfo == null ? null : (Parameter)p_object);
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
                    GUILayout.Label(p_name, GUILayout.Width(160));
                    HandleReferencing(p_reference, referenceInfo, false, p_parameterInfo == null ? null : (Parameter)p_object);
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
                    HandleReferencing(p_reference, referenceInfo, false, p_parameterInfo == null ? null : (Parameter)p_object);
                    if (EditorGUI.EndChangeCheck())
                    {
                        p_fieldInfo.SetValue(p_object, vector2Value);
                        return true;
                    }

                    return false;
                case "UnityEngine.Vector3":
                    EditorGUI.BeginChangeCheck();
                    var vector3Value = EditorGUILayout.Vector3Field(p_name, (Vector3) p_fieldInfo.GetValue(p_object));
                    HandleReferencing(p_reference, referenceInfo, false, p_parameterInfo == null ? null : (Parameter)p_object);
                    if (EditorGUI.EndChangeCheck())
                    {
                        p_fieldInfo.SetValue(p_object, vector3Value);
                        return true;
                    }

                    return false;
                case "UnityEngine.Vector4":
                    EditorGUI.BeginChangeCheck();
                    var vector4Value = EditorGUILayout.Vector4Field(p_name, (Vector3) p_fieldInfo.GetValue(p_object));
                    HandleReferencing(p_reference, referenceInfo, false, p_parameterInfo == null ? null : (Parameter)p_object);
                    if (EditorGUI.EndChangeCheck())
                    {
                        p_fieldInfo.SetValue(p_object, vector4Value);
                        return true;
                    }

                    return false;
                case "UnityEngine.Color":
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(p_name, GUILayout.Width(160));
                    EditorGUI.BeginChangeCheck();
                    var colorValue = EditorGUILayout.ColorField("", (Color) p_fieldInfo.GetValue(p_object), GUILayout.Width(60));
                    HandleReferencing(p_reference, referenceInfo, false, p_parameterInfo == null ? null : (Parameter)p_object);
                    GUILayout.EndHorizontal();
                    if (EditorGUI.EndChangeCheck())
                    {
                        p_fieldInfo.SetValue(p_object, colorValue);
                        return true;
                    }
                    return false;
                default:
                    Debug.Log(type + " type inspection not implemented. Field: " + p_fieldInfo.Name);
                    return false;
            }
        }
        
        protected static void HandleReferencing(IReferencable p_reference, FieldInfo p_fieldInfo, bool p_directExpression = false, Parameter p_parameter = null)
        {
            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) &&
                Event.current.button == 1 && Event.current.type == EventType.MouseDown)
            {
                RuntimeGenericMenu menu = new RuntimeGenericMenu();
                
                menu.AddItem(new GUIContent("Copy reference"), false,
                    () =>
                    {
                        DashEditorCore.propertyReference = "[$" + p_reference.Id + "." +
                                                           p_fieldInfo.Name + "]";
                    });
                
                if (p_parameter != null && !string.IsNullOrEmpty(DashEditorCore.propertyReference))
                {
                    menu.AddItem(new GUIContent("Paste reference"), false,
                        () =>
                        {
                            p_parameter.isExpression = true;
                            p_parameter.expression = DashEditorCore.propertyReference;
                        });
                }
                
                if (p_directExpression && !string.IsNullOrEmpty(DashEditorCore.propertyReference))
                {
                    menu.AddItem(new GUIContent("Paste reference"), false,
                        () => { p_fieldInfo.SetValue(p_reference, DashEditorCore.propertyReference); });
                }

                //menu.ShowAsContext();
                GenericMenuPopup.Show(menu, "",  Event.current.mousePosition, 240, 300, false, false);
            }
        }
        
        static public bool MeetsDependencies(FieldInfo p_fieldInfo, Object p_object)
        {
            IEnumerable<DependencyAttribute> attributes = p_fieldInfo.GetCustomAttributes<DependencyAttribute>();
            bool meetsAllDependencies = true;
            foreach (DependencyAttribute attribute in attributes)
            {
                FieldInfo dependencyField = p_object.GetType().GetField(attribute.DependencyName);
                if (dependencyField == null)
                {
                    Debug.LogWarning("Dependency upon nonexistent property: "+attribute.DependencyName+" on "+p_object.GetType());
                    return false;
                }

                if (typeof(Parameter).IsAssignableFrom(dependencyField.FieldType))
                {
                    Parameter<bool> dependencyParameter = dependencyField.GetValue(p_object) as Parameter<bool>;
                    meetsAllDependencies = meetsAllDependencies && (dependencyParameter.isExpression || dependencyParameter.GetValue(null));
                }
                else if (dependencyField != null &&
                         attribute.Value.ToString() != dependencyField.GetValue(p_object).ToString())
                {
                    return false;
                }
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

            return meetsAllDependencies;
        }

        static public bool IsHidden(FieldInfo p_fieldInfo)
        {
            HideInInspector hideInInspectorAttribute = p_fieldInfo.GetCustomAttribute<HideInInspector>();
            return hideInInspectorAttribute != null;
        }
        
        static public int GroupSort(FieldInfo p_field1, FieldInfo p_field2)
        {
            TitledGroupAttribute attribute1 = p_field1.GetCustomAttribute<TitledGroupAttribute>();
            TitledGroupAttribute attribute2 = p_field2.GetCustomAttribute<TitledGroupAttribute>();
            if (attribute1 == null && attribute2 == null)
                return OrderSort(p_field1, p_field2);

            if (attribute1 != null && attribute2 == null)
                return attribute1.Order > 0 ? 1 : -1;

            if (attribute1 == null && attribute2 != null)
                return attribute2.Order > 0 ? -1 : 1;

            if (attribute1.Group == attribute2.Group)
                return OrderSort(p_field1, p_field2);
            
            if (attribute1.Order != attribute2.Order) 
                return attribute1.Order.CompareTo(attribute2.Order);
            
            return attribute1.Group.CompareTo(attribute2.Group);
        }
        
        static public int OrderSort(FieldInfo p_field1, FieldInfo p_field2)
        {
            OrderAttribute attribute1 = p_field1.GetCustomAttribute<OrderAttribute>();
            OrderAttribute attribute2 = p_field2.GetCustomAttribute<OrderAttribute>();
            
            if (attribute1 == null && attribute2 == null)
                return 0;

            if (attribute1 != null && attribute2 == null)
                return attribute1.Order > 0 ? 1 : -1;
            
            if (attribute1 == null && attribute2 != null)
                return attribute2.Order > 0 ? -1 : 1;

            return attribute1.Order.CompareTo(attribute2.Order);
        }
    }
}
#endif