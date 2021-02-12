/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Dash.Attributes;
using OdinSerializer.Utilities;
using UnityEditor;
using UnityEngine;
using Object = System.Object;

namespace Dash
{
    public enum GraphContextMenuType
    {
        GRAPH,
        NODE,
        CONNECTION
    }
    
    public enum GraphContextMenuItemType
    {
        CREATE_GRAPH,
        LOAD_GRAPH,
        CREATE_NODE,
        DELETE_NODE,
        DELETE_CONNECTION
    }
    
    public class GraphContextMenu
    {
        static private DashGraph Graph => DashEditorCore.Config.editingGraph;
        
        static private Vector2 _lastMousePosition;
        static public void Show(GraphContextMenuType p_type, Event p_event, object p_object)
        {
            _lastMousePosition = Event.current.mousePosition;
            GenericMenu menu = new GenericMenu();

            if (p_type == GraphContextMenuType.GRAPH)
            {
                menu.AddItem(new GUIContent("Create Graph"), false, CreateGraph);
                menu.AddItem(new GUIContent("Load Graph"), false, LoadGraph);

                menu.AddSeparator("");
                if (DashEditorCore.Config.editingGraph != null)
                {
                    List<Type> nodeTypes = ReflectionUtils.GetAllTypes(typeof(NodeBase));
                    foreach (Type type in nodeTypes)
                    {
                        if (IsExperimental(type) && !DashEditorCore.Config.showExperimental)
                            continue;

                        if (IsHidden(type))
                            continue;

                        if (CheckMultiple(type))
                            continue;
                        
                        CategoryAttribute attribute = type.GetCustomAttribute<CategoryAttribute>();
                        NodeCategoryType category = attribute == null ? NodeCategoryType.OTHER : attribute.type;
                        string categoryString = category.ToString();
                        categoryString = categoryString.Substring(0, 1) + categoryString.Substring(1).ToLower();

                        string node = type.ToString().Substring(type.ToString().IndexOf(".") + 1);
                        node = node.Substring(0, node.Length-4);
                        menu.AddItem(new GUIContent("Create Node/"+categoryString+"/"+node), false, CreateNode, type);
                    }
                }
            }

            if (p_type == GraphContextMenuType.NODE)
            {
                if (DashEditorCore.selectedNodes.Count > 1)
                {
                    menu.AddItem(new GUIContent("Delete Nodes"), false, DeleteNode, null);
                    menu.AddItem(new GUIContent("Duplicate Nodes"), false, DuplicateNode, null);
                }
                else
                {
                    menu.AddItem(new GUIContent("Delete Node"), false, DeleteNode, (NodeBase) p_object);   
                    menu.AddItem(new GUIContent("Duplicate Node"), false, DuplicateNode, (NodeBase) p_object);
                    menu.AddItem(new GUIContent("Set as Preview"), false, SetAsPreview, (NodeBase) p_object);
                    menu.AddItem(new GUIContent("Instant Preview"), false, InstantPreview, (NodeBase) p_object);
                }
            }

            if (p_type == GraphContextMenuType.CONNECTION)
            {
                NodeConnection connection = (NodeConnection) p_object;
                if (connection.active)
                {
                    menu.AddItem(new GUIContent("Deactivate connection."), false, DeactivateConnection, connection);

                }
                else
                {
                    menu.AddItem(new GUIContent("Activate connection."), false, ActivateConnection, connection);
                }

                menu.AddItem(new GUIContent("Delete Connection"), false, DeleteConnection, (NodeConnection)p_object);
            }

            menu.ShowAsContext();
            
            p_event.Use();
        }

        static bool IsHidden(Type p_type)
        {
            CategoryAttribute attribute = p_type.GetCustomAttribute<CategoryAttribute>();

            if (attribute != null && attribute.type == NodeCategoryType.HIDDEN)
                return true;

            return false;
        }
        
        static bool IsExperimental(Type p_type)
        {
            return p_type.GetCustomAttribute<ExperimentalAttribute>() != null;
        }
        
        static bool CheckMultiple(Type p_type)
        {
            SettingsAttribute sa = (SettingsAttribute) p_type.GetCustomAttribute<SettingsAttribute>();
            if (sa != null && !sa.canHaveMultiple && Graph.HasNodeOfType(p_type))
                return true;

            return false;
        }
        
        static void CreateGraph()
        {
            DashEditorCore.EditGraph(GraphUtils.CreateGraphAsAssetFile());
        }

        static void LoadGraph()
        {
            DashGraph graph = GraphUtils.LoadGraph();
            if (graph != null)
            {
                DashEditorCore.EditGraph(graph);
            }
        }

        static void CreateNode(object p_nodeType)
        {
            Graph.CreateNodeInEditor((Type)p_nodeType, _lastMousePosition);
        }

        static void SetAsPreview(object p_node)
        {
            Graph.previewNode = (NodeBase)p_node;
        }
        
        static void InstantPreview(object p_node)
        {
            DashEditorCore.Previewer.StartPreview((NodeBase)p_node);
        }
        
        static void DeleteNode(object p_node)
        {
            if (p_node == null)
            {
                Undo.RecordObject(Graph, "DeleteNodes");

                while (DashEditorCore.selectedNodes.Count > 0)
                {
                    int index = DashEditorCore.selectedNodes[0];
                    Graph.RemoveNode(Graph.Nodes[DashEditorCore.selectedNodes[0]]);
                    DashEditorCore.selectedNodes.Remove(index);
                    DashEditorCore.ReindexSelected(index);
                }
            }
            else
            {
                Undo.RecordObject(Graph, "DeleteNode");

                int index = ((NodeBase) p_node).Index;
                Graph.RemoveNode((NodeBase) p_node);
                DashEditorCore.selectedNodes.Remove(index);
                DashEditorCore.ReindexSelected(index);
            }
            
            DashEditorCore.SetDirty();
        }
        
        static void DuplicateNode(object p_node)
        {
            if (p_node == null)
            {
                Undo.RecordObject(Graph, "DuplicateNodes");

                List<NodeBase> nodes = DashEditorCore.selectedNodes.Select(i => Graph.Nodes[i]).ToList();
                List<NodeBase> newNodes = Graph.DuplicateNodes(nodes);
                DashEditorCore.selectedNodes = newNodes.Select(n => n.Index).ToList();
            }
            else
            {
                Undo.RecordObject(Graph, "DuplicateNode");
                
                NodeBase node = Graph.DuplicateNode((NodeBase) p_node);
                DashEditorCore.selectedNodes = new List<int> { node.Index };
            }

            DashEditorCore.SetDirty();
        }

        static void DeleteConnection(object p_connection)
        {
            Undo.RecordObject(Graph, "Delete connection.");
            Graph.Disconnect((NodeConnection)p_connection);
        }
        
        static void DeactivateConnection(object p_connection)
        {
            ((NodeConnection) p_connection).active = false;
        }
        
        static void ActivateConnection(object p_connection)
        {
            ((NodeConnection) p_connection).active = true;
        }
    }
}