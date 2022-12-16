/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using OdinSerializer.Utilities;
using UnityEditor;
using UnityEngine;

namespace Dash.Editor
{
    public class ExecutionDebugWindow : UnityEditor.EditorWindow
    {
        private String _search = "";
        private Vector2 _scrollPosition;
        private bool _isDirty = false;

        private bool _showTime = true;
        private bool _forceLowerCase = true;
        
        public static ExecutionDebugWindow Instance { get; private set; }
        
        public static void Init()
        {
            Instance = GetWindow<ExecutionDebugWindow>();
            Instance.titleContent = new GUIContent("Dash Execution Debug");
            Instance.minSize = new Vector2(800, 400);
        }
        
        private void OnEnable()
        {
            DashEditorDebug.OnDebugUpdate -= OnDebugUpdate;
            DashEditorDebug.OnDebugUpdate += OnDebugUpdate;
        }
        
        private void OnDisable()
        {
            DashEditorDebug.OnDebugUpdate -= OnDebugUpdate;
        }

        private void OnDebugUpdate()
        {
            _isDirty = true;
        }

        private void OnGUI()
        {
            var rect = new Rect(0, 0, position.width, position.height);
            
            GUIEditorUtils.DrawTitle("Dash Execution Debug");

            GUI.backgroundColor = Color.white;
            
            GUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Clear", GUILayout.Width(120)))
            {
                DashEditorDebug.DebugList?.Clear();
            }
            
            EditorGUIUtility.labelWidth = 35;
            _search = EditorGUILayout.TextField(new GUIContent("Filter:"), _search, GUILayout.ExpandWidth(true));
            GUILayout.Space(4);
            _forceLowerCase = GUILayout.Toggle(_forceLowerCase, new GUIContent("Lower Case"), GUILayout.Width(100));
            GUILayout.Space(8);
            EditorGUIUtility.labelWidth = 55;
            DashEditorCore.EditorConfig.maxLog = EditorGUILayout.IntField(new GUIContent("Max Log:"), DashEditorCore.EditorConfig.maxLog, GUILayout.Width(120));
            GUILayout.Space(4);
            _showTime = GUILayout.Toggle(_showTime, new GUIContent("Show Time"), GUILayout.Width(100));
            // GUILayout.Space(4);
            // _showController = GUILayout.Toggle(_showController, new GUIContent("Controller"), GUILayout.ExpandWidth(false));
            // GUILayout.Space(4);
            // _showGraph = GUILayout.Toggle(_showGraph, new GUIContent("GraphPath"), GUILayout.ExpandWidth(false));
            // GUILayout.Space(4);
            // _showNode = GUILayout.Toggle(_showNode, new GUIContent("NodeId"), GUILayout.ExpandWidth(false));
            // GUILayout.Space(4);
            // _showTarget = GUILayout.Toggle(_showTarget, new GUIContent("Target"), GUILayout.ExpandWidth(false));
            GUILayout.EndHorizontal();
            
            GUILayout.BeginArea(new Rect(rect.x, rect.y+60, rect.width, rect.height-60));
            if (_isDirty)
            {
                _scrollPosition = new Vector2(0, int.MaxValue);
            }

            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, false);
            GUILayout.BeginVertical();
            
            if (DashEditorDebug.DebugList != null)
            {
                int start = DashEditorCore.EditorConfig.maxLog < DashEditorDebug.DebugList.Count ? DashEditorDebug.DebugList.Count - DashEditorCore.EditorConfig.maxLog : 0;
                for (int i = start; i<DashEditorDebug.DebugList.Count; i++)
                {
                    var debug = DashEditorDebug.DebugList[i];

                    bool found = false;
                    if (!string.IsNullOrWhiteSpace(_search))
                    {
                        found = found || debug.Search(_forceLowerCase ? _search.ToLower() : _search, _forceLowerCase);
                        
                        if (!found)
                            continue;
                    }
                    
                    GUILayout.BeginHorizontal();
                    
                    debug.Draw(_showTime);
                    
                    GUILayout.Space(4);
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.EndVertical();
            GUILayout.EndScrollView();
            GUILayout.EndArea();
            
            if (_isDirty)
            {
                _isDirty = false;
                Repaint();
            }
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }
    }
}