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
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, false);
            GUILayout.BeginVertical();
            
            var style = new GUIStyle();
            style.alignment = TextAnchor.MiddleLeft;
            if (DashEditorCore.DebugList != null)
            {
                for (int i = DashEditorCore.DebugList.Count-1; i>=0; i--)
                {
                    var split = DashEditorCore.DebugList[i].Split('|');
                    GUILayout.BeginHorizontal(GUILayout.Height(16));
                    style.normal.textColor = Color.gray;
                    GUILayout.TextField("[" + split[0] + "] ", style, GUILayout.ExpandWidth(false));
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
                }
            }

            GUILayout.EndVertical();
            GUILayout.EndScrollView();
            
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