/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEditor;
using UnityEngine;
using Object = System.Object;

namespace Dash
{
    public class NodeContextMenu
    {
        static private DashGraph Graph => DashEditorCore.Config.editingGraph;
        
        public static void Show(NodeBase p_node)
        {
            GenericMenu menu = new GenericMenu();
            
            if (DashEditorCore.selectedNodes.Count > 1)
            {
                menu.AddItem(new GUIContent("Delete Nodes"), false, DeleteNode, null);
                menu.AddItem(new GUIContent("Duplicate Nodes"), false, DuplicateNode, null);
                menu.AddItem(new GUIContent("Create Region"), false, CreateRegion);
            }
            else
            {
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
        
        static void DeleteNode(object p_node)
        {
            if (p_node == null)
            {
                Undo.RecordObject(Graph, "Delete Nodes");

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
                Undo.RecordObject(Graph, "Delete Node");

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

                DashEditorCore.DuplicateSelectedNodes();
            }
            else
            {
                Undo.RecordObject(Graph, "DuplicateNode");

                DashEditorCore.DuplicateNode((NodeBase)p_node);
            }
        }

        static void CreateRegion()
        {
            Undo.RecordObject(Graph, "Create Region");
            
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