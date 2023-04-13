/*
 *	Created by:  Peter @sHTiF Stefcek
 */
#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Dash.Editor
{
    public class SelectionManager
    {
        static public List<NodeBase> copiedNodes = new List<NodeBase>();
        
        static public List<int> selectedNodes = new List<int>();
        static public List<int> selectingNodes = new List<int>();
        
        static public GraphBox selectedBox;
        
        static public int connectingIndex = -1;
        static public NodeBase connectingNode = null;
        static public NodeConnectorType connectingType = NodeConnectorType.INPUT;
        static public Vector2 connectingPosition;
        
        static public int SelectedCount => selectedNodes == null ? 0 : selectedNodes.Count;

        public static bool IsConnecting()
        {
            return connectingNode != null;
        }
        
        public static void StartConnectionDrag(NodeBase p_node, int p_connectorIndex, NodeConnectorType p_connectorType, Vector2 p_mousePosition)
        {
            connectingNode = p_node;
            connectingIndex = p_connectorIndex;
            connectingType = p_connectorType;
            connectingPosition = p_mousePosition;
        }

        public static void EndConnectionDrag(NodeBase p_node = null, int p_index = -1)
        {
            if (p_node != null && p_index >= 0)
            {
                if (p_node != connectingNode)
                {
                    DashGraph graph = DashEditorCore.EditorConfig.editingGraph;
                    Undo.RegisterCompleteObjectUndo(graph, "Connect node");

                    switch (connectingType)
                    {
                        case NodeConnectorType.INPUT:
                            graph.Connect(connectingNode, SelectionManager.connectingIndex, p_node, p_index);
                            break;
                        case NodeConnectorType.OUTPUT:
                            graph.Connect(p_node, p_index, connectingNode, connectingIndex);
                            break;
                    }

                    DashEditorCore.SetDirty();
                }
            }

            connectingNode = null;
            connectingIndex = -1;
        }
        
        public static NodeBase GetSelectedNode(DashGraph p_graph)
        {
            if (p_graph == null)
                return null;

            return (selectedNodes != null && selectedNodes.Count == 1)
                ? selectedNodes[0] < p_graph.Nodes.Count ? p_graph.Nodes[selectedNodes[0]] : null
                : null;
        }

        public static bool IsSelected(int p_nodeIndex) => selectedNodes.Contains(p_nodeIndex);

        public static bool IsSelected(NodeBase p_node)
        {
            DashGraph graph = DashEditorCore.EditorConfig.editingGraph;
            
            return graph == null ? false : IsSelected(graph.Nodes.IndexOf(p_node)); 
        }
        
        public static bool IsSelecting(int p_nodeIndex) => selectingNodes.Contains(p_nodeIndex);

        public static void ClearSelection()
        {
            DashGraph graph = DashEditorCore.EditorConfig.editingGraph;
            if (graph != null)
            {
                selectedNodes.FindAll(i => i<graph.Nodes.Count).ForEach(n => graph.Nodes[n].Unselect());
            }
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

        public static void DragSelectedNodes(DashGraph p_graph, Vector2 p_delta)
        {
            selectedNodes.ForEach(n => p_graph.Nodes[n].rect.position += p_delta * p_graph.zoom);
        }
        
        public static void CopyNode(DashGraph p_graph, NodeBase p_node)
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
        
        public static void PasteNodes(DashGraph p_graph, Vector3 p_mousePosition)
        {
            if (p_graph == null || copiedNodes.Count == 0)
                return;
            
            List<NodeBase> newNodes = NodeUtils.DuplicateNodes(p_graph, copiedNodes);
            
            newNodes[0].rect = new Rect(p_mousePosition.x * p_graph.zoom - p_graph.viewOffset.x,
                p_mousePosition.y * p_graph.zoom - p_graph.viewOffset.y, 0, 0);

            for (int i = 1; i < newNodes.Count; i++)
            {
                NodeBase node = newNodes[i];
                node.rect.x = newNodes[0].rect.x + (node.rect.x - copiedNodes[0].rect.x);
                node.rect.y = newNodes[0].rect.y + (node.rect.y - copiedNodes[0].rect.y);
            }
            
            selectedNodes = newNodes.Select(n => n.Index).ToList();

            DashEditorCore.SetDirty();
        }

        public static void CreateSubGraphFromSelectedNodes(DashGraph p_graph)
        {
            if (p_graph == null || selectedNodes.Count == 0)
                return;
            
            UndoUtils.RegisterCompleteObject(p_graph, "Create SubGraph");

            List<NodeBase> nodes = selectedNodes.Select(i => p_graph.Nodes[i]).ToList();
            SubGraphNode subGraphNode = NodeUtils.PackNodesToSubGraph(p_graph, nodes);
            selectedNodes.Clear();
            selectedNodes.Add(subGraphNode.Index);
            
            DashEditorCore.SetDirty();
        }

        public static void UnpackSelectedSubGraphNode(DashGraph p_graph, SubGraphNode p_subGraphNode)
        {
            if (p_graph == null || p_subGraphNode == null)
                return;
            
            UndoUtils.RegisterCompleteObject(p_graph, "Unpack SubGraph");
            selectedNodes.Clear();
            
            NodeUtils.UnpackNodesFromSubGraph(p_graph, p_subGraphNode);
            
            DashEditorCore.SetDirty();
        }
        
        public static void DuplicateSelectedNodes(DashGraph p_graph)
        {
            if (p_graph == null || selectedNodes.Count == 0)
                return;
            
            UndoUtils.RegisterCompleteObject(p_graph, "Duplicate Nodes");

            List<NodeBase> nodes = selectedNodes.Select(i => p_graph.Nodes[i]).ToList();
            List<NodeBase> newNodes = NodeUtils.DuplicateNodes(p_graph, nodes);
            selectedNodes = newNodes.Select(n => n.Index).ToList();
            
            DashEditorCore.SetDirty();
        }
        
        public static void DuplicateNode(NodeBase p_node, DashGraph p_graph)
        {
            if (p_graph == null)
                return;
            
            UndoUtils.RegisterCompleteObject(p_graph, "Duplicate Node");

            NodeBase node = NodeUtils.DuplicateNode(p_graph,(NodeBase) p_node);
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
        
        public static void DeleteNode(DashGraph p_graph, NodeBase p_node)
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

        public static void SelectNode(DashGraph p_graph, NodeBase p_node, bool p_forceView = false)
        {
            selectedNodes.Clear();

            if (p_node == null || p_graph == null)
                return;

            selectedNodes.Add(p_node.Index);

            if (DashEditorCore.EditorConfig.editingController != null)
            {
                p_node.SelectEditorTarget();
            }

            if (p_forceView)
            {
                p_graph.viewOffset = -p_node.rect.center + p_graph.zoom * DashEditorCore.EditorConfig.editorPosition.size / 2;
            }
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
            SelectNode(p_graph, node);
            return node;
        }

        public static void ArrangeNodes(DashGraph p_graph, NodeBase p_node)
        {
            if (p_graph == null)
                return;
            
            UndoUtils.RegisterCompleteObject(p_graph, "Arrange Nodes");

            NodeUtils.ArrangeNodes(p_graph, p_node);
        
            DashEditorCore.SetDirty();
        }
        
        public static void SelectConnectedNodes(DashGraph p_graph, NodeBase p_node)
        {
            if (p_graph == null)
                return;

            SelectNode(p_graph, p_node);

            SelectOutputs(p_graph, p_node);
        }

        public static void SelectOutputs(DashGraph p_graph, NodeBase p_node)
        {
            var connections = p_graph.Connections.FindAll(c => c.outputNode == p_node);
            connections.ForEach(c =>
            {
                if (!IsSelected(c.inputNode))
                {
                    AddNodeToSelection(c.inputNode.Index);
                    SelectOutputs(p_graph, c.inputNode);
                }
            });
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
#endif