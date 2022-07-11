/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using OdinSerializer;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Dash.Editor
{
    public class ChecksumWindow : EditorWindow
    {
        private Vector2 _scrollPositionScanned;
        private Vector2 _scrollPositionPrevious;

        private DashChecksumObject _previousChecksumObject;
        private bool _highlightNonMatchingChecksums = false;
        private Dictionary<string, string> _currentChecksums;

        private int _previousScanIndex = -1;
        private DashChecksumObject[] _previousScans;

        public static ChecksumWindow Instance { get; private set; }
        
        [MenuItem ("Tools/Dash/Scan/Checksum")]
        public static ChecksumWindow InitChecksumWindow()
        {
            Instance = GetWindow<ChecksumWindow>();
            Instance.Initialize();

            return Instance;
        }

        private void Initialize()
        {
            titleContent = new GUIContent("Dash Checksum Editor");
            minSize = new Vector2(800, 400);
            _previousScans = LoadAllScans();
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
                    
                    if (GUILayout.Button("Nodes Checksums", GUILayout.Width(120)))
                    {
                       ViewNodeChecksums(DashEditorCore.EditorConfig.lastChecksumObject, graph);
                    }
                    GUILayout.EndHorizontal();
                }
            }
            
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
            
            bool scan = GUILayout.Button("Scan Graphs", GUILayout.Height(40));
            
            GUILayout.Space(8);
            _previousScanIndex = EditorGUILayout.Popup("Previous scans", _previousScanIndex, _previousScans.Select(s => s.name).ToArray());
            _previousChecksumObject = _previousScanIndex == -1 ? null : _previousScans[_previousScanIndex];
            
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
                    var checksum = GetChecksum(graphJson);
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
                    
                    if (GUILayout.Button("Nodes Checksums", GUILayout.Width(120)))
                    {
                        ViewNodeChecksums(_previousChecksumObject, graph, DashEditorCore.EditorConfig.lastChecksumObject);
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

        void ViewNodeChecksums(DashChecksumObject p_checksum, string p_graph, DashChecksumObject p_previousChecksum = null)
        {
            ConsoleWindow.InitConsoleWindow();
            
            if (p_checksum.nodeChecksums == null || !p_checksum.nodeChecksums.ContainsKey(p_graph))
            {
                Console.Add("Node checksums for graph at "+p_graph+" missing.");
                return;
            }
            
            Console.Add("Node checksums for graph at " + p_graph);
            
            foreach (var pair in p_checksum.nodeChecksums[p_graph])
            {
                if (p_previousChecksum != null && p_previousChecksum.nodeChecksums.ContainsKey(p_graph))
                {
                    if (!p_previousChecksum.nodeChecksums[p_graph].ContainsKey(pair.Key))
                    {
                        Console.Add("Node "+pair.Key+" not found in previous checksum.");   
                    }
                    else
                    {
                        if (pair.Value != p_previousChecksum.nodeChecksums[p_graph][pair.Key])
                        {
                            Console.Add(
                                pair.Key + ": " + pair.Value + " - " +
                                p_previousChecksum.nodeChecksums[p_graph][pair.Key] + " DIFFERENT", Color.red);
                        }
                        else
                        {
                            Console.Add(pair.Key + ": " + pair.Value + " - " +
                                        p_previousChecksum.nodeChecksums[p_graph][pair.Key] + " MATCH");
                        }
                    }
                }
                else
                {
                    Console.Add(pair.Key + " : " + pair.Value);
                }
            }
        }

        string GetChecksum(byte[] p_bytes)
        {
            return JSONHashUtils.GetJSONHash(Encoding.Default.GetString(p_bytes));
        }

        void Scan()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Resources/Editor/Scans"))
            {
                AssetDatabase.CreateFolder("Assets/Resources/Editor", "Scans");
            }

            var now = DateTime.Now;
            var timestamp = now.ToString("dd'_'MM'_'yyyy'_'HH'_'mm'_'ss");

            List<(string,DashGraph,byte[])> scannedGraphs;
            DashScanner.ScanForJson(out scannedGraphs);
            
            DashChecksumObject checksumObject = ScriptableObject.CreateInstance<DashChecksumObject>();
            checksumObject.scannedGraphs = new List<string>();
            checksumObject.scannedGraphJsons = new List<byte[]>();
            List<Object> references = new List<Object>();

            foreach (var data in scannedGraphs)
            {
                checksumObject.scannedGraphs.Add(data.Item1);
                checksumObject.scannedGraphJsons.Add(data.Item3);
                
                // Dictionary<string, string> nodes = new Dictionary<string, string>();
                // checksumObject.nodeChecksums.Add(pair.Key, nodes);
                // foreach (var node in pair.Value.Nodes)
                // {
                //     var bytes = node.SerializeToBytes(DataFormat.JSON, ref references);
                //     if (nodes.ContainsKey(node.Id))
                //     {
                //         Debug.LogWarning("Duplicate node id found "+node.Id+" in "+pair.Value.name+" Node checksum will be incomplete.");
                //     }
                //     else
                //     {
                //         nodes.Add(node.Id, GetChecksum(bytes));
                //     }
                // }
            }

            AssetDatabase.CreateAsset(checksumObject,
                "Assets/Resources/Editor/Scans/Scan_" + timestamp + ".asset");

            DashEditorCore.EditorConfig.lastChecksumObject = checksumObject;
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private DashChecksumObject[] LoadAllScans()
        {
            string[] assetGUIDs = UnityEditor.AssetDatabase.FindAssets("t:" + nameof(DashChecksumObject),
                new[] { "Assets/Resources/Editor/Scans/" });
            List<DashChecksumObject> scans = new List<DashChecksumObject>();
            foreach (string guid in assetGUIDs)
            {
                string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                DashChecksumObject scan = AssetDatabase.LoadAssetAtPath<DashChecksumObject>(assetPath);
                scans.Add(scan);
            }
            return scans.ToArray();
        }
    }
}