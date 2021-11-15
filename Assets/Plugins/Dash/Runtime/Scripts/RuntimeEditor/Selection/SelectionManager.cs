/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dash
{
    public class SelectionManager
    {
        static public float zoom => DashEditorCore.EditorConfig.zoom;
        
        static public List<NodeBase> copiedNodes = new List<NodeBase>();
        static public List<int> selectedNodes = new List<int>();
        static public List<int> selectingNodes = new List<int>();

        static public int SelectedCount => selectedNodes == null ? 0 : selectedNodes.Count;
        
        public static NodeBase GetSelectedNode(DashGraph p_graph)
        {
            if (p_graph == null)
                return null;

            return (selectedNodes != null && selectedNodes.Count == 1)
                ? p_graph.Nodes[selectedNodes[0]]
                : null;
        }

        public static bool IsSelected(int p_nodeIndex) => selectedNodes.Contains(p_nodeIndex);
        
        public static bool IsSelecting(int p_nodeIndex) => selectingNodes.Contains(p_nodeIndex);

        public static void ClearSelection()
        {
            selectedNodes.Clear();
        }
        
        public static void ReindexSelected(int p_index)
        {
            for (int i = 0; i < selectedNodes.Count; i++)
            {
                if (selectedNodes[i] > p_index)
                    selectedNodes[i]--;
            }
        }

        public static void DragSelectedNodes(Vector2 p_delta, DashGraph p_graph)
        {
            selectedNodes.ForEach(n => p_graph.Nodes[n].rect.position += p_delta*zoom);
        }
        
        public static void CopyNode(NodeBase p_node, DashGraph p_graph)
        {
            if (p_graph == null)
                return;

            copiedNodes.Clear();
            copiedNodes.Add(p_node);
        }

        public static bool HasCopiedNodes()
        {
            return copiedNodes.Count != 0;
        }
        
        public static void PasteNodes(Vector3 p_mousePosition, DashGraph p_graph)
        {
            if (p_graph == null || copiedNodes.Count == 0)
                return;
            
            List<NodeBase> newNodes = p_graph.DuplicateNodes(copiedNodes);
            
            newNodes[0].rect = new Rect(p_mousePosition.x * zoom - p_graph.viewOffset.x,
                p_mousePosition.y * zoom - p_graph.viewOffset.y, 0, 0);

            for (int i = 1; i < newNodes.Count; i++)
            {
                NodeBase node = newNodes[i];
                node.rect.x = newNodes[0].rect.x + (node.rect.x - copiedNodes[0].rect.x);
                node.rect.y = newNodes[0].rect.y + (node.rect.y - copiedNodes[0].rect.y);
            }
            
            selectedNodes = newNodes.Select(n => n.Index).ToList();

            DashEditorCore.SetDirty();
        }

        public static void DuplicateSelectedNodes(DashGraph p_graph)
        {
            if (p_graph == null || selectedNodes.Count == 0)
                return;
            
            UndoUtils.RegisterCompleteObject(p_graph, "Duplicate Nodes");

            List<NodeBase> nodes = selectedNodes.Select(i => p_graph.Nodes[i]).ToList();
            List<NodeBase> newNodes = p_graph.DuplicateNodes(nodes);
            selectedNodes = newNodes.Select(n => n.Index).ToList();
            
            DashEditorCore.SetDirty();
        }
        
        public static void DuplicateNode(NodeBase p_node, DashGraph p_graph)
        {
            if (p_graph == null)
                return;
            
            UndoUtils.RegisterCompleteObject(p_graph, "Duplicate Node");

            NodeBase node = p_graph.DuplicateNode((NodeBase) p_node);
            selectedNodes = new List<int> { node.Index };
            
            DashEditorCore.SetDirty();
        }
        
        public static void CopySelectedNodes(DashGraph p_graph)
        {
            if (p_graph == null || selectedNodes.Count == 0)
                return;

            copiedNodes = selectedNodes.Select(i => p_graph.Nodes[i]).ToList();
        }
        
        public static void CreateBoxAroundSelectedNodes(DashGraph p_graph)
        {
            List<NodeBase> nodes = selectedNodes.Select(i => p_graph.Nodes[i]).ToList();
            Rect region = nodes[0].rect;
            
            nodes.ForEach(n =>
            {
                if (n.rect.xMin < region.xMin) region.xMin = n.rect.xMin;
                if (n.rect.yMin < region.yMin) region.yMin = n.rect.yMin;
                if (n.rect.xMax > region.xMax) region.xMax = n.rect.xMax;
                if (n.rect.yMax > region.yMax) region.yMax = n.rect.yMax;
            });

            p_graph.CreateBox(region);
            
            DashEditorCore.SetDirty();
        }
        
        public static void DeleteSelectedNodes(DashGraph p_graph)
        {
            if (p_graph == null || selectedNodes.Count == 0)
                return;
            
            UndoUtils.RegisterCompleteObject(p_graph, "Delete Nodes");

            var nodes = selectedNodes.Select(i => p_graph.Nodes[i]).ToList();
            nodes.ForEach(n => p_graph.DeleteNode(n));

            selectedNodes = new List<int>();
            
            DashEditorCore.SetDirty();
        }
        
        public static void DeleteNode(NodeBase p_node, DashGraph p_graph)
        {
            if (p_graph == null)
                return;
            
            UndoUtils.RegisterCompleteObject(p_graph, "Delete Node");

            int index = p_node.Index;
            p_graph.DeleteNode(p_node);
            selectedNodes.Remove(index);
            ReindexSelected(index);
            
            DashEditorCore.SetDirty();
        }

        public static void AddNodeToSelection(int p_nodeIndex)
        {
            selectedNodes.Add(p_nodeIndex);
        }
        
        public static void SelectNode(NodeBase p_node, DashGraph p_graph)
        {
            selectedNodes.Clear();

            if (p_node == null || p_graph == null)
                return;
            
            selectedNodes.Add(p_node.Index);
            p_graph.viewOffset = -p_node.rect.center + zoom * DashEditorCore.EditorConfig.editorPosition.size / 2;
        }

        public static void SelectingNodes(List<int> p_nodes)
        {
            selectingNodes = p_nodes;
        }

        public static void SelectingToSelected()
        {
            selectedNodes.Clear();
            selectedNodes.AddRange(selectingNodes);
            selectingNodes.Clear();
        }
        
        public static NodeBase SearchAndSelectNode(DashGraph p_graph, string p_search, int p_index)
        {
            if (p_graph == null)
                return null;

            var searchNodes = p_graph.Nodes.FindAll(n => n.Id.ToLower().Contains(p_search)).ToList();
            if (searchNodes.Count == 0)
                return null;
            
            if (p_index >= searchNodes.Count) p_index = p_index%searchNodes.Count;

            var node = searchNodes[p_index];
            SelectNode(node, p_graph);
            return node;
        }
        
        // public static bool GoToNode(DashController p_controller, string p_graphPath, string p_nodeId)
        // {
        //     if (p_controller == null)
        //         return false;
        //     
        //     EditController(p_controller, p_graphPath);
        //
        //     var graph = DashEditorCore.EditorConfig.editingGraph;
        //     if (graph == null)
        //         return false;
        //     
        //     var node = graph.Nodes.Find(n => n.Id == p_nodeId);
        //     if (node == null)
        //         return false;
        //     
        //     SelectionManager.SelectNode(node, graph);
        //
        //     return true;
        // }
    }
}