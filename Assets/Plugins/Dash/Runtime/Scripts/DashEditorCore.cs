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
using UnityEngine.Experimental.Rendering;

namespace Dash
{
  
    [Serializable]
    [InitializeOnLoad]
    public class DashEditorCore
    {
        static public DashRuntimeConfig RuntimeConfig { get; private set; }
        
        static public DashEditorConfig EditorConfig { get; private set; }

        static public DashEditorPreviewer Previewer { get; private set; }

        static public GUISkin Skin => (GUISkin)Resources.Load("Skins/EditorSkins/NodeEditorSkin");
        
        static public DashGraph Graph => EditorConfig.editingGraph;

        static public int TITLE_TAB_HEIGHT = 26;

        static public int CONNECTOR_HEIGHT = 24;

        static public int CONNECTOR_PADDING = 4;

        static public GraphBox editingBoxComment;
        static public GraphBox selectedBox;

        static public List<Variable> copiedVariables = new List<Variable>();
        
        static public List<NodeBase> copiedNodes = new List<NodeBase>();
        static public List<int> selectedNodes = new List<int>();
        static public List<int> selectingNodes = new List<int>();

        static public bool DetailsVisible => EditorConfig.zoom < 2.5;

        static public string propertyReference;

        static DashEditorCore()
        {
            SetExecutionOrder(typeof(DashGlobalVariables), -501);
            SetExecutionOrder(typeof(DashController), -500);
            
            CreateEditorConfig();
            CreateRuntimeConfig();
            CreatePreviewer();

            CheckDashVersion();
            
            //InvalidateControllersIds();

            EditorApplication.playModeStateChanged += OnPlayModeChanged;
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
        }

        static void CheckDashVersion()
        {
            if (EditorConfig.lastUsedVersion == 0 || DashRuntimeCore.GetVersionNumber() > EditorConfig.lastUsedVersion)
            {
                
            }

            EditorConfig.lastUsedVersion = DashRuntimeCore.GetVersionNumber();
        }

        static void SetExecutionOrder(Type p_classType, int p_order)
        {
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
            }
            else
            {
                if (Graph.Controller != null)
                {
                    EditorUtility.SetDirty(Graph.Controller);
                }

                EditorUtility.SetDirty(Graph);
            }
        }

        public static NodeBase Search(string p_search, int p_index)
        {
            if (Graph == null)
                return null;

            var searchNodes = Graph.Nodes.FindAll(n => n.Id.ToLower().Contains(p_search)).ToList();
            if (searchNodes.Count == 0)
                return null;
            
            if (p_index >= searchNodes.Count) p_index = p_index%searchNodes.Count;

            var node = searchNodes[p_index];
            SelectNode(node);
            return node;
        }
        
        public static bool GoToNode(DashController p_controller, string p_graphPath, string p_nodeId)
        {
            if (p_controller == null)
                return false;
            
            EditController(p_controller, p_graphPath);

            var graph = DashEditorCore.EditorConfig.editingGraph;
            if (graph == null)
                return false;
            
            var node = graph.Nodes.Find(n => n.Id == p_nodeId);
            if (node == null)
                return false;
            
            SelectNode(node);

            return true;
        }

