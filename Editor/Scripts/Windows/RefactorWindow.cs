/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Dash.Editor
{
    public class RefactorWindow : EditorWindow
    {
        private static Vector2 _scrollPosition;

        public static RefactorWindow Instance { get; private set; }

        private static Variable _variable;
        private static string _refactoredName; 
        
        public static RefactorWindow InitRefactorWindow()
        {
            Instance = GetWindow<RefactorWindow>();
            Instance.Initialize();

            return Instance;
        }

        private void Initialize()
        {
            titleContent = new GUIContent("Dash Refactoring");
            minSize = new Vector2(800, 400);
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
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Old Name: "+_variable.Name);
            GUILayout.Label("New Name: ");
            _refactoredName = GUILayout.TextField(_refactoredName);
            GUILayout.EndHorizontal();

            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, scrollViewStyle,
                GUILayout.ExpandWidth(true), GUILayout.Height(rect.height - 80));
            GUILayout.BeginVertical();
            
            // foreach (var message in messages)
            // {
            //     GUI.color = message.Item2;
            //     GUILayout.Label(message.Item1);
            //     GUI.color = Color.white;
            // }
            
            GUILayout.EndVertical();
            GUILayout.EndScrollView();

            GUILayout.Space(4);
            if (GUILayout.Button("Refactor", GUILayout.Height(30)))
            {
                
            }
        }
    }
}