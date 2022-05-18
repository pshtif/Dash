/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Dash.Editor
{
    public class ChecksumWindow : EditorWindow
    {
        private Vector2 _scrollPositionScanned;
        private Vector2 _scrollPositionPrevious;

        private DashChecksumObject _previousChecksumObject;
        private bool _highlightNonMatchingChecksums = false;
        private Dictionary<string, string> _currentChecksums;

        public static ChecksumWindow Instance { get; private set; }
        
        [MenuItem ("Tools/Dash/Scan/Checksum")]
        public static ChecksumWindow InitChecksumWindow()
        {
            Instance = GetWindow<ChecksumWindow>();
            Instance.titleContent = new GUIContent("Dash Checksum Editor");
            Instance.minSize = new Vector2(800, 400);

            return Instance;
        }

        private void OnGUI()
        {
            var rect = new Rect(0, 0, position.width, position.height);
            
            GUICustomUtils.DrawTitle("Dash Checksum Scanner");

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
            GUILayout.Label("Scanned graphs", titleStyle, GUILayout.ExpandWidth(true));
            GUILayout.Label(
                "Last scan found " +
                (DashEditorCore.EditorConfig.lastChecksumObject == null
                    ? 0
                    : DashEditorCore.EditorConfig.lastChecksumObject.scannedGraphs.Count) + " graphs", infoStyle,
                GUILayout.ExpandWidth(true));
            GUILayout.Space(2);

            _scrollPositionScanned = GUILayout.BeginScrollView(_scrollPositionScanned, scrollViewStyle,
                GUILayout.ExpandWidth(true), GUILayout.Height(rect.height/2 - 110));
            GUILayout.BeginVertical();
            
            if (DashEditorCore.EditorConfig.lastChecksumObject != null)
            {
                _currentChecksums = new Dictionary<string, string>();
                for (int i = 0; i<DashEditorCore.EditorConfig.lastChecksumObject.scannedGraphs.Count; i++)
                {
                    var graph = DashEditorCore.EditorConfig.lastChecksumObject.scannedGraphs[i];
                    var graphJson = DashEditorCore.EditorConfig.lastChecksumObject.scannedGraphJsons[i];
                    GUILayout.BeginHorizontal();
                    var checksum = JSONHashUtils.GetJSONHash(Encoding.Default.GetString(graphJson));
                    GUILayout.Label(graph);
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(checksum);
                    _currentChecksums.Add(graph, checksum);
                    if (GUILayout.Button("ViewJSON", GUILayout.Width(120)))
                    {
                        ViewJSON(DashEditorCore.EditorConfig.lastChecksumObject, graph, graphJson);
                    }
                    GUILayout.EndHorizontal();
                }
            }
            
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
            
            bool scan = GUILayout.Button("Scan Graphs", GUILayout.Height(40));
            
            GUILayout.Space(8);
            _previousChecksumObject = (DashChecksumObject)EditorGUILayout.ObjectField(
                new GUIContent("Previous Checksum Object"), _previousChecksumObject, typeof(DashChecksumObject), false);
            
            GUILayout.Label("Previously scanned graphs", titleStyle, GUILayout.ExpandWidth(true));
            GUILayout.Label(
                "You have " +
                (_previousChecksumObject == null
                    ? 0
                    : _previousChecksumObject.scannedGraphs.Count) + " scanned graphs.", infoStyle,
                GUILayout.ExpandWidth(true));
            GUILayout.Space(2);

            _scrollPositionPrevious = GUILayout.BeginScrollView(_scrollPositionPrevious, scrollViewStyle,
                GUILayout.ExpandWidth(true), GUILayout.Height(rect.height / 2 - 110));
            GUILayout.BeginVertical();

            if (_previousChecksumObject != null)
            {
                for (int i = 0; i<_previousChecksumObject.scannedGraphs.Count; i++)
                {
                    var graph = _previousChecksumObject.scannedGraphs[i];
                    var graphJson = _previousChecksumObject.scannedGraphJsons[i];
                    GUILayout.BeginHorizontal();
                    var checksum = JSONHashUtils.GetJSONHash(Encoding.Default.GetString(graphJson));
                    GUILayout.Label(graph);
                    GUI.color = (_highlightNonMatchingChecksums && _currentChecksums.ContainsKey(graph) && _currentChecksums[graph] != checksum)
                        ? Color.red
                        : Color.white;
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(checksum);
                    GUI.color = Color.white;
                    if (GUILayout.Button("ViewJSON", GUILayout.Width(120)))
                    {
                        ViewJSON(_previousChecksumObject, graph, graphJson);
                    }
                    GUILayout.EndHorizontal();
                }
            }
            
            GUILayout.EndVertical();
            GUILayout.EndScrollView();

            _highlightNonMatchingChecksums =
                GUILayout.Toggle(_highlightNonMatchingChecksums, "Highlight non matching checksums");

            if (scan)
            {
                Scan();
            } 
        }

        void ViewJSON(DashChecksumObject p_checksum, string p_name, byte[] p_data)
        {
            var assetPath = AssetDatabase.GetAssetPath(p_checksum);
            assetPath = assetPath.Substring(0, assetPath.LastIndexOf("/"));
            
            var filePath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/") + 1) + assetPath;

            p_name = p_name.Replace("/", "_");
            File.WriteAllText(filePath + "/" + p_name + ".json", Encoding.Default.GetString(p_data));
            
            //Debug.Log(assetPath+" : "+filePath);
            AssetDatabase.ImportAsset(assetPath + "/" + p_name + ".json");
            var jsonAsset = AssetDatabase.LoadMainAssetAtPath(assetPath + "/" + p_name + ".json");
            AssetDatabase.OpenAsset(jsonAsset);
        }

        void Scan()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Resources/Editor/Scans"))
            {
                AssetDatabase.CreateFolder("Assets/Resources/Editor", "Scans");
            }

            var timestamp = DateTime.Now.ToFileTime();
            AssetDatabase.CreateFolder("Assets/Resources/Editor/Scans", "SCAN_"+timestamp);
            
            Dictionary<string,byte[]> scannedGraphs = DashScanner.ScanForJson();
            
            DashChecksumObject checksumObject = ScriptableObject.CreateInstance<DashChecksumObject>();
            checksumObject.scannedGraphs = new List<string>();
            checksumObject.scannedGraphJsons = new List<byte[]>();
            foreach (var pair in scannedGraphs)
            {
                checksumObject.scannedGraphs.Add(pair.Key);
                checksumObject.scannedGraphJsons.Add(pair.Value);
            }
            
            AssetDatabase.CreateAsset(checksumObject, "Assets/Resources/Editor/Scans/SCAN_"+timestamp+"/Checksum.asset");

            DashEditorCore.EditorConfig.lastChecksumObject = checksumObject;
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}