/*
 *	Created by:  Peter @sHTiF Stefcek
 */

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Dash
{
    [Serializable]
    [InitializeOnLoad]
    public class DashEditorCore
    {
        public const string VERSION = "0.2.1b";

        static public DashEditorConfig Config { get; private set; }

        static public DashEditorPreviewer Previewer { get; private set; }

        static public GUISkin Skin => (GUISkin)Resources.Load("Skins/EditorSkins/NodeEditorSkin");

        static public int TITLE_TAB_HEIGHT = 26;

        static public int CONNECTOR_HEIGHT = 24;

        static public int CONNECTOR_PADDING = 4;

        static public Color NODE_EXECUTING_COLOR = Color.cyan;
        static public Color NODE_SELECTED_COLOR = Color.green;
        static public Color CONNECTION_INACTIVE_COLOR = new Color(0.3f, 0.3f, .3f);
        static public Color CONNECTION_ACTIVE_COLOR = new Color(0.8f, 0.6f, 0f);
        static public Color CONNECTOR_INPUT_CONNECTED_COLOR = new Color(0.9f, 0.7f, 0f);
        static public Color CONNECTOR_INPUT_DISCONNECTED_COLOR = new Color(0.4f, 0.3f, 0f);
        static public Color CONNECTOR_OUTPUT_CONNECTED_COLOR = new Color(1f, 0.7f, 0f);
        static public Color CONNECTOR_OUTPUT_DISCONNECTED_COLOR = new Color(1, 1, 1);
        
        static public bool draggingNodes = false;
        
        static public List<int> selectedNodes = new List<int>();
        
        static DashEditorCore()
        {
            CreateConfig();
            CreatePreviewer();

            EditorApplication.playModeStateChanged += OnPlayModeChanged;
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
        }

        public static void ReindexSelected(int p_index)
        {
            for (int i = 0; i < selectedNodes.Count; i++)
            {
                if (selectedNodes[i] > p_index)
                    selectedNodes[i]--;
            }
        }
        
        public static void EditController(DashController p_controller)
        {
            DeselectAllNodes();
            
            if (p_controller != null)
            {
                Config.editingGraph = p_controller.Graph;
                if (p_controller.Graph != null)
                {
                    ((IGraphEditorAccess) p_controller.Graph).SetController(p_controller);
                }
            }
            else
            {
                Config.editingGraph = null;
            }
        }

        public static void EditGraph(DashGraph p_graph)
        {
            DeselectAllNodes();
            
            Config.editingGraph = p_graph;
            if (p_graph != null)
                ((IGraphEditorAccess)p_graph).SetController(null);
        }
        
        public static void UnloadGraph()
        {
            DeselectAllNodes();
            
            Config.editingGraph = null;
        }
        
        static void OnHierarchyChanged()
        {
            // Debug.Log("OnHierarchyChanged");
        }

        static void OnPlayModeChanged(PlayModeStateChange p_change)
        {
            // Debug.Log("[PLAYMODECHANGE] "+p_change);
            
            if (p_change == PlayModeStateChange.ExitingEditMode)
            {
                Config.enteringPlayModeController = Config.editingGraph != null ? Config.editingGraph.Controller : null;
            }
            
            if (p_change == PlayModeStateChange.EnteredPlayMode)
            {
                if (Config.enteringPlayModeController != null)
                {
                    EditController(Config.enteringPlayModeController);
                }
            }

            if (p_change == PlayModeStateChange.EnteredEditMode)
            {
                if (Config.enteringPlayModeController != null)
                {
                    EditController(Config.enteringPlayModeController);    
                }
                else
                {
                    EditGraph(Config.enteringPlayModeGraph);
                }
            }
        }

        static void CreatePreviewer()
        {
            Previewer = new DashEditorPreviewer();
        }

        static void CreateConfig()
        {
            Config = (DashEditorConfig) AssetDatabase.LoadAssetAtPath("Assets/Resources/DashEditorConfig.asset",
                typeof(DashEditorConfig));
            
            if (Config == null)
            {
                Config = ScriptableObject.CreateInstance<DashEditorConfig>();
                if (Config != null)
                {
                    if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                    {
                        AssetDatabase.CreateFolder("Assets", "Resources");
                    }
                    AssetDatabase.CreateAsset(Config, "Assets/Resources/DashEditorConfig.asset");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
        }

        static List<DashGraph> GetAllGraphs()
        {
            List<DashGraph> graphs = new List<DashGraph>();
            string[] graphGuids = AssetDatabase.FindAssets("t:DashGraph");
            foreach (string graphGuid in graphGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(graphGuid);
                DashGraph graph = AssetDatabase.LoadAssetAtPath<DashGraph>(path);
                graphs.Add(graph);
            }

            return graphs;
        }
        
        public static void RecacheAnimations()
        {
            GetAllGraphs().ForEach(g => g.RecacheAnimations());
        }
        
        public static Texture GetNodeIconByCategory(NodeCategoryType p_category)
        {
            switch (p_category)
            {
                case NodeCategoryType.EVENTS:
                    return IconManager.GetIcon("Event_Icon");
                case NodeCategoryType.ANIMATION:
                    return IconManager.GetIcon("Animation_Icon");
                case NodeCategoryType.RETARGET:
                    return IconManager.GetIcon("Retargeting_Icon");
                case NodeCategoryType.SPAWN:
                    return IconManager.GetIcon("Spawn_Icon");
                case NodeCategoryType.LOGIC:
                    return IconManager.GetIcon("Settings_Icon");
            }

            return null;
        }
        
        public static Color GetNodeBackgroundColorByCategory(NodeCategoryType p_category)
        {
            switch (p_category)
            {
                case NodeCategoryType.EVENTS:
                    return new Color(1f, 0.7f, 0.7f);
                case NodeCategoryType.ANIMATION:
                    return new Color(0.7f, 0.7f, 1f);
                case NodeCategoryType.RETARGET:
                    return new Color(0.7f, 1f, 1f);
                case NodeCategoryType.SPAWN:
                    return new Color(1f, 0.7f, 1f);
                case NodeCategoryType.LOGIC:
                    return Color.white;
            }

            return Color.white;
        }
        
        public static Color GetNodeTitleBackgroundColorByCategory(NodeCategoryType p_category)
        {
            switch (p_category)
            {
                case NodeCategoryType.EVENTS:
                    return new Color(0.8f, 0.5f, 0.5f);
                case NodeCategoryType.ANIMATION:
                    return new Color(0.5f, 0.5f, 0.8f);
                case NodeCategoryType.RETARGET:
                    return new Color(0.5f, 0.7f, 0.7f);
                case NodeCategoryType.SPAWN:
                    return new Color(0.7f, 0.5f, 0.7f);
                case NodeCategoryType.LOGIC:
                    return new Color(.6f, .6f, 0.7f);
            }

            return new Color(.6f,.6f,.7f);
        }
        
        public static Color GetNodeTitleTextColorByCategory(NodeCategoryType p_category)
        {
            switch (p_category)
            {
                case NodeCategoryType.EVENTS:
                    return new Color(1, 0.8f, 0.8f);
                case NodeCategoryType.ANIMATION:
                    return new Color(0.8f, 0.8f, 1f);
                case NodeCategoryType.RETARGET:
                    return new Color(0.8f, 1f, 1f);
                case NodeCategoryType.SPAWN:
                    return new Color(1f, 0.8f, 1f);
                case NodeCategoryType.LOGIC:
                    return new Color(.9f, .9f, 1f);
            }

            return new Color(.9f, .9f, 1f);
        }
        
        public static void DeselectAllNodes()
        {
            draggingNodes = false;
            selectedNodes.Clear();
        }

    }
}
#endif