/*
 *	Created by:  Peter @sHTiF Stefcek
 */

#if NET_STANDARD_2_0
#error Dash is unable to compile with .NET Standard 2.0 API. Change the API Compatibility Level in the Player settings.
#endif

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Dash
{
  
    [Serializable]
    [InitializeOnLoad]
    public class DashEditorCore
    {
        public static Action<string> OnDebugMessage;

        static public DashEditorConfig Config { get; private set; }

        static public DashEditorPreviewer Previewer { get; private set; }

        static public GUISkin Skin => (GUISkin)Resources.Load("Skins/EditorSkins/NodeEditorSkin");
        
        static public DashGraph Graph => Config.editingGraph;

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

        static public GraphBox editingBoxComment;
        static public GraphBox selectedBox;

        static public List<Variable> copiedVariables = new List<Variable>();
        
        static public List<NodeBase> copiedNodes = new List<NodeBase>();
        static public List<int> selectedNodes = new List<int>();
        static public List<int> selectingNodes = new List<int>();

        static public bool DetailsVisible => Config.zoom < 2.5;

        static public List<string> DebugList { get; private set; }

        static public string propertyReference;

        static DashEditorCore()
        {
            SetExecutionOrder(typeof(DashGlobalVariables), -501);
            SetExecutionOrder(typeof(DashController), -500);
            
            CreateConfig();
            CreatePreviewer();

            EditorApplication.playModeStateChanged += OnPlayModeChanged;
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
            AssemblyReloadEvents.afterAssemblyReload += OnAssemblyReload;
        }

        static public void Debug(string p_debug)
        {
            if (DebugList == null)
            {
                DebugList = new List<string>();
            }
            
            DebugList.Add(p_debug);
            
            OnDebugMessage?.Invoke(p_debug);
        }
        
        static void SetExecutionOrder(Type p_classType, int p_order){
            MonoScript[] scripts = (MonoScript[])Resources.FindObjectsOfTypeAll(typeof(MonoScript));
            
            MonoScript classScript = scripts.First(s => s.GetClass() == p_classType);

            // We need to check the order first and set only if different otherwise we may get into infinity reload assembly loop.
            if (MonoImporter.GetExecutionOrder(classScript) != p_order)
            {
                MonoImporter.SetExecutionOrder(classScript, p_order);
            }
        }

        public static void SetDirty()
        {
            if (Graph == null)
                return;

            if (Graph.IsBound)
            {
                Graph.Controller.ReserializeBound();
                EditorUtility.SetDirty(Graph.Controller);
                //EditorSceneManager.MarkSceneDirty(Graph.Controller.gameObject.scene);
            }
            else
            {
                EditorUtility.SetDirty(Graph);
            }
        }

        public static NodeBase Search(string p_search, int p_index)
        {
            if (Graph == null)
                return null;
            
            selectedNodes.Clear();

            var searchNodes = Graph.Nodes.FindAll(n => n.Id.ToLower().Contains(p_search)).ToList();
            if (searchNodes.Count == 0)
                return null;
            
            if (p_index >= searchNodes.Count) p_index = p_index%searchNodes.Count;

            var node = searchNodes[p_index];
            selectedNodes.Add(node.Index);
            return node;
        }

        public static void DuplicateSelectedNodes()
        {
            if (Graph == null || selectedNodes.Count == 0)
                return;
            
            Undo.RegisterCompleteObjectUndo(Graph, "Duplicate Nodes");
            
            List<NodeBase> nodes = selectedNodes.Select(i => Graph.Nodes[i]).ToList();
            List<NodeBase> newNodes = Graph.DuplicateNodes(nodes);
            selectedNodes = newNodes.Select(n => n.Index).ToList();
            
            SetDirty();
        }
        
        public static void DuplicateNode(NodeBase p_node)
        {
            if (Graph == null)
                return;
            Undo.RegisterCompleteObjectUndo(Graph, "Duplicate Node");
            
            NodeBase node = Graph.DuplicateNode((NodeBase) p_node);
            selectedNodes = new List<int> { node.Index };
            
            SetDirty();
        }
        
        public static void CopySelectedNodes()
        {
            if (Graph == null || selectedNodes.Count == 0)
                return;

            copiedNodes = selectedNodes.Select(i => Graph.Nodes[i]).ToList();
        }
        
        public static void CopyNode(NodeBase p_node)
        {
            if (Graph == null)
                return;

            copiedNodes.Clear();
            copiedNodes.Add(p_node);
        }

        public static bool HasCopiedNodes()
        {
            return copiedNodes.Count != 0;
        }
        
        public static void PasteNodes(Vector2 p_mousePosition)
        {
            if (Graph == null || copiedNodes.Count == 0)
                return;
            
            List<NodeBase> newNodes = Graph.DuplicateNodes(copiedNodes);
            
            float zoom = Config.zoom;
            newNodes[0].rect = new Rect(p_mousePosition.x * zoom - Graph.viewOffset.x,
                p_mousePosition.y * zoom - Graph.viewOffset.y, 0, 0);

            for (int i = 1; i < newNodes.Count; i++)
            {
                NodeBase node = newNodes[i];
                node.rect.x = newNodes[0].rect.x + (node.rect.x - copiedNodes[0].rect.x);
                node.rect.y = newNodes[0].rect.y + (node.rect.y - copiedNodes[0].rect.y);
            }
            
            selectedNodes = newNodes.Select(n => n.Index).ToList();

            SetDirty();
        }

        public static void DeleteSelectedNodes()
        {
            if (Graph == null || selectedNodes.Count == 0)
                return;
            
            Undo.RegisterCompleteObjectUndo(Graph, "Delete Nodes");

            var nodes = selectedNodes.Select(i => Graph.Nodes[i]).ToList();
            nodes.ForEach(n => Graph.DeleteNode(n));

            selectedNodes = new List<int>();
            
            SetDirty();
        }
        
        public static void DeleteNode(NodeBase p_node)
        {
            if (Graph == null)
                return;
            
            Undo.RegisterCompleteObjectUndo(Graph, "Delete Node");

            int index = p_node.Index;
            Graph.DeleteNode(p_node);
            selectedNodes.Remove(index);
            ReindexSelected(index);
            
            SetDirty();
        }

        public static void CopyVariables(DashVariables p_fromVariables)
        {
            copiedVariables.Clear();
            foreach (var variable in p_fromVariables)
            {
                copiedVariables.Add(variable);
            }
        }
        
        public static void CopyVariable(Variable p_variable)
        {
            copiedVariables.Clear();
            copiedVariables.Add(p_variable);
        }

        public static void PasteVariables(DashVariables p_toVariables, GameObject p_target)
        {
            copiedVariables.ForEach(v => p_toVariables.PasteVariable(v.Clone(), p_target));
        }

        public static void CreateBoxAroundSelectedNodes()
        {
            List<NodeBase> nodes = selectedNodes.Select(i => Graph.Nodes[i]).ToList();
            Rect region = nodes[0].rect;
            
            nodes.ForEach(n =>
            {
                if (n.rect.xMin < region.xMin) region.xMin = n.rect.xMin;
                if (n.rect.yMin < region.yMin) region.yMin = n.rect.yMin;
                if (n.rect.xMax > region.xMax) region.xMax = n.rect.xMax;
                if (n.rect.yMax > region.yMax) region.yMax = n.rect.yMax;
            });

            Graph.CreateBox(region);
            
            SetDirty();
        }

        public static void ReindexSelected(int p_index)
        {
            for (int i = 0; i < selectedNodes.Count; i++)
            {
                if (selectedNodes[i] > p_index)
                    selectedNodes[i]--;
            }
        }
        
        public static void EditController(DashController p_controller, string p_graphPath = "")
        {
            selectedNodes.Clear();
            
            if (p_controller != null)
            {
                Config.editingGraphPath = p_graphPath;
                Config.editingGraph = p_controller.GetGraphAtPath(p_graphPath);
                
                if (Graph != null)
                {
                    ((IEditorGraphAccess) Graph).SetController(p_controller);
                }
            }
            else
            {
                Config.editingGraph = null;
            }
        }

        public static void EditGraph(DashGraph p_graph)
        {
            selectedNodes.Clear();
            
            Config.editingGraphPath = "";
            Config.editingGraph = p_graph;
            if (p_graph != null)
                ((IEditorGraphAccess)p_graph).SetController(null);
        }
        
        public static void UnloadGraph()
        {
            selectedNodes.Clear();
            
            Config.editingGraph = null;
        }
        
        static void OnHierarchyChanged()
        {
            // Debug.Log("OnHierarchyChanged");
        }

        static void OnAssemblyReload()
        {
            if (Graph != null && Graph.Controller != null)
            {
                EditController(Graph.Controller, Config.editingGraphPath);
            }
        }
        
        static void OnPlayModeChanged(PlayModeStateChange p_change)
        {
            // Debug.Log("[PLAYMODECHANGE] "+p_change);
            
            if (p_change == PlayModeStateChange.ExitingEditMode)
            {
                Config.enteringPlayModeController = Graph != null ? Graph.Controller : null;
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
                DashTweenCore.Reset();
                
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
        
        static List<DashAnimation> GetAllAnimations()
        {
            List<DashAnimation> animations = new List<DashAnimation>();
            string[] graphGuids = AssetDatabase.FindAssets("t:DashAnimation");
            foreach (string graphGuid in graphGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(graphGuid);
                DashAnimation animation = AssetDatabase.LoadAssetAtPath<DashAnimation>(path);
                animations.Add(animation);
            }

            return animations;
        }
        
        public static void RecacheAnimation()
        {
            // Extract all, think about extracting changed only later (could be problematic to match)
            GetAllAnimations().ForEach(a => a.Reextract());
        }
        
        public static Texture GetNodeIconByCategory(NodeCategoryType p_category)
        {
            switch (p_category)
            {
                case NodeCategoryType.EVENT:
                    return IconManager.GetIcon("Event_Icon");
                case NodeCategoryType.ANIMATION:
                    return IconManager.GetIcon("Animation_Icon");
                case NodeCategoryType.MODIFIER:
                    return IconManager.GetIcon("Retargeting_Icon");
                case NodeCategoryType.CREATION:
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
                case NodeCategoryType.EVENT:
                    return new Color(1f, 0.7f, 0.7f);
                case NodeCategoryType.ANIMATION:
                    return new Color(0.7f, 0.7f, 1f);
                case NodeCategoryType.MODIFIER:
                    return new Color(0.7f, 1f, 1f);
                case NodeCategoryType.CREATION:
                    return new Color(1f, 0.7f, 1f);
                case NodeCategoryType.GRAPH:
                    return new Color(0.8f, 0.6f, 0f);
                case NodeCategoryType.LOGIC:
                    return Color.white;
            }

            return Color.white;
        }
        
        public static Color GetNodeTitleBackgroundColorByCategory(NodeCategoryType p_category)
        {
            switch (p_category)
            {
                case NodeCategoryType.EVENT:
                    return new Color(0.8f, 0.5f, 0.5f);
                case NodeCategoryType.ANIMATION:
                    return new Color(0.5f, 0.5f, 0.8f);
                case NodeCategoryType.MODIFIER:
                    return new Color(0.5f, 0.7f, 0.7f);
                case NodeCategoryType.CREATION:
                    return new Color(0.7f, 0.5f, 0.7f);
                case NodeCategoryType.GRAPH:
                    return new Color(0.8f, 0.5f, 0f);
                case NodeCategoryType.LOGIC:
                    return new Color(.6f, .6f, 0.7f);
            }

            return new Color(.6f,.6f,.7f);
        }
        
        public static Color GetNodeTitleTextColorByCategory(NodeCategoryType p_category)
        {
            switch (p_category)
            {
                case NodeCategoryType.EVENT:
                    return new Color(1, 0.8f, 0.8f);
                case NodeCategoryType.ANIMATION:
                    return new Color(0.8f, 0.8f, 1f);
                case NodeCategoryType.MODIFIER:
                    return new Color(0.8f, 1f, 1f);
                case NodeCategoryType.CREATION:
                    return new Color(1f, 0.8f, 1f);
                case NodeCategoryType.GRAPH:
                    return new Color(1f, 0.8f, 0.5f);
                case NodeCategoryType.LOGIC:
                    return new Color(.9f, .9f, 1f);
            }

            return new Color(.9f, .9f, 1f);
        }

    }
}
#endif