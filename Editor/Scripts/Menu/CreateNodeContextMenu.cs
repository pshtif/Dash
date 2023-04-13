/*
 *	Created by:  Peter @sHTiF Stefcek
 */
#if UNITY_EDITOR

using System;
using Dash.Attributes;
using OdinSerializer.Utilities;
using UnityEditor;
using UnityEngine;
using TooltipAttribute = Dash.Attributes.TooltipAttribute;

namespace Dash.Editor
{ 
    public class CreateNodeContextMenu
    {
        
        static private Vector2 _lastMousePosition;
        static public void Show(DashGraph p_graph)
        {
            Get(p_graph).ShowAsEditorMenu();
        }

        static public void ShowAsPopup(DashGraph p_graph)
        {
            _lastMousePosition = Event.current.mousePosition;

            GenericMenuPopup.Show(Get(p_graph), "Create Node", _lastMousePosition, 240, 300);
        }
        
        static public RuntimeGenericMenu Get(DashGraph p_graph)
        {
            RuntimeGenericMenu menu = new RuntimeGenericMenu();
            
            if (p_graph != null)
            {
                foreach (var subGraph in DashEditorCore.GraphAssets)
                {
                    if (subGraph == null || subGraph == p_graph)
                        continue;
                
                    menu.AddItem(new GUIContent("Graphs/" + subGraph.name, ""), false,
                        () => CreateGraphNode(p_graph, subGraph));
                }
                
                Type[] nodeTypes = ReflectionUtils.GetAllTypes(typeof(NodeBase)).ToArray();
                Array.Sort(nodeTypes, CategorySort);
                foreach (Type type in nodeTypes)
                {
                    if (IsObsolete(type) && !DashEditorCore.EditorConfig.showObsolete)
                        continue;
                    
                    if (IsExperimental(type) && !DashEditorCore.EditorConfig.showExperimental)
                        continue;

                    if (IsHidden(type))
                        continue;

                    if (CheckMultiple(p_graph, type))
                        continue;
                    
                    TooltipAttribute tooltipAttribute = type.GetCustomAttribute<TooltipAttribute>();
                    string tooltip = tooltipAttribute != null ? tooltipAttribute.help : "";
                    
                    NodeCategoryType category = NodeUtils.GetNodeCategory(type);
                    string categoryLabel = NodeUtils.GetCategoryLabel(type);

                    string node = type.ToString().Substring(type.ToString().LastIndexOf(".") + 1);
                    node = node.Substring(0, node.Length-4);
                    
                    if (category == NodeCategoryType.GRAPH)
                    {
                        menu.AddItem(new GUIContent(node, tooltip), false, () => CreateNode(p_graph, type));
                    }
                    else
                    {
                        menu.AddItem(new GUIContent(categoryLabel + "/" + node, tooltip), false,
                            () => CreateNode(p_graph, type));
                    }
                }

                if (SelectionManager.HasCopiedNodes())
                {
                    menu.AddItem(new GUIContent("Paste Nodes"), false, () => PasteNodes(p_graph));
                }
                
                menu.AddSeparator("");
                Transform[] selectedTransforms = SelectionUtils.GetTransformsFromSelection();
                if (selectedTransforms.Length > 0)
                {
                    foreach (Type type in nodeTypes)
                    {
                        CategoryAttribute attribute = type.GetCustomAttribute<CategoryAttribute>();
                        if (attribute == null || attribute.type != NodeCategoryType.ANIMATION)
                            continue;
                    
                        TooltipAttribute tooltipAttribute = type.GetCustomAttribute<TooltipAttribute>();
                        string tooltip = tooltipAttribute != null ? tooltipAttribute.help : "";
                    
                        string node = type.ToString().Substring(type.ToString().IndexOf(".") + 1);
                        node = node.Substring(0, node.Length-4);
                    
                        menu.AddItem(new GUIContent("Create For Selected/"+node, tooltip), false, () => CreateAnimationNodesFromSelection(p_graph, type));
                    }
                }
                
            }

            return menu;
        }

        static void CreateAnimationNodesFromSelection(DashGraph p_graph, object p_nodeType)
        {
            Transform[] selectedTransforms = SelectionUtils.GetTransformsFromSelection();
            Vector2 position = new Vector2(_lastMousePosition.x * p_graph.zoom - p_graph.viewOffset.x, _lastMousePosition.y * p_graph.zoom - p_graph.viewOffset.y);
            Vector2 offset = Vector2.zero;
            
            foreach (Transform transform in selectedTransforms)
            {
                NodeBase node = NodeUtils.CreateNode(p_graph, (Type)p_nodeType, position + offset);
                
                if (node != null)
                {
                    RetargetNodeModelBase model = node.GetModel() as RetargetNodeModelBase;
                    model.retarget = true;
                    //model.target.SetValue(transform.name);
                    
                    model.useReference = true;
                    IExposedPropertyTable propertyTable = p_graph.Controller;
                    bool isDefault = PropertyName.IsNullOrEmpty(model.targetReference.exposedName);

                    if (isDefault)
                    {
                        PropertyName newExposedName = new PropertyName(GUID.Generate().ToString());
                        model.targetReference.exposedName = newExposedName;
                        
                        propertyTable.SetReferenceValue(newExposedName, transform);
                        //p_fieldInfo.SetValue(p_object, exposedReference);
                    }
                    else
                    {
                        propertyTable.SetReferenceValue(model.targetReference.exposedName, transform);
                    }
                    
                    // If its bindable bind all values to current transform
                    if (node is IAnimationNodeBindable)
                    {
                        ((IAnimationNodeBindable)node).GetTargetFrom(transform);
                        ((IAnimationNodeBindable)node).GetTargetTo(transform);
                    }

                    offset.y += node.Size.y + 24;
                }
            }
        }

