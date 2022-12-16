/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEditor;
using UnityEngine;
using Object = System.Object;

namespace Dash.Editor
{
    public class NodeContextMenu
    {
        static private DashGraph Graph => DashEditorCore.EditorConfig.editingGraph;
        
        public static void Show(NodeBase p_node)
        {
            RuntimeGenericMenu menu = new RuntimeGenericMenu();
            
            if (SelectionManager.SelectedCount > 1)
            {
                menu.AddItem(new GUIContent("Copy Nodes"), false, CopyNode, null);
                menu.AddItem(new GUIContent("Delete Nodes"), false, DeleteNode, null);
                menu.AddItem(new GUIContent("Duplicate Nodes"), false, DuplicateNodes);
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Create Box"), false, CreateBox);
                menu.AddItem(new GUIContent("Create SubGraph"), false, CreateSubGraph, null);
            }
            else
            {
                menu.AddItem(new GUIContent("Copy Node"), false, CopyNode, p_node);
                menu.AddItem(new GUIContent("Delete Node"), false, DeleteNode, p_node);   
                menu.AddItem(new GUIContent("Duplicate Node"), false, DuplicateNode, p_node);
                menu.AddItem(new GUIContent("Disconnect Node"), false, DisconnectNode, p_node);
                menu.AddItem(new GUIContent("Arrange Nodes"), false, ArrangeNodes, p_node);
                menu.AddItem(new GUIContent("Select Connected"), false, SelectConnectedNodes, p_node);
                
                menu.AddSeparator("");
                if (p_node.HasComment())
                {
                    menu.AddItem(new GUIContent("Remove Comment"), false, p_node.RemoveComment);
                }
                else
                {
                    menu.AddItem(new GUIContent("Create Comment"), false, p_node.CreateComment);
                }
                
                if (p_node is SubGraphNode)
                {
                    menu.AddSeparator("");
                    menu.AddItem(new GUIContent("Unpack SubGraph"), false, UnpackSubGraph, p_node);
                }

                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Set as Preview"), false, SetAsPreview, p_node);
                menu.AddItem(new GUIContent("Instant Preview"), false, InstantPreview, p_node);

                var controller = DashEditorCore.EditorConfig.editingController;
                if (p_node is InputNode && controller != null)
                {
                    
                    InputNode node = p_node as InputNode;
                    menu.AddSeparator("");
                    if (!controller.autoStart || controller.autoStartInput != node.Model.inputName)
                    {
                        menu.AddItem(new GUIContent("Set as Start Input"), false, SetAsStartInput, p_node);
                    }
                    else
                    {
                        menu.AddItem(new GUIContent("Remove as Start Input"), false, RemoveAsStartInput, p_node);
                    }

                    if (!controller.autoOnEnable || controller.autoOnEnableInput != node.Model.inputName)
                    {
                        menu.AddItem(new GUIContent("Set as OnEnable Input"), false, SetAsOnEnableInput, p_node);    
                    }
                    else
                    {
                        menu.AddItem(new GUIContent("Remove as OnEnable Input"), false, RemoveAsOnEnableInput, p_node);
                    }
                    
                }
            }
            
            menu.AddSeparator("");

            if (!SelectionManager.selectedNodes.Contains(Graph.Nodes.IndexOf(p_node)))
            {
                menu.AddItem(new GUIContent("Connect Selection as Output"), false, ConnectSelectionAsOutput, p_node);
                menu.AddItem(new GUIContent("Connect Selection as Input"), false, ConnectSelectionAsInput, p_node);
            }

            ((INodeAccess)p_node).GetCustomContextMenu(ref menu);

            //menu.ShowAsEditorMenu();
            GenericMenuPopup.Show(menu, "",  Event.current.mousePosition, 240, 300, false, false);
        }

        static void ConnectSelectionAsInput(object p_node)
        {
            foreach (int nodeIndex in SelectionManager.selectedNodes)
            {
                NodeBase node = Graph.Nodes[nodeIndex];
                Graph.Connect((NodeBase)p_node, 0, node, 0);
            }
            
            DashEditorCore.SetDirty();
        }
        
        static void ConnectSelectionAsOutput(object p_node)
        {
            foreach (int nodeIndex in SelectionManager.selectedNodes)
            {
                NodeBase node = Graph.Nodes[nodeIndex];
                Graph.Connect(node, 0, (NodeBase)p_node, 0);
            }
            
            DashEditorCore.SetDirty();
        }

        static void SetAsStartInput(object p_node)
        {
            InputNode node = p_node as InputNode;

            DashEditorCore.EditorConfig.editingController.autoStart = true;
            DashEditorCore.EditorConfig.editingController.autoStartInput = node.Model.inputName;
        }
        
        static void RemoveAsStartInput(object p_node)
        {
            InputNode node = p_node as InputNode;
            DashEditorCore.EditorConfig.editingController.autoStart = false;
            DashEditorCore.EditorConfig.editingController.autoStartInput = "";
        }

        static void SetAsOnEnableInput(object p_node)
        {
            InputNode node = p_node as InputNode;
            DashEditorCore.EditorConfig.editingController.autoOnEnable = true;
            DashEditorCore.EditorConfig.editingController.autoOnEnableInput = node.Model.inputName;
        }
        
        static void RemoveAsOnEnableInput(object p_node)
        {
            InputNode node = p_node as InputNode;
            DashEditorCore.EditorConfig.editingController.autoOnEnable = false;
            DashEditorCore.EditorConfig.editingController.autoOnEnableInput = "";
        }

        static void CopyNode(object p_node)
        {
            if (p_node == null)
            {
                SelectionManager.CopySelectedNodes(Graph);
            }
            else
            {
                SelectionManager.CopyNode((NodeBase)p_node, Graph);
            }
        }

        static void DeleteNode(object p_node)
        {
            if (p_node == null)
            {
                SelectionManager.DeleteSelectedNodes(Graph);
            }
            else
            {
                SelectionManager.DeleteNode((NodeBase)p_node, Graph);
            }
        }

        static void DisconnectNode(object p_node)
        {
            if (p_node != null)
            {
                Graph.DisconnectNode((NodeBase) p_node);
            }
        }
        
        static void CreateSubGraph(object p_node)
        {
            if (p_node == null)
            {
                SelectionManager.CreateSubGraphFromSelectedNodes(Graph);
            }
        }
        
        static void UnpackSubGraph(object p_node)
        {
            if (p_node != null)
            {
                SelectionManager.UnpackSelectedSubGraphNode(Graph, (SubGraphNode)p_node);
            }
        }
        
        static void DuplicateNode(object p_node)
        {
            SelectionManager.DuplicateNode((NodeBase)p_node, Graph);
        }
        
        static void DuplicateNodes()
        {
            SelectionManager.DuplicateSelectedNodes(Graph);
        }

        static void ArrangeNodes(object p_node)
        {
            SelectionManager.ArrangeNodes(Graph, (NodeBase)p_node);
        }
        
        static void SelectConnectedNodes(object p_node)
        {
            SelectionManager.SelectConnectedNodes(Graph, (NodeBase)p_node);
        }

        static void CreateBox()
        {
            Undo.RegisterCompleteObjectUndo(Graph, "Create Box");
            
            SelectionManager.CreateBoxAroundSelectedNodes(Graph);
        }
        
        static void SetAsPreview(object p_node)
        {
            Graph.previewNode = (NodeBase)p_node;
            DashEditorCore.SetDirty();
        }
        
        static void InstantPreview(object p_node)
        {
            DashEditorCore.Previewer.StartPreview((NodeBase)p_node);
        }
    }
}