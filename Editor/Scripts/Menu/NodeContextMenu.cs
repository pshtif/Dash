/*
 *	Created by:  Peter @sHTiF Stefcek
 */
#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Dash.Editor
{
    public class NodeContextMenu
    {
        public static void Show(DashGraph p_graph, NodeBase p_node)
        {
            RuntimeGenericMenu menu = new RuntimeGenericMenu();

            if (SelectionManager.SelectedCount > 1)
            {
                menu.AddItem(new GUIContent("Copy Nodes"), false, () => CopyNode(p_graph, null));
                menu.AddItem(new GUIContent("Delete Nodes"), false, () => DeleteNode(p_graph, null));
                menu.AddItem(new GUIContent("Duplicate Nodes"), false, () => DuplicateNodes(p_graph));
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Create Box"), false, () => CreateBox(p_graph));
                menu.AddItem(new GUIContent("Create SubGraph"), false, () => CreateSubGraph(p_graph));
            }
            else
            {
                menu.AddItem(new GUIContent("Copy Node"), false, () => CopyNode(p_graph, p_node));
                menu.AddItem(new GUIContent("Delete Node"), false, () => DeleteNode(p_graph, p_node));   
                menu.AddItem(new GUIContent("Duplicate Node"), false, () => DuplicateNode(p_graph, p_node));
                menu.AddItem(new GUIContent("Disconnect Node"), false, () => DisconnectNode(p_graph, p_node));
                menu.AddItem(new GUIContent("Arrange Nodes"), false, () => ArrangeNodes(p_graph, p_node));
                menu.AddItem(new GUIContent("Select Connected"), false, () => SelectConnectedNodes(p_graph, p_node));
                
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
                    menu.AddItem(new GUIContent("Unpack SubGraph"), false,
                        () => UnpackSubGraph(p_graph, (SubGraphNode)p_node));
                }
            
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Set as Preview"), false, () => SetAsPreview(p_graph, p_node));
                menu.AddItem(new GUIContent("Instant Preview"), false, () => InstantPreview(p_node));
            
                var controller = DashEditorCore.EditorConfig.editingController;
                if (p_node is InputNode && controller != null)
                {
                    
                    InputNode node = p_node as InputNode;
                    menu.AddSeparator("");
                    if (!controller.bindStart || controller.bindStartInput != node.Model.inputName)
                    {
                        menu.AddItem(new GUIContent("Set as Start Input"), false,
                            () => SetAsStartInput((InputNode)p_node));
                    }
                    else
                    {
                        menu.AddItem(new GUIContent("Remove as Start Input"), false,
                            () => RemoveAsStartInput((InputNode)p_node));
                    }
            
                    if (!controller.bindOnEnable || controller.bindOnEnableInput != node.Model.inputName)
                    {
                        menu.AddItem(new GUIContent("Set as OnEnable Input"), false,
                            () => SetAsOnEnableInput((InputNode)p_node));
                    }
                    else
                    {
                        menu.AddItem(new GUIContent("Remove as OnEnable Input"), false,
                            () => RemoveAsOnEnableInput((InputNode)p_node));
                    }
                    
                }
            }
            
            menu.AddSeparator("");
            
            if (!SelectionManager.selectedNodes.Contains(p_graph.Nodes.IndexOf(p_node)))
            {
                menu.AddItem(new GUIContent("Connect Selection as Output"), false,
                    () => ConnectSelectionAsOutput(p_graph, p_node));
                menu.AddItem(new GUIContent("Connect Selection as Input"), false,
                    () => ConnectSelectionAsInput(p_graph, p_node));
            }
            
            p_node.GetCustomContextMenu(ref menu);
            
            GenericMenuPopup.Show(menu, "",  Event.current.mousePosition, 240, 300, false, false);
        }

        static void ConnectSelectionAsInput(DashGraph p_graph, NodeBase p_node)
        {
            foreach (int nodeIndex in SelectionManager.selectedNodes)
            {
                NodeBase node = p_graph.Nodes[nodeIndex];
                p_graph.Connect(p_node, 0, node, 0);
            }
            
            DashEditorCore.SetDirty();
        }
        
        static void ConnectSelectionAsOutput(DashGraph p_graph, NodeBase p_node)
        {
            foreach (int nodeIndex in SelectionManager.selectedNodes)
            {
                NodeBase node = p_graph.Nodes[nodeIndex];
                p_graph.Connect(node, 0, p_node, 0);
            }
            
            DashEditorCore.SetDirty();
        }

        static void SetAsStartInput(InputNode p_node)
        {
            DashEditorCore.EditorConfig.editingController.bindStart = true;
            DashEditorCore.EditorConfig.editingController.bindStartInput = p_node.Model.inputName;
        }
        
        static void RemoveAsStartInput(InputNode p_node)
        {
            DashEditorCore.EditorConfig.editingController.bindStart = false;
            DashEditorCore.EditorConfig.editingController.bindStartInput = "";
        }

        static void SetAsOnEnableInput(InputNode p_node)
        {
            DashEditorCore.EditorConfig.editingController.bindOnEnable = true;
            DashEditorCore.EditorConfig.editingController.bindOnEnableInput = p_node.Model.inputName;
        }
        
        static void RemoveAsOnEnableInput(InputNode p_node)
        {
            DashEditorCore.EditorConfig.editingController.bindOnEnable = false;
            DashEditorCore.EditorConfig.editingController.bindOnEnableInput = "";
        }

        static void CopyNode(DashGraph p_graph, NodeBase p_node)
        {
            if (p_node == null)
            {
                SelectionManager.CopySelectedNodes(p_graph);
            }
            else
            {
                SelectionManager.CopyNode(p_graph, p_node);
            }
        }

        static void DeleteNode(DashGraph p_graph, NodeBase p_node)
        {
            if (p_node == null)
            {
                SelectionManager.DeleteSelectedNodes(p_graph);
            }
            else
            {
                SelectionManager.DeleteNode(p_graph, p_node);
            }
        }

        static void DisconnectNode(DashGraph p_graph, NodeBase p_node)
        {
            if (p_node != null)
            {
                p_graph.DisconnectNode(p_node);
            }
        }
        
        static void CreateSubGraph(DashGraph p_graph)
        {
            SelectionManager.CreateSubGraphFromSelectedNodes(p_graph);
        }
        
        static void UnpackSubGraph(DashGraph p_graph, SubGraphNode p_node)
        {
            if (p_node != null)
            {
                SelectionManager.UnpackSelectedSubGraphNode(p_graph, p_node);
            }
        }
        
        static void DuplicateNode(DashGraph p_graph, NodeBase p_node)
        {
            SelectionManager.DuplicateNode(p_node, p_graph);
        }
        
        static void DuplicateNodes(DashGraph p_graph)
        {
            SelectionManager.DuplicateSelectedNodes(p_graph);
        }

        static void ArrangeNodes(DashGraph p_graph, NodeBase p_node)
        {
            SelectionManager.ArrangeNodes(p_graph, p_node);
        }
        
        static void SelectConnectedNodes(DashGraph p_graph, NodeBase p_node)
        {
            SelectionManager.SelectConnectedNodes(p_graph, (NodeBase)p_node);
        }

        static void CreateBox(DashGraph p_graph)
        {
            Undo.RegisterCompleteObjectUndo(p_graph, "Create Box");
            
            SelectionManager.CreateBoxAroundSelectedNodes(p_graph);
        }
        
        static void SetAsPreview(DashGraph p_graph, NodeBase p_node)
        {
            p_graph.previewNode = p_node;
            DashEditorCore.SetDirty();
        }
        
        static void InstantPreview(NodeBase p_node)
        {
            DashEditorCore.Previewer.StartPreview(p_node);
        }
    }
}
#endif