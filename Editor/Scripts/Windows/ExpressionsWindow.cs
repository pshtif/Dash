/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
            
            DashEditorCore.RuntimeConfig.enableCustomExpressionClasses = GUILayout.Toggle(DashEditorCore.RuntimeConfig.enableCustomExpressionClasses, new GUIContent("Enable Custom Expression Classes (has small runtime impact)"));

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

            GUILayout.Space(4);
            GUILayout.Label("Expression macros", titleStyle, GUILayout.ExpandWidth(true));
            GUILayout.Label(
                "You have " +
                (DashEditorCore.RuntimeConfig.expressionMacros == null
                    ? 0
                    : DashEditorCore.RuntimeConfig.expressionMacros.Count) + " expression macros defined.", infoStyle,
                GUILayout.ExpandWidth(true));
            GUILayout.Space(2);

            _scrollPositionScanned = GUILayout.BeginScrollView(_scrollPositionScanned, scrollViewStyle,
                GUILayout.ExpandWidth(true), GUILayout.Height(rect.height - 145));
            GUILayout.BeginVertical();
            
            if (DashEditorCore.RuntimeConfig.expressionMacros != null)
            {
                foreach (var pair in DashEditorCore.RuntimeConfig.expressionMacros)
                {
                    GUILayout.BeginHorizontal();
                    string strippedName = pair.Key.Substring(1, pair.Key.Length - 2);

                    string newName = GUILayout.TextField(strippedName, GUILayout.Width(160)).ToUpper();
                    newName = Regex.Replace(newName, @"[^a-zA-Z0-9 _]", "");
                    
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
                        break;
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