/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Dash.Attributes;
using UnityEditor;
using UnityEngine;
using LinqExtensions = OdinSerializer.Utilities.LinqExtensions;

namespace Dash
{
    public class NodeUtils
    {
        public static bool CanHaveMultipleInstances(Type p_nodeType)
        {
            SingleInstanceAttribute attribute = (SingleInstanceAttribute) Attribute.GetCustomAttribute(p_nodeType, typeof(SingleInstanceAttribute));
            return attribute == null;
        }

        public static string CategoryToString(NodeCategoryType p_type)
        {
            return p_type.ToString().Substring(0, 1) + p_type.ToString().Substring(1).ToLower();
        }
#if UNITY_EDITOR        
        public static NodeBase CreateNode(DashGraph p_graph, Type p_nodeType, Vector2 p_position)
        {
            if (!NodeUtils.CanHaveMultipleInstances(p_nodeType) && p_graph.GetNodeByType(p_nodeType) != null)
                return null;
            
            Undo.RegisterCompleteObjectUndo(p_graph, "Create "+NodeBase.GetNodeNameFromType(p_nodeType));
            
            NodeBase node = NodeBase.Create(p_nodeType, p_graph);

            if (node != null)
            {
                float zoom = DashEditorCore.EditorConfig.zoom;
                node.rect = new Rect(p_position.x, p_position.y, 0, 0);
                p_graph.Nodes.Add(node);
            }

            //Debug.Log(EditorUtility.IsDirty(p_graph));
            DashEditorCore.SetDirty();

            return node;
        }
        
        public static NodeBase DuplicateNode(DashGraph p_graph, NodeBase p_node)
        {
            NodeBase clone = p_node.Clone(p_graph);
            clone.rect = new Rect(p_node.rect.x + 20, p_node.rect.y + 20, 0, 0);
            p_graph.Nodes.Add(clone);
            return clone;
        }
        
        public static List<NodeBase> DuplicateNodes(DashGraph p_graph, List<NodeBase> p_nodes)
        {
            if (p_nodes == null || p_nodes.Count == 0)
                return null;

            List<NodeBase> newNodes = new List<NodeBase>();
            foreach (NodeBase node in p_nodes)
            {
                NodeBase clone = node.Clone(p_graph);
                clone.rect = new Rect(node.rect.x + 20, node.rect.y + 20, 0, 0);
                p_graph.Nodes.Add(clone);
                newNodes.Add(clone);
            }

            DashGraph originalGraph = p_nodes[0].Graph;
            // Recreate connections within duplicated part
            foreach (NodeBase node in p_nodes)
            {
                List<NodeConnection> connections =
                    originalGraph.Connections.FindAll(c => c.inputNode == node && p_nodes.Contains(c.outputNode));
                foreach (NodeConnection connection in connections)
                {
                    p_graph.Connect(newNodes[p_nodes.IndexOf(connection.inputNode)], connection.inputIndex,
                        newNodes[p_nodes.IndexOf(connection.outputNode)], connection.outputIndex);
                }
            }

            return newNodes;
        }

        public static SubGraphNode PackNodesToSubGraph(DashGraph p_graph, List<NodeBase> p_nodes)
        {
            Vector2 center = Vector2.zero;
            p_nodes.ForEach(n => center += n.rect.center);
            center /= p_nodes.Count;

            SubGraphNode subGraphNode = (SubGraphNode)CreateNode(p_graph, typeof(SubGraphNode), center);
            List<NodeBase> newNodes = DuplicateNodes(subGraphNode.SubGraph, p_nodes);

            List<NodeConnection> inputs = new List<NodeConnection>();
            List<NodeConnection> outputs = new List<NodeConnection>();
            p_nodes.ForEach(node =>
            {
                // Check for inputs
                inputs = inputs.Concat(p_graph.Connections.FindAll(c =>
                {
                    bool valid = c.inputNode == node;
                    p_nodes.ForEach(n => valid = valid && n != c.outputNode);
                    return valid;
                })).ToList();

                outputs = outputs.Concat(p_graph.Connections.FindAll(c =>
                {
                    bool valid = c.outputNode == node;
                    p_nodes.ForEach(n => valid = valid && n != c.inputNode);
                    return valid;
                })).ToList();
            });

            NodeBase previousNode = null;
            int index = 0;
            foreach (var connection in inputs)
            {
                if (previousNode != connection.inputNode)
                {
                    InputNode inputNode = (InputNode)CreateNode(subGraphNode.SubGraph, typeof(InputNode),
                        connection.inputNode.rect.position - new Vector2(200, index * 100));
                    inputNode.Model.inputName = "Input" + (index + 1);
                    subGraphNode.SubGraph.Connect(newNodes[p_nodes.IndexOf(connection.inputNode)],
                        connection.inputIndex, inputNode, 0);
                    p_graph.Connect(subGraphNode, index++, connection.outputNode, connection.outputIndex);
                    previousNode = connection.inputNode;
                }
                else
                {
                    p_graph.Connect(subGraphNode, index - 1, connection.outputNode, connection.outputIndex);
                }
            }

            previousNode = null;
            index = 0;
            foreach (var connection in outputs)
            {
                if (previousNode != connection.outputNode)
                {
                    OutputNode outputNode = (OutputNode)CreateNode(subGraphNode.SubGraph, typeof(OutputNode),
                        connection.outputNode.rect.position + new Vector2(300, index * 100));
                    outputNode.Model.outputName = "Output" + (index + 1);
                    subGraphNode.SubGraph.Connect(outputNode, 0, newNodes[p_nodes.IndexOf(connection.outputNode)],
                        connection.outputIndex);
                    p_graph.Connect(connection.inputNode, connection.inputIndex, subGraphNode, index++);
                    previousNode = connection.outputNode;
                }
                else
                {
                    p_graph.Connect(connection.inputNode, connection.inputIndex, subGraphNode, index - 1);
                }
            }
            
            p_nodes.ForEach(n => p_graph.DeleteNode(n));
            
            return subGraphNode;
        }

        public static void UnpackNodesFromSubGraph(DashGraph p_graph, SubGraphNode p_subGraphNode)
        {
            List<NodeConnection> oldInputConnections =
                p_graph.Connections.FindAll(c => c.inputNode == p_subGraphNode);
            List<NodeConnection> oldOutputConnections =
                p_graph.Connections.FindAll(c => c.outputNode == p_subGraphNode);

            List<NodeBase> newNodes = DuplicateNodes(p_graph, p_subGraphNode.SubGraph.Nodes);
            List<NodeBase> inputNodes = newNodes.FindAll(n => n is InputNode);
            List<NodeBase> outputNodes = newNodes.FindAll(n => n is OutputNode);

            foreach (var connection in oldInputConnections)
            {
                var oldConnection = p_graph.Connections.Find(c => c.outputNode == inputNodes[connection.inputIndex]);
                p_graph.Connect(oldConnection.inputNode, oldConnection.inputIndex, connection.outputNode,
                        connection.outputIndex);
            }
            
            foreach (var connection in oldOutputConnections)
            {
                var oldConnection = p_graph.Connections.Find(c => c.inputNode == outputNodes[connection.outputIndex]);
                p_graph.Connect(connection.inputNode, connection.inputIndex, oldConnection.outputNode,
                    oldConnection.outputIndex);
            }
            
            inputNodes.ForEach(n => p_graph.DeleteNode(n));
            outputNodes.ForEach(n => p_graph.DeleteNode(n));
            p_graph.DeleteNode(p_subGraphNode);
        }
#endif
    }
}