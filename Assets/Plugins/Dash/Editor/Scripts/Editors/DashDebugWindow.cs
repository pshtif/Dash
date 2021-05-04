/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Dash
{
    public class DashDebugWindow : EditorWindow
    {
        private String _search = "";
        private Vector2 _scrollPosition;
        private bool _isDirty = false;
        
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
            DashEditorCore.OnDebugMessage -= OnDebug;
            DashEditorCore.OnDebugMessage += OnDebug;
        }
        
        private void OnDisable()
        {
            DashEditorCore.OnDebugMessage -= OnDebug;
        }

        private void OnDebug(string p_debug)
        {
            //Debug.Log(p_debug);
            _isDirty = true;
        }

        private void OnGUI()
        {
            var rect = new Rect(0, 0, position.width, position.height);
            
            GUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 60;
            var labelStyle = new GUIStyle();
            labelStyle.alignment = TextAnchor.MiddleRight;
            _search = EditorGUILayout.TextField(new GUIContent("Search"), _search, GUILayout.Width(200));
            
            GUILayout.Space(4);
            
            EditorGUILayout.IntField(new GUIContent("Max Log"), 10, GUILayout.Width(120));
            GUILayout.EndHorizontal();
            
            GUILayout.BeginArea(new Rect(rect.x, rect.y+30, rect.width, rect.height-30));
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, false);
            GUILayout.BeginVertical();
            
            var style = new GUIStyle();
            style.alignment = TextAnchor.MiddleLeft;
            if (DashEditorCore.DebugList != null)
            {
                for (int i = DashEditorCore.DebugList.Count-1; i>=0; i--)
                {
                    var debug = DashEditorCore.DebugList[i];
                    var split = debug.Split('|').ToList();
                    
                    if (!string.IsNullOrWhiteSpace(_search))
                    {
                        if (!split.Exists(s => s.Contains(_search)))
                        {
                            continue;
                        }
                    }
                    
                    GUILayout.BeginHorizontal();
                    GUILayout.BeginHorizontal(GUILayout.Height(16));
                    style.normal.textColor = Color.gray;
                    GUILayout.TextField("[" + split[0] + "] ", style, GUILayout.Width(85), GUILayout.ExpandWidth(false));
                    style.normal.textColor = Color.white;
                    GUILayout.TextField("Controller: ", style, GUILayout.ExpandWidth(false));
                    style.normal.textColor = Color.green;
                    if (GUILayout.Button(split[1], style, GUILayout.ExpandWidth(false)))
                    {
                        
                    }
                    style.normal.textColor = Color.white;
                    GUILayout.TextField(" Graph: ", style, GUILayout.ExpandWidth(false));
                    style.normal.textColor = Color.yellow;
                    GUILayout.TextField(split[2], style, GUILayout.ExpandWidth(false));
                    
                    style.normal.textColor = Color.white;
                    GUILayout.TextField(" Node: ", style, GUILayout.ExpandWidth(false));
                    style.normal.textColor = Color.cyan;
                    GUILayout.TextField(split[3], style, GUILayout.ExpandWidth(false));
                    
                    style.normal.textColor = Color.white;
                    GUILayout.TextField(" Target: ", style, GUILayout.ExpandWidth(false));
                    style.normal.textColor = Color.green;
                    GUILayout.TextField(split[4], style, GUILayout.ExpandWidth(false));
                    
                    GUILayout.EndHorizontal();
                    if (GUILayout.Button(IconManager.GetIcon("Settings_Icon"), GUIStyle.none, GUILayout.Height(12), GUILayout.MaxWidth(12)))
                    {
                        
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
        
        private void OnInspectorUpdate()
        {
            Repaint();
        }
    }
}