        public static void SelectNode(NodeBase p_node)
        {
            selectedNodes.Clear();

            if (p_node == null || Graph == null)
                return;
            
            selectedNodes.Add(p_node.Index);
            Graph.viewOffset = -p_node.rect.center + EditorConfig.zoom * EditorConfig.editorPosition.size / 2;
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
            
            float zoom = EditorConfig.zoom;
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
                EditorConfig.editingGraphPath = p_graphPath;
                EditorConfig.editingGraph = p_controller.GetGraphAtPath(p_graphPath);

                if (Graph != null)
                {
                    ((IEditorGraphAccess) Graph).SetController(p_controller);
                }
            }
            else
            {
                EditorConfig.editingGraph = null;
            }
        }
        
        public static void FetchGlobalVariables()
        {
            var components = GameObject.FindObjectsOfType<DashGlobalVariables>();
            if (components.Length > 1)
            {
                Debug.LogWarning("Multiple global variables found, only first instance used.");
            }
            
            if (components.Length > 0)
            {
                DashRuntimeCore.Instance.SetGlobalVariables(components[0]);
            }
            else
            {
                DashRuntimeCore.Instance.SetGlobalVariables(null);
            }
        }

        public static void EditGraph(DashGraph p_graph)
        {
            selectedNodes.Clear();
            
            EditorConfig.editingGraphPath = "";
            EditorConfig.editingGraph = p_graph;
            if (p_graph != null)
                ((IEditorGraphAccess)p_graph).SetController(null);
        }
        
        public static void UnloadGraph()
        {
            selectedNodes.Clear();
            
            EditorConfig.editingGraph = null;
        }
        
        static void OnHierarchyChanged()
        {
            if (EditorApplication.isPlaying || Previewer.IsPreviewing)
                return;

            // InvalidateControllersIds();
        }

        // static void InvalidateControllersIds()
        // {
        //     var found = GameObject.FindObjectsOfType<DashController>(true).ToList();
        //     if (found.Count == 0)
        //         return;
        //
        //     foreach (var controller in found)
        //     {
        //         var cid = controller.Id;
        //
        //         while (found.Exists(dc => dc != controller && dc.Id == cid))
        //         {
        //             int number = cid.Length > 2 ? Int32.Parse(cid.Substring(2)) : 0;
        //             cid = "DC" + (number + 1);
        //         }
        //     
        //         controller.Id = cid;
        //     }
        // }

        static void OnAfterAssemblyReload()
        {
            // Debug.Log("OnAfterAssemblyReload");
            
            if (Graph != null && Graph.Controller != null)
            {
                EditController(Graph.Controller, EditorConfig.editingGraphPath);
            }
        }

        static void OnBeforeAssemblyReload()
        {
            // Debug.Log("OnBeforeAssemblyReload");
        }
        
        static void OnPlayModeChanged(PlayModeStateChange p_change)
        {
            //Debug.Log("[PLAYMODECHANGE] "+p_change);
            
            if (p_change == PlayModeStateChange.ExitingEditMode)
            {
                EditorConfig.enteringPlayModeController = Graph != null ? Graph.Controller : null;
            }
            
            if (p_change == PlayModeStateChange.EnteredPlayMode)
            {
                if (EditorConfig.enteringPlayModeController != null)
                {
                    EditController(EditorConfig.enteringPlayModeController);
                }
            }

            if (p_change == PlayModeStateChange.EnteredEditMode)
            {
                DashTweenCore.Reset();
                
                if (EditorConfig.enteringPlayModeController != null)
                {
                    EditController(EditorConfig.enteringPlayModeController);    
                }
                else
                {
                    EditGraph(EditorConfig.enteringPlayModeGraph);
                }
            }
        }

        static void CreatePreviewer()
        {
            Previewer = new DashEditorPreviewer();
        }

        static void CreateRuntimeConfig()
        {
            RuntimeConfig = (DashRuntimeConfig) AssetDatabase.LoadAssetAtPath("Assets/Resources/DashRuntimeConfig.asset",
                typeof(DashRuntimeConfig));
            
            if (RuntimeConfig == null)
            {
                RuntimeConfig = ScriptableObject.CreateInstance<DashRuntimeConfig>();
                if (RuntimeConfig != null)
                {
                    if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                    {
                        AssetDatabase.CreateFolder("Assets", "Resources");
                    }
                    AssetDatabase.CreateAsset(RuntimeConfig, "Assets/Resources/DashRuntimeConfig.asset");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
        }
        
        static void CreateEditorConfig()
        {
            EditorConfig = (DashEditorConfig) AssetDatabase.LoadAssetAtPath("Assets/Resources/Editor/DashEditorConfig.asset",
                typeof(DashEditorConfig));
            
            if (EditorConfig == null)
            {
                EditorConfig = ScriptableObject.CreateInstance<DashEditorConfig>();
                if (EditorConfig != null)
                {
                    if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                    {
                        AssetDatabase.CreateFolder("Assets", "Resources");
                        AssetDatabase.CreateFolder("Assets/Resources", "Editor");
                    } 
                    else if (!AssetDatabase.IsValidFolder("Assets/Resources/Editor"))
                    {
                        AssetDatabase.CreateFolder("Assets/Resources", "Editor");
                    }
                    AssetDatabase.CreateAsset(EditorConfig, "Assets/Resources/Editor/DashEditorConfig.asset");
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
    }
}
#endif