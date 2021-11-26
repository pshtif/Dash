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
                menu.AddItem(new GUIContent("Duplicate Nodes"), false, DuplicateNode, null);
                menu.AddItem(new GUIContent("Create Box"), false, CreateBox);
            }
            else
            {
                menu.AddItem(new GUIContent("Copy Node"), false, CopyNode, p_node);
                menu.AddItem(new GUIContent("Delete Node"), false, DeleteNode, p_node);   
                menu.AddItem(new GUIContent("Duplicate Node"), false, DuplicateNode, p_node);
                menu.AddSeparator("");
                if (p_node.HasComment())
                {
                    menu.AddItem(new GUIContent("Remove Comment"), false, p_node.RemoveComment);
                }
                else
                {
                    menu.AddItem(new GUIContent("Create Comment"), false, p_node.CreateComment);
                }

                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Set as Preview"), false, SetAsPreview, p_node);
                menu.AddItem(new GUIContent("Instant Preview"), false, InstantPreview, p_node);
                
                if (p_node is InputNode)
                {
                    InputNode node = p_node as InputNode;
                    menu.AddSeparator("");
                    if (!node.Controller.autoStart || node.Controller.autoStartInput != node.Model.inputName)
                    {
                        menu.AddItem(new GUIContent("Set as Start Input"), false, SetAsStartInput, p_node);
                    }
                    else
                    {
                        menu.AddItem(new GUIContent("Remove as Start Input"), false, RemoveAsStartInput, p_node);
                    }

                    if (!node.Controller.autoOnEnable || node.Controller.autoOnEnableInput != node.Model.inputName)
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
            node.Controller.autoStart = true;
            node.Controller.autoStartInput = node.Model.inputName;
        }
        
        static void RemoveAsStartInput(object p_node)
        {
            InputNode node = p_node as InputNode;
            node.Controller.autoStart = false;
            node.Controller.autoStartInput = "";
        }

        static void SetAsOnEnableInput(object p_node)
        {
            InputNode node = p_node as InputNode;
            node.Controller.autoOnEnable = true;
            node.Controller.autoOnEnableInput = node.Model.inputName;
        }
        
        static void RemoveAsOnEnableInput(object p_node)
        {
            InputNode node = p_node as InputNode;
            node.Controller.autoOnEnable = false;
            node.Controller.autoOnEnableInput = "";
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
            
            DashEditorCore.SetDirty();
        }
        
        static void DuplicateNode(object p_node)
        {
            if (p_node == null)
            {
                SelectionManager.DuplicateSelectedNodes(Graph);
            }
            else
            {
                SelectionManager.DuplicateNode((NodeBase)p_node, Graph);
            }
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