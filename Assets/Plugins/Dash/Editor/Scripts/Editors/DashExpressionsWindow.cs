/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using OdinSerializer.Utilities;
using UnityEditor;
using UnityEngine;

namespace Dash.Editor
{
    public class DashExpressionsWindow : EditorWindow
    {
        public static DashExpressionsWindow Instance { get; private set; }

        [MenuItem ("Tools/Dash/Expressions")]
        public static DashExpressionsWindow InitDebugWindow()
        {
            Instance = GetWindow<DashExpressionsWindow>();
            Instance.titleContent = new GUIContent("Dash Expressions Editor");

            return Instance;
        }

        private Vector2 _scrollPositionMacros;
        private Vector2 _scrollPositionClasses;
        
        private void OnEnable()
        {
          
        }
        
        private void OnDisable()
        {
            
        }
        

        private void OnGUI()
        {
            var rect = new Rect(0, 0, position.width, position.height);

            var scrollViewStyle = new GUIStyle();
            scrollViewStyle.normal.background = DashEditorCore.GetColorTexture(new Color(.1f, .1f, .1f));
            
            // _scrollPositionMacros = GUILayout.BeginScrollView(_scrollPositionMacros, scrollViewStyle, GUILayout.ExpandWidth(true), GUILayout.Height(rect.height/2-45));
            //
            // GUILayout.BeginVertical();
            //
            // if (DashEditorCore.RuntimeConfig.expressionMacros != null)
            // {
            //     foreach (var pair in DashEditorCore.RuntimeConfig.expressionMacros)
            //     {
            //         GUILayout.BeginHorizontal();
            //         string newKey = GUILayout.TextField(pair.Key);
            //         string newValue = GUILayout.TextField(pair.Value);
            //         if (GUILayout.Button("Remove", GUILayout.Width(120)))
            //         {
            //             DashEditorCore.RuntimeConfig.expressionMacros.Remove(pair.Key);
            //             break;
            //         }
            //         
            //         if (newKey != pair.Key)
            //         {
            //             // If key changed we need to remove/readd the value to new key
            //             DashEditorCore.RuntimeConfig.expressionMacros.Remove(pair.Key);
            //             DashEditorCore.RuntimeConfig.expressionMacros.Add(newKey, newValue);
            //             break;
            //         } 
            //         if (newValue != pair.Value)
            //         {
            //             DashEditorCore.RuntimeConfig.expressionMacros[newKey] = newValue;
            //             break;
            //         }
            //         GUILayout.EndHorizontal();
            //     }
            // }
            //
            // GUILayout.EndVertical();
            // GUILayout.EndScrollView();
            //
            // if (GUILayout.Button("Create New Macro", GUILayout.Height(40)))
            // {
            //     AddMacro();
            // }
            
            _scrollPositionClasses = GUILayout.BeginScrollView(_scrollPositionClasses, scrollViewStyle, GUILayout.ExpandWidth(true), GUILayout.Height(rect.height-45));
            
            GUILayout.BeginVertical();
            
            if (DashEditorCore.RuntimeConfig.expressionClasses != null)
            {
                foreach (Type type in DashEditorCore.RuntimeConfig.expressionClasses)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label((string.IsNullOrEmpty(type.Namespace) ? "" : type.Namespace + ".") +
                                    type.GetReadableTypeName());
                    if (GUILayout.Button("Remove", GUILayout.Width(120)))
                    {
                        DashEditorCore.RuntimeConfig.expressionClasses.Remove(type);
                        break;
                    }
                    GUILayout.EndHorizontal();
                }
            }
            
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
            
            if (GUILayout.Button("Add Expression Class", GUILayout.Height(40)))
            {
                AddTypeContextMenu.ShowAsPopup(AddClass);
            }
        }
        
        static void AddClass(object p_type)
        {
            if (DashEditorCore.RuntimeConfig.expressionClasses == null)
            {
                DashEditorCore.RuntimeConfig.expressionClasses = new List<Type>();
            }

            if (!DashEditorCore.RuntimeConfig.expressionClasses.Contains((Type)p_type))
            {
                DashEditorCore.RuntimeConfig.expressionClasses.Add((Type)p_type);
            }
            
            EditorUtility.SetDirty(DashEditorCore.RuntimeConfig);
        }

        // static void AddMacro()
        // {
        //     if (DashEditorCore.RuntimeConfig.expressionMacros == null)
        //     {
        //         DashEditorCore.RuntimeConfig.expressionMacros = new Dictionary<string, string>();
        //     }
        //     
        //     DashEditorCore.RuntimeConfig.expressionMacros.Add("newMacro", "");
        // }

        private void OnInspectorUpdate()
        {
            Repaint();
        }
    }
}