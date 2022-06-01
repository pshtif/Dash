/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Dash.Editor
{
    public class Console
    {
        public static void Add(string p_message, Color? p_color = null)
        {
            Color color = p_color == null ? Color.white : p_color.Value;
            ConsoleWindow.AddMessage((p_message, color));
        }
    }
    
    public class ConsoleWindow : EditorWindow
    {
        private static Vector2 _scrollPosition;

        public static ConsoleWindow Instance { get; private set; }

        public static List<(string,Color)> messages = new List<(string,Color)>();
        
        [MenuItem ("Tools/Dash/Console")]
        public static ConsoleWindow InitConsoleWindow()
        {
            Instance = GetWindow<ConsoleWindow>();
            Instance.Initialize();

            return Instance;
        }

        private void Initialize()
        {
            titleContent = new GUIContent("Dash Console");
            minSize = new Vector2(800, 400);
        }

        public static void AddMessage((string, Color) p_message)
        {
            messages.Add(p_message);
            _scrollPosition = new Vector2(0, int.MaxValue);
        }

        private void OnGUI()
        {
            var rect = new Rect(0, 0, position.width, position.height);
            
            GUICustomUtils.DrawTitle("Dash Console");

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

            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, scrollViewStyle,
                GUILayout.ExpandWidth(true), GUILayout.Height(rect.height - 80));
            GUILayout.BeginVertical();
            
            foreach (var message in messages)
            {
                GUI.color = message.Item2;
                GUILayout.Label(message.Item1);
                GUI.color = Color.white;
            }
            
            GUILayout.EndVertical();
            GUILayout.EndScrollView();

            GUILayout.Space(4);
            if (GUILayout.Button("CLEAR", GUILayout.Height(30)))
            {
                messages = new List<(string, Color)>();
            }
        }
    }
}