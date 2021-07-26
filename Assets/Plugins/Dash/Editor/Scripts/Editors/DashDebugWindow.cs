/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using OdinSerializer.Utilities;
using UnityEditor;
using UnityEngine;

namespace Dash.Editor
{
    public class DashDebugWindow : EditorWindow
    {
        private String _search = "";
        private Vector2 _scrollPosition;
        private bool _isDirty = false;

        private int _maxLog = 10;
        
        private bool _showTime = true;
        private bool _showController = true;
        private bool _showGraph = true;
        private bool _showNode = true;
        private bool _showTarget = true;
        
        public static DashDebugWindow Instance { get; private set; }
        
        [MenuItem ("Tools/Dash/Debug")]
        public static DashDebugWindow InitDebugWindow()
        {
            Instance = GetWindow<DashDebugWindow>();
            Instance.titleContent = new GUIContent("Dash Debug Console");

            return Instance;
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
            
            GUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Clear"))
            {
                DashEditorDebug.DebugList.Clear();
            }
            
            var labelStyle = new GUIStyle();
            labelStyle.alignment = TextAnchor.MiddleRight;
            EditorGUIUtility.labelWidth = 35;
            _search = EditorGUILayout.TextField(new GUIContent("Filter:"), _search, GUILayout.Width(200));
            GUILayout.Space(8);
            EditorGUIUtility.labelWidth = 55;
            _maxLog = EditorGUILayout.IntField(new GUIContent("Max Log:"), _maxLog, GUILayout.Width(120));
            GUILayout.Space(4);
            _showTime = GUILayout.Toggle(_showTime, new GUIContent("Time"), GUILayout.ExpandWidth(false));
            GUILayout.Space(4);
            _showController = GUILayout.Toggle(_showController, new GUIContent("Controller"), GUILayout.ExpandWidth(false));
            GUILayout.Space(4);
            _showGraph = GUILayout.Toggle(_showGraph, new GUIContent("GraphPath"), GUILayout.ExpandWidth(false));
            GUILayout.Space(4);
            _showNode = GUILayout.Toggle(_showNode, new GUIContent("NodeId"), GUILayout.ExpandWidth(false));
            GUILayout.Space(4);
            _showTarget = GUILayout.Toggle(_showTarget, new GUIContent("Target"), GUILayout.ExpandWidth(false));
            GUILayout.EndHorizontal();
            
            GUILayout.BeginArea(new Rect(rect.x, rect.y+30, rect.width, rect.height-30));
            if (_isDirty)
            {
                _scrollPosition = new Vector2(0, int.MaxValue);
            }

            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, false);
            GUILayout.BeginVertical();
            
            var style = new GUIStyle();
            style.alignment = TextAnchor.MiddleLeft;
            if (DashEditorDebug.DebugList != null)
            {
                int start = _maxLog < DashEditorDebug.DebugList.Count ? DashEditorDebug.DebugList.Count - _maxLog : 0;
                for (int i = start; i<DashEditorDebug.DebugList.Count; i++)
                {
                    var debug = DashEditorDebug.DebugList[i];

                    bool found = false;
                    if (!string.IsNullOrWhiteSpace(_search))
                    {
                        if (_showController && debug.controllerName.Contains(_search))
                            found = true;
                        
                        if (_showGraph && debug.graphPath.Contains(_search))
                            found = true;
                        
                        if (_showNode && debug.nodeId.Contains(_search))
                            found = true;
                        
                        if (_showTarget && debug.targetName.Contains(_search))
                            found = true;

                        if (!found)
                            continue;
                    }
                    
                    GUILayout.BeginHorizontal();
                    GUILayout.BeginHorizontal(GUILayout.Height(16));
                    
                    switch (debug.type)
                    {
                        case DebugType.EXECUTE:
                            DrawExecuteItem(debug, style);
                            break;
                        case DebugType.ERROR:
                            DrawErrorItem(debug, style);
                            break;
                    }

                    GUILayout.EndHorizontal();
                    if (GUILayout.Button(IconManager.GetIcon("Settings_Icon"), GUIStyle.none, GUILayout.Height(12), GUILayout.MaxWidth(12)))
                    {
                        DebugItemContextMenu.Show(debug);   
                    }
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

        private void DrawErrorItem(DebugItem p_item, GUIStyle p_style)
        {
            if (_showTime)
            {
                GUILayout.Space(4);
                p_style.normal.textColor = Color.red;
                TimeSpan span = TimeSpan.FromSeconds(p_item.time);
                string timeString = span.ToString(@"hh\:mm\:ss\:fff");
                GUILayout.Label("[" + timeString + "] ", p_style, GUILayout.Width(60),
                    GUILayout.ExpandWidth(false));
            }
            
            GUILayout.Space(30);
            p_style.normal.textColor = Color.red;
            GUILayout.Label("ERROR", p_style, GUILayout.ExpandWidth(false));
            
            GUILayout.Space(4);
            p_style.normal.textColor = Color.white;
            GUILayout.Label(" Graph: ", p_style, GUILayout.ExpandWidth(false));
            p_style.normal.textColor = Color.yellow;
            GUILayout.Label(p_item.graphPath, p_style, GUILayout.ExpandWidth(false));

            GUILayout.Space(4);
            p_style.normal.textColor = Color.white;
            GUILayout.Label(" Node: ", p_style, GUILayout.ExpandWidth(false));
            p_style.normal.textColor = Color.cyan;
            GUILayout.Label(p_item.nodeId, p_style, GUILayout.ExpandWidth(false));

            p_style.normal.textColor = Color.white;
            GUILayout.Label(" Message: ", p_style, GUILayout.ExpandWidth(false));
            p_style.normal.textColor = Color.yellow;
            GUILayout.Label(" "+p_item.message, p_style, GUILayout.ExpandWidth(false));
        }

        private void DrawExecuteItem(DebugItem p_item, GUIStyle p_style)
        {
            if (_showTime)
            {
                GUILayout.Space(4);
                p_style.normal.textColor = Color.gray;
                TimeSpan span = TimeSpan.FromSeconds(p_item.time);
                string timeString = span.ToString(@"hh\:mm\:ss\:fff");
                GUILayout.Label("[" + timeString + "] ", p_style, GUILayout.Width(60),
                    GUILayout.ExpandWidth(false));
            }

            GUILayout.Space(30);
            p_style.normal.textColor = Color.gray;
            GUILayout.Label("EXECUTE", p_style, GUILayout.ExpandWidth(false));
            

            if (_showController)
            {
                GUILayout.Space(4);
                p_style.normal.textColor = Color.white;
                GUILayout.Label("Controller: ", p_style, GUILayout.ExpandWidth(false));
                p_style.normal.textColor = Color.green;
                GUILayout.Label(p_item.controllerName, p_style, GUILayout.ExpandWidth(false));
            }

            if (_showGraph)
            {
                GUILayout.Space(4);
                p_style.normal.textColor = Color.white;
                GUILayout.Label(" Graph: ", p_style, GUILayout.ExpandWidth(false));
                p_style.normal.textColor = Color.yellow;
                GUILayout.Label(p_item.graphPath, p_style, GUILayout.ExpandWidth(false));
            }

            if (_showNode)
            {
                GUILayout.Space(4);
                p_style.normal.textColor = Color.white;
                GUILayout.Label(" Node: ", p_style, GUILayout.ExpandWidth(false));
                p_style.normal.textColor = Color.cyan;
                GUILayout.Label(p_item.nodeId, p_style, GUILayout.ExpandWidth(false));
            }

            if (_showTarget)
            {
                GUILayout.Space(4);
                p_style.normal.textColor = Color.white;
                GUILayout.Label(" Target: ", p_style, GUILayout.ExpandWidth(false));
                p_style.normal.textColor = Color.magenta;
                GUILayout.Label(p_item.targetName, p_style, GUILayout.ExpandWidth(false));
            }
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }
    }
}