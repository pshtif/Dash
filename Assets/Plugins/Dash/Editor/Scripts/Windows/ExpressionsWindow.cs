/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Dash.Editor
{
    public class ExpressionsWindow : EditorWindow
    {
        private Vector2 _scrollPositionScanned;
        private Vector2 _scrollPositionExplicit;

        public static ExpressionsWindow Instance { get; private set; }
        
        [MenuItem ("Tools/Dash/Expressions")]
        public static ExpressionsWindow InitDebugWindow()
        {
            Instance = GetWindow<ExpressionsWindow>();
            Instance.titleContent = new GUIContent("Dash Expressions Editor");
            Instance.minSize = new Vector2(800, 400);

            return Instance;
        }

        private void OnGUI()
        {
            var rect = new Rect(0, 0, position.width, position.height);
            
            GUICustomUtils.DrawTitle("Dash Expressions Editor");
            
            DashEditorCore.RuntimeConfig.enableCustomExpressionClasses = GUILayout.Toggle(DashEditorCore.RuntimeConfig.enableCustomExpressionClasses, new GUIContent("Enable Custom Expression Classes"));

            var titleStyle = new GUIStyle();
            titleStyle.alignment = TextAnchor.MiddleLeft;
            titleStyle.padding.left = 5;
            titleStyle.normal.textColor = new Color(1, .5f, 0);
            titleStyle.fontStyle = FontStyle.Bold;
            titleStyle.fontSize = 16;
            
            var infoStyle = new GUIStyle();
            infoStyle.normal.textColor = Color.gray;
            infoStyle.alignment = TextAnchor.MiddleLeft;
            infoStyle.padding.left = 5;

            var scrollViewStyle = new GUIStyle();
            scrollViewStyle.normal.background = TextureUtils.GetColorTexture(new Color(.1f, .1f, .1f));

            GUI.enabled = DashEditorCore.RuntimeConfig.enableCustomExpressionClasses;
            
            GUILayout.Space(4);
            GUILayout.Label("Expression classes", titleStyle, GUILayout.ExpandWidth(true));
            GUILayout.Label("You have "+(DashEditorCore.RuntimeConfig.expressionClasses == null ? 0 : DashEditorCore.RuntimeConfig.expressionClasses.Count)+" expression classes defined.", infoStyle, GUILayout.ExpandWidth(true));
            GUILayout.Space(2);

            _scrollPositionScanned = GUILayout.BeginScrollView(_scrollPositionScanned, scrollViewStyle,
                GUILayout.ExpandWidth(true), GUILayout.Height(rect.height/2 - 140));
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
                        RemoveExpressionClass(type);
                        break;
                    }
                    GUILayout.EndHorizontal();
                }
            }
            
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
            
            GUILayout.Space(4);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Add Expression Class", GUILayout.Height(40)))
            {
                AddTypeContextMenu.ShowAsPopup(AddExpressionClass);
            }

            GUILayout.EndHorizontal();

            GUI.enabled = true;
            
            GUILayout.Space(4);
            GUILayout.Label("Expression macros", titleStyle, GUILayout.ExpandWidth(true));
            //GUILayout.Label("You have "+(DashEditorCore.RuntimeConfig.expressionClasses == null ? 0 : DashEditorCore.RuntimeConfig.expressionClasses.Count)+" expression classes defined.", infoStyle, GUILayout.ExpandWidth(true));
            GUILayout.Space(2);

            _scrollPositionScanned = GUILayout.BeginScrollView(_scrollPositionScanned, scrollViewStyle,
                GUILayout.ExpandWidth(true), GUILayout.Height(rect.height/2 - 80));
            GUILayout.BeginVertical();
            
            if (DashEditorCore.RuntimeConfig.expressionMacros != null)
            {
                foreach (var pair in DashEditorCore.RuntimeConfig.expressionMacros)
                {
                    GUILayout.BeginHorizontal();
                    string strippedName = pair.Key.Substring(1, pair.Key.Length - 2);

                    string newName = GUILayout.TextField(strippedName, GUILayout.Width(160)).ToUpper();
                    if (newName != strippedName) 
                    {
                        DashEditorCore.RuntimeConfig.expressionMacros.Remove(pair.Key);
                        DashEditorCore.RuntimeConfig.expressionMacros.Add("{"+GetUniqueName(newName)+"}", pair.Value);
                        break;
                    }
                    
                    string newValue = GUILayout.TextField(pair.Value);
                    if (newValue != pair.Value)
                    {
                        DashEditorCore.RuntimeConfig.expressionMacros[pair.Key] = newValue;
                    }

                    if (GUILayout.Button("Remove", GUILayout.Width(120)))
                    {
                        RemoveExpressionMacro(pair.Key);
                        break;
                    }
                    GUILayout.EndHorizontal();
                }
            }
            
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
            
            GUILayout.Space(4);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Add Expression Macro", GUILayout.Height(40)))
            {
                AddExpressionMacro();
            }

            GUILayout.EndHorizontal();
        }
        
        static void AddExpressionClass(object p_type)
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

        static void RemoveExpressionClass(Type p_type)
        {
            if (DashEditorCore.RuntimeConfig.expressionClasses != null)
            {
                DashEditorCore.RuntimeConfig.expressionClasses.Remove(p_type);
            }
        }

        static void AddExpressionMacro()
        {
            DashEditorCore.RuntimeConfig.expressionMacros.Add("{"+GetUniqueName("MACRO")+"}", "");
        }
        
        static void RemoveExpressionMacro(string p_name)
        {
            if (DashEditorCore.RuntimeConfig.expressionMacros != null)
            {
                DashEditorCore.RuntimeConfig.expressionMacros.Remove(p_name);
            }
        }
        
        static string GetUniqueName(string p_name)
        {
            while (DashEditorCore.RuntimeConfig.expressionMacros.ContainsKey("{"+p_name+"}")) 
            {
                string number = string.Concat(p_name.Reverse().TakeWhile(char.IsNumber).Reverse());
                p_name = p_name.Substring(0,p_name.Length-number.Length) + (string.IsNullOrEmpty(number) ? 1 : (Int32.Parse(number)+1));
            }

            return p_name;
        }
    }
}