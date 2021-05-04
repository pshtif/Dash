/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using UnityEditor;
using UnityEngine;

namespace Dash
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
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, false);
            GUILayout.BeginVertical();
            
            var style = new GUIStyle();
            style.alignment = TextAnchor.MiddleLeft;
            int count = 0;
            if (DashEditorDebug.DebugList != null)
            {
                for (int i = DashEditorDebug.DebugList.Count-1; i>=0; i--)
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

                    if (_showTime)
                    {
                        GUILayout.Space(4);
                        style.normal.textColor = Color.gray;
                        TimeSpan span = TimeSpan.FromSeconds(debug.time);
                        string timeString = span.ToString(@"hh\:mm\:ss\:fff");
                        GUILayout.Label("[" + timeString + "] ", style, GUILayout.Width(85),
                            GUILayout.ExpandWidth(false));
                    }

                    if (_showController)
                    {
                        GUILayout.Space(4);
                        style.normal.textColor = Color.white;
                        GUILayout.Label("Controller: ", style, GUILayout.ExpandWidth(false));
                        style.normal.textColor = Color.green;
                        GUILayout.Label(debug.controllerName, style, GUILayout.ExpandWidth(false));
                    }

                    if (_showGraph)
                    {
                        GUILayout.Space(4);
                        style.normal.textColor = Color.white;
                        GUILayout.Label(" Graph: ", style, GUILayout.ExpandWidth(false));
                        style.normal.textColor = Color.yellow;
                        GUILayout.Label(debug.graphPath, style, GUILayout.ExpandWidth(false));
                    }

                    if (_showNode)
                    {
                        GUILayout.Space(4);
                        style.normal.textColor = Color.white;
                        GUILayout.Label(" Node: ", style, GUILayout.ExpandWidth(false));
                        style.normal.textColor = Color.cyan;
                        GUILayout.Label(debug.nodeId, style, GUILayout.ExpandWidth(false));
                    }

                    if (_showTarget)
                    {
                        GUILayout.Space(4);
                        style.normal.textColor = Color.white;
                        GUILayout.Label(" Target: ", style, GUILayout.ExpandWidth(false));
                        style.normal.textColor = Color.magenta;
                        GUILayout.Label(debug.targetName, style, GUILayout.ExpandWidth(false));
                    }

                    GUILayout.EndHorizontal();
                    if (GUILayout.Button(IconManager.GetIcon("Settings_Icon"), GUIStyle.none, GUILayout.Height(12), GUILayout.MaxWidth(12)))
                    {
                        DebugItemContextMenu.Show(debug);   
                    }
                    GUILayout.Space(4);
                    GUILayout.EndHorizontal();

                    count++;
                    if (count > _maxLog)
                        break;
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