/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEditor;
using UnityEngine;

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
            }
            else
            {
                menu.AddItem(new GUIContent("Delete Node"), false, DeleteNode, p_node);   
                menu.AddItem(new GUIContent("Duplicate Node"), false, DuplicateNode, p_node);
                menu.AddItem(new GUIContent("Set as Preview"), false, SetAsPreview, p_node);
                menu.AddItem(new GUIContent("Instant Preview"), false, InstantPreview, p_node);
            }

            menu.ShowAsContext();
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

                DashEditorCore.DuplicateSelectedNodes();
            }
            else
            {
                Undo.RecordObject(Graph, "DuplicateNode");

                DashEditorCore.DuplicateNode((NodeBase)p_node);
            }
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