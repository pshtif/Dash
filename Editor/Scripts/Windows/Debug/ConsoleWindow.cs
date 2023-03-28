/*
 *	Created by:  Peter @sHTiF Stefcek
 */
#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using OdinSerializer.Utilities;
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
        private static string _command;

        public static ConsoleWindow Instance { get; private set; }

        public static List<(string,Color)> messages = new List<(string,Color)>();
        
        public static void Init()
        {
            Instance = GetWindow<ConsoleWindow>();
            Instance.Initialize();
        }

        public static void RunInitialGraphScan()
        {
            if (Instance == null)
            {
                Instance = GetWindow<ConsoleWindow>();
                Instance.Initialize();
            }
            Instance.Show();

            AddMessage((
                "Version upgraded from " + VersionUtils.GetVersionString(DashEditorCore.EditorConfig.lastUsedVersion) +
                " to " + VersionUtils.GetVersionString(DashCore.GetVersionNumber()), Color.white));
            AddMessage(("Performing graph scanning...", Color.white));
            ScanGraphs();
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
            
            GUIUtils.DrawTitle("Dash Console");

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
                GUILayout.ExpandWidth(true), GUILayout.Height(rect.height - 100));
            GUILayout.BeginVertical();
            
            foreach (var message in messages)
            {
                GUI.color = message.Item2;
                GUILayout.Label(message.Item1);
                GUI.color = Color.white;
            }
            
            GUILayout.EndVertical();
            GUILayout.EndScrollView();

            _command = GUILayout.TextField(_command);
            if (Event.current.keyCode == KeyCode.Return && !_command.IsNullOrWhitespace())
            {
                ExecuteCommand();
            }

            GUILayout.Space(4);
            if (GUILayout.Button("CLEAR", GUILayout.Height(30)))
            {
                messages = new List<(string, Color)>();
            }
        }

        static void ExecuteCommand()
        {
            var commandSplit = _command.Split(' ').ToList();
            string command = commandSplit[0];

            bool verbose = commandSplit.Contains("--verbose");
            commandSplit.Remove("--verbose");

            switch (command)
            {
                case "/version":
                    AddMessage(("Dash Version "+DashCore.GetVersionNumber(),Color.white));
                    break;
                case "/scan":
                    ScanGraphs(commandSplit.Count>1 ? commandSplit[1] : "", verbose);
                    break;
                default:
                    AddMessage(("Unknown command "+command,Color.red));
                    break;
            }

            _command = "";
        }
        
        // Move to Scanner when bound scanning will be removed
        private static bool ScanGraphs(string p_match = "", bool p_verbose = false)
        {
            var graphs = AssetUtils.FindAssetsByType<DashGraph>();

            foreach (var graph in graphs)
            {
                if (p_match != "" && !graph.name.Contains(p_match))
                    continue;

                var info = graph.CheckValidity(p_verbose);
                for (int i = 0; i < info.Length; i++)
                {
                    AddMessage(info[i]);
                }
            }

            return true;
        }
    }
}
#endif