        static void ShowAnimationNodeTypesMenu(DashGraph p_graph)
        {
            RuntimeGenericMenu menu = new RuntimeGenericMenu();
            
            if (p_graph != null)
            {
                Type[] nodeTypes = ReflectionUtils.GetAllTypes(typeof(NodeBase)).ToArray();
                foreach (Type type in nodeTypes)
                {
                    CategoryAttribute attribute = type.GetCustomAttribute<CategoryAttribute>();
                    if (attribute == null || attribute.type != NodeCategoryType.ANIMATION)
                        continue;
                    
                    TooltipAttribute tooltipAttribute = type.GetCustomAttribute<TooltipAttribute>();
                    string tooltip = tooltipAttribute != null ? tooltipAttribute.help : "";
                    
                    string node = type.ToString().Substring(type.ToString().IndexOf(".") + 1);
                    node = node.Substring(0, node.Length-4);
                    
                    menu.AddItem(new GUIContent(node, tooltip), false, () => CreateAnimationNodesFromSelection(p_graph, type));
                }
            }
            
            GenericMenuPopup.Show(menu, "", Event.current.mousePosition, 240, 300, false);
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
        
        static bool IsObsolete(Type p_type)
        {
            return p_type.GetCustomAttribute<ObsoleteAttribute>() != null;
        }
        
        static bool CheckMultiple(DashGraph p_graph, Type p_type)
        {
            if (!NodeUtils.CanHaveMultipleInstances(p_type) && p_graph.HasNodeOfType(p_type))
                return true;

            return false;
        }

        static void PasteNodes(DashGraph p_graph)
        {
            SelectionManager.PasteNodes(p_graph, _lastMousePosition);
        }

        static void CreateNode(DashGraph p_graph, Type p_nodeType)
        {
            var graph = DashEditorCore.EditorConfig.editingGraph;
            Vector2 offset = graph.viewOffset;
            Vector2 position = new Vector2(_lastMousePosition.x * p_graph.zoom - offset.x, _lastMousePosition.y * p_graph.zoom - offset.y);
            
            var node = NodeUtils.CreateNode(graph, p_nodeType, position);

            if (SelectionManager.connectingNode != null)
            {
                SelectionManager.EndConnectionDrag(node, 0);
            }
        }
        
        static void CreateGraphNode(DashGraph p_graph, DashGraph p_subGraph)
        {
            Vector2 position = new Vector2(_lastMousePosition.x * p_graph.zoom - p_graph.viewOffset.x, _lastMousePosition.y * p_graph.zoom - p_graph.viewOffset.y);
            
            SubGraphNode node = (SubGraphNode)NodeBase.Create(typeof(SubGraphNode), p_graph);

            if (node != null)
            {
                node.rect = new Rect(position.x, position.y, 0, 0);
                DashEditorCore.EditorConfig.editingGraph.Nodes.Add(node);
            }

            node.Model.useAsset = true;
            node.Model.graphAsset = (DashGraph)p_subGraph;

            DashEditorCore.SetDirty();
            
            if (SelectionManager.connectingNode != null)
            {
                SelectionManager.EndConnectionDrag(node, 0);
            }
        }
        
        static public int CategorySort(Type p_type1, Type p_type2)
        {
            string categoryString1 = NodeUtils.GetCategoryLabel(p_type1);
            string categoryString2 = NodeUtils.GetCategoryLabel(p_type2);

            if (categoryString1 == categoryString2)
            {
                return NodeUtils.GetNodeLabel(p_type1).CompareTo(NodeUtils.GetNodeLabel(p_type2));
            }
            
            if (categoryString1 == "Graph")
                return 1;
            
            if (categoryString2 == "Graph")
                return -1;

            return CompareCategoryLabel(categoryString1, categoryString2);
        }

        static private int CompareCategoryLabel(string p_categoryLabel1, string p_categoryLabel2)
        {
            int index1 = p_categoryLabel1.IndexOf("/");
            int index2 = p_categoryLabel2.IndexOf("/");
            
            if (index1 == -1 && index2 == -1)
            {
                return p_categoryLabel1.CompareTo(p_categoryLabel2);
            }

            if (index1 == -1)
                return 1;

            if (index2 == -1)
                return -1;
            
            return CompareCategoryLabel(p_categoryLabel1.Substring(index1 + 1),
                p_categoryLabel2.Substring(index2 + 1));
        }
        
    }
}
#endif