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
    public class AOTWindow : EditorWindow
    {
        private Vector2 _scrollPositionScanned;
        private Vector2 _scrollPositionExplicit;

        private bool _generateLinkXml = true;
        private bool _includeOdin = false;

        public static AOTWindow Instance { get; private set; }
        
        [MenuItem ("Tools/Dash/AOT")]
        public static AOTWindow InitDebugWindow()
        {
            Instance = GetWindow<AOTWindow>();
            Instance.titleContent = new GUIContent("Dash AOT Editor");
            Instance.minSize = new Vector2(800, 400);

            return Instance;
        }

        private void OnGUI()
        {
            var rect = new Rect(0, 0, position.width, position.height);
            
            GUICustomUtils.DrawTitle("Dash AOT Scanner/Generator");

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
            GUILayout.Label("Scanned types", titleStyle, GUILayout.ExpandWidth(true));
            GUILayout.Label(
                "Last scan found " +
                (DashEditorCore.EditorConfig.scannedAOTTypes == null
                    ? 0
                    : DashEditorCore.EditorConfig.scannedAOTTypes.Count) + " types", infoStyle,
                GUILayout.ExpandWidth(true));
            GUILayout.Space(2);

            _scrollPositionScanned = GUILayout.BeginScrollView(_scrollPositionScanned, scrollViewStyle,
                GUILayout.ExpandWidth(true), GUILayout.Height(rect.height / 2 - 100));
            GUILayout.BeginVertical();
            
            if (DashEditorCore.EditorConfig.scannedAOTTypes != null)
            {
                foreach (Type type in DashEditorCore.EditorConfig.scannedAOTTypes)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label((string.IsNullOrEmpty(type.Namespace) ? "" : type.Namespace + ".") +
                                    type.GetReadableTypeName());
                    if (GUILayout.Button("Remove", GUILayout.Width(120)))
                    {
                        DashAOTScanner.RemoveScannedAOTType(type);
                        break;
                    }
                    GUILayout.EndHorizontal();
                }
            }
            
            GUILayout.EndVertical();
            GUILayout.EndScrollView();


            GUILayout.Space(4);
            GUILayout.Label("Explicit Types", titleStyle, GUILayout.ExpandWidth(true));
            GUILayout.Label(
                "You have " +
                (DashEditorCore.EditorConfig.explicitAOTTypes == null
                    ? 0
                    : DashEditorCore.EditorConfig.explicitAOTTypes.Count) + " explicitly defined types.", infoStyle,
                GUILayout.ExpandWidth(true));
            GUILayout.Space(2);

            _scrollPositionExplicit = GUILayout.BeginScrollView(_scrollPositionExplicit, scrollViewStyle,
                GUILayout.ExpandWidth(true), GUILayout.Height(rect.height / 2 - 100));
            GUILayout.BeginVertical();
            
            if (DashEditorCore.EditorConfig.explicitAOTTypes != null)
            {
                foreach (Type type in DashEditorCore.EditorConfig.explicitAOTTypes)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label((string.IsNullOrEmpty(type.Namespace) ? "" : type.Namespace + ".") +
                                    type.GetReadableTypeName());
                    if (GUILayout.Button("Remove", GUILayout.Width(120)))
                    {
                        DashAOTScanner.RemoveExplicitAOTType(type);
                        break;
                    }
                    GUILayout.EndHorizontal();
                }
            }
            
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
            
            GUILayout.Space(4);
            GUILayout.BeginVertical();
            
            GUILayout.BeginHorizontal();
            _generateLinkXml =
                GUILayout.Toggle(_generateLinkXml, new GUIContent("Generate Link Xml"), GUILayout.Width(140));
            _includeOdin = GUILayout.Toggle(_includeOdin, new GUIContent("Include Odin Assembly"));

            GUILayout.EndHorizontal();
            GUILayout.Space(2);
            GUILayout.BeginHorizontal();

            bool generate = GUILayout.Button("Generate AOT DLL", GUILayout.Height(40));

            bool scan = GUILayout.Button("Scan Types", GUILayout.Height(40));
            
            if (GUILayout.Button("Add Explicit Type", GUILayout.Height(40)))
            {
                AddTypeContextMenu.ShowAsPopup(AddType);
            }

            GUILayout.EndHorizontal();

            var dll = PluginImporter.GetAtPath(DashEditorCore.EditorConfig.AOTAssemblyPath + "/" +
                                                  DashEditorCore.EditorConfig.AOTAssemblyName+".dll");

            if (dll != null)
            {
                GUILayout.Label("Assembly generated in " + DashEditorCore.EditorConfig.AOTAssemblyPath + "/" +
                                DashEditorCore.EditorConfig.AOTAssemblyName + ".dll" + " last generated on " +
                                DashEditorCore.EditorConfig.AOTAssemblyGeneratedTime);
            }
            else
            {
                GUILayout.Label("No generated Dash AOT Assembly found.");
            }

            GUILayout.EndVertical();

            if (scan)
            {
                DashAOTScanner.Scan();
            } else if (generate)
            {
                DashAOTScanner.GenerateDLL(_generateLinkXml, _includeOdin);
            }
        }
        
        static void AddType(object p_type)
        {
            if (DashEditorCore.EditorConfig.explicitAOTTypes == null)
            {
                DashEditorCore.EditorConfig.explicitAOTTypes = new List<Type>();
            }

            if (!DashEditorCore.EditorConfig.explicitAOTTypes.Contains((Type)p_type))
            {
                DashEditorCore.EditorConfig.explicitAOTTypes.Add((Type)p_type);
            }
            
            EditorUtility.SetDirty(DashEditorCore.EditorConfig);
        }
    }
}