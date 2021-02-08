/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System.Collections.Generic;
using System.IO;
using System.Linq;
using OdinSerializer;
using UnityEditor;
using UnityEngine;

namespace Dash
{
    public static class GraphUtils
    {
        #if UNITY_EDITOR
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

        public static DashGraph CreateEmptyGraph()
        {
            return ScriptableObject.CreateInstance<DashGraph>();
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

            List<Object> references = new List<Object>();
            byte[] bytes = p_graph.SerializeToBytes(DataFormat.JSON, ref references);
            File.WriteAllBytes(path, bytes);
        }
#endif
    }
}
