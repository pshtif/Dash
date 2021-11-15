/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Dash.Editor
{
    public class ExpressionsWindow : UnityEditor.EditorWindow
    {
        private Vector2 _scrollPositionScanned;
        private Vector2 _scrollPositionExplicit;
        private bool _isDirty = false;

        private bool _generateLinkXml = true;
        private bool _includeOdin = false;

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
            scrollViewStyle.normal.background = DashThemeManager.GetColorTexture(new Color(.1f, .1f, .1f));

            GUI.enabled = DashEditorCore.RuntimeConfig.enableCustomExpressionClasses;
            
            GUILayout.Space(4);
            GUILayout.Label("Expression classes", titleStyle, GUILayout.ExpandWidth(true));
            GUILayout.Label("You have "+(DashEditorCore.RuntimeConfig.expressionClasses == null ? 0 : DashEditorCore.RuntimeConfig.expressionClasses.Count)+" expression classes defined.", infoStyle, GUILayout.ExpandWidth(true));
            GUILayout.Space(2);

            _scrollPositionScanned = GUILayout.BeginScrollView(_scrollPositionScanned, scrollViewStyle,
                GUILayout.ExpandWidth(true), GUILayout.Height(rect.height - 140));
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

            if (_isDirty)
            {
                _isDirty = false;
                Repaint();
            }
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
    }
}