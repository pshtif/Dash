﻿/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System.Collections.Generic;
using System.IO;
using System.Linq;
using OdinSerializer;
using OdinSerializer.Utilities;
using UnityEditor;
using UnityEngine;

namespace Dash
{
    public static class GraphUtils
    {
        public static DashGraph CreateEmptyGraph()
        {
            DashGraph graph = ScriptableObject.CreateInstance<DashGraph>();
            ((IInternalGraphAccess)graph).SetVersion(DashCore.GetVersionNumber());
            return graph;
        }
        
#if UNITY_EDITOR
        public static string AddChildPath(string p_path, string p_subPath)
        {
            return p_path + (p_path.Length>0 ?  "/" : "") + p_subPath;
        }

        public static bool IsSubGraph(string p_path)
        {
            return !string.IsNullOrEmpty(p_path);
        }

        public static string GetParentPath(string p_path)
        {
            if (string.IsNullOrWhiteSpace(p_path) || p_path.IndexOf("/") == -1)
                return "";

            return p_path.Substring(0, p_path.LastIndexOf("/"));
        }
        
        public static DashGraph CreateGraphAsAssetFile(DashGraph p_graph = null)
        {
            var path = EditorUtility.SaveFilePanelInProject(
                "Create Dash graph",
                "DashGraph",
                "asset",
                "Enter name for new Dash graph.");
            
            if (path.Length != 0)
            {
                return CreateGraphAsAssetFromPath(path, p_graph);
            }
            
            return null;
        }
        
        public static DashGraph CreateGraphAsAssetFromPath(string p_path, DashGraph p_graph = null)
        {
            if (p_graph == null)
                p_graph = CreateEmptyGraph();

            AssetDatabase.CreateAsset(p_graph, p_path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return p_graph;
        }

        public static DashGraph LoadGraph()
        {
            string graphPath = EditorUtility.OpenFilePanel("Load Graph", Application.dataPath, "");

            if (string.IsNullOrEmpty(graphPath))
                return null;
            
            int appPathLength = Application.dataPath.Length;
            graphPath = graphPath.Substring(appPathLength - 6);

            return (DashGraph)AssetDatabase.LoadAssetAtPath(graphPath, typeof(DashGraph));
        }

        public static void ImportJSON(DashGraph p_graph)
        {
            string path = EditorUtility.OpenFilePanel("Load JSON", Application.dataPath, "json");

            byte[] content = File.ReadAllBytes(path);
            List<Object> references = new Object[] {p_graph}.ToList(); // refactor to serialize to acquire all correct ones before deserialization
            p_graph.DeserializeFromBytes(content, DataFormat.JSON, ref references);
        }

        public static void ExportJSON(DashGraph p_graph)
        {
            var path = EditorUtility.SaveFilePanel(
                "Export Graph JSON",
                Application.dataPath,
                "",
                "json");

            if (!path.IsNullOrWhitespace())
            {
                List<Object> references = new List<Object>();
                byte[] bytes = p_graph.SerializeToBytes(DataFormat.JSON, ref references);
                File.WriteAllBytes(path, bytes);
            }
        }
        
        // static List<DashGraph> GetAllGraphs()
        // {
        //     List<DashGraph> graphs = new List<DashGraph>();
        //     string[] graphGuids = AssetDatabase.FindAssets("t:DashGraph");
        //     foreach (string graphGuid in graphGuids)
        //     {
        //         string path = AssetDatabase.GUIDToAssetPath(graphGuid);
        //         DashGraph graph = AssetDatabase.LoadAssetAtPath<DashGraph>(path);
        //         graphs.Add(graph);
        //     }
        //
        //     return graphs;
        // }
#endif
    }
}