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
        static private DashGraph Graph => DashEditorCore.Config.editingGraph;
        
        public static void Show(NodeBase p_node)
        {
            GenericMenu menu = new GenericMenu();
            
            if (DashEditorCore.selectedNodes.Count > 1)
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
            }

            menu.ShowAsContext();
        }

        static void CopyNode(object p_node)
        {
            if (p_node == null)
            {
                DashEditorCore.CopySelectedNodes();
            }
            else
            {
                DashEditorCore.CopyNode((NodeBase)p_node);
            }
        }

        static void DeleteNode(object p_node)
        {
            if (p_node == null)
            {
                DashEditorCore.DeleteSelectedNodes();
            }
            else
            {
                DashEditorCore.DeleteNode((NodeBase)p_node);
            }
            
            DashEditorCore.SetDirty();
        }
        
        static void DuplicateNode(object p_node)
        {
            if (p_node == null)
            {
                DashEditorCore.DuplicateSelectedNodes();
            }
            else
            {
                DashEditorCore.DuplicateNode((NodeBase)p_node);
            }
        }

        static void CreateBox()
        {
            Undo.RegisterCompleteObjectUndo(Graph, "Create Box");
            
            DashEditorCore.CreateBoxAroundSelectedNodes();
        }
        
        static void SetAsPreview(object p_node)
        {
            Graph.previewNode = (NodeBase)p_node;
        }
        
        static void InstantPreview(object p_node)
        {
            DashEditorCore.Previewer.StartPreview((NodeBase)p_node);
        }
    }
}