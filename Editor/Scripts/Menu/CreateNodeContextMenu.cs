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
using TooltipAttribute = Dash.Attributes.TooltipAttribute;

namespace Dash.Editor
{ 
    public class CreateNodeContextMenu
    {
        
        static private Vector2 _lastMousePosition;
        static public void Show()
        {
            Get().ShowAsEditorMenu();
        }

        static public void ShowAsPopup()
        {
            _lastMousePosition = Event.current.mousePosition;

            GenericMenuPopup.Show(Get(), "Create Node", _lastMousePosition, 240, 300);
        }
        
        static public RuntimeGenericMenu Get()
        {
            RuntimeGenericMenu menu = new RuntimeGenericMenu();
            
            if (DashEditorCore.EditorConfig.editingGraph != null)
            {
                foreach (var graph in DashEditorCore.GraphAssets)
                {
                    if (graph == null || graph == DashEditorCore.EditorConfig.editingGraph)
                        continue;
                    
                    menu.AddItem(new GUIContent("Graphs/" + graph.name, ""), false, CreateGraphNode,
                        graph);
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

                    if (CheckMultiple(type))
                        continue;
                    
                    TooltipAttribute tooltipAttribute = type.GetCustomAttribute<TooltipAttribute>();
                    string tooltip = tooltipAttribute != null ? tooltipAttribute.help : "";
                    
                    NodeCategoryType category = NodeUtils.GetNodeCategory(type);
                    string categoryLabel = NodeUtils.GetCategoryLabel(type);

                    string node = type.ToString().Substring(type.ToString().LastIndexOf(".") + 1);
                    node = node.Substring(0, node.Length-4);
                    
                    if (category == NodeCategoryType.GRAPH)
                    {
                        menu.AddItem(new GUIContent(node, tooltip), false, CreateNode, type);
                    }
                    else
                    {
                        menu.AddItem(new GUIContent(categoryLabel + "/" + node, tooltip), false, CreateNode,
                            type);
                    }
                }

                if (SelectionManager.HasCopiedNodes())
                {
                    menu.AddItem(new GUIContent("Paste Nodes"), false, PasteNodes);
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
                    
                        menu.AddItem(new GUIContent("Create For Selected/"+node, tooltip), false, CreateAnimationNodesFromSelection, type);
                    }
                }
                
            }

            return menu;
        }

        static void CreateAnimationNodesFromSelection(object p_nodeType)
        {
            Transform[] selectedTransforms = SelectionUtils.GetTransformsFromSelection();
            float zoom = DashEditorCore.EditorConfig.zoom;
            Vector2 viewOffset = DashEditorCore.EditorConfig.editingGraph.viewOffset;
            Vector2 position = new Vector2(_lastMousePosition.x * zoom - viewOffset.x, _lastMousePosition.y * zoom - viewOffset.y);
            Vector2 offset = Vector2.zero;
            
            foreach (Transform transform in selectedTransforms)
            {
                NodeBase node = NodeUtils.CreateNode(DashEditorCore.EditorConfig.editingGraph, (Type)p_nodeType, position + offset);
                
                if (node != null)
                {
                    RetargetNodeModelBase model = node.GetModel() as RetargetNodeModelBase;
                    model.retarget = true;
                    //model.target.SetValue(transform.name);
                    
                    model.useReference = true;
                    IExposedPropertyTable propertyTable = DashEditorCore.EditorConfig.editingController;
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

        static void ShowAnimationNodeTypesMenu()
        {
            RuntimeGenericMenu menu = new RuntimeGenericMenu();
            
            if (DashEditorCore.EditorConfig.editingGraph != null)
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
                    
                    menu.AddItem(new GUIContent(node, tooltip), false, CreateAnimationNodesFromSelection, type);
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
        
        static bool CheckMultiple(Type p_type)
        {
            if (!NodeUtils.CanHaveMultipleInstances(p_type) && DashEditorCore.EditorConfig.editingGraph.HasNodeOfType(p_type))
                return true;

            return false;
        }

        static void PasteNodes()
        {
            SelectionManager.PasteNodes(_lastMousePosition, DashEditorCore.EditorConfig.editingGraph);
        }

        static void CreateNode(object p_nodeType)
        {
            var graph = DashEditorCore.EditorConfig.editingGraph;
            float zoom = DashEditorCore.EditorConfig.zoom;
            Vector2 offset = graph.viewOffset;
            Vector2 position = new Vector2(_lastMousePosition.x * zoom - offset.x, _lastMousePosition.y * zoom - offset.y);
            
            var node = NodeUtils.CreateNode(graph, (Type)p_nodeType, position);

            if (SelectionManager.connectingNode != null)
            {
                SelectionManager.EndConnectionDrag(node, 0);
            }
        }
        
        static void CreateGraphNode(object p_graph)
        {
            float zoom = DashEditorCore.EditorConfig.zoom;
            Vector2 offset = DashEditorCore.EditorConfig.editingGraph.viewOffset;
            Vector2 position = new Vector2(_lastMousePosition.x * zoom - offset.x, _lastMousePosition.y * zoom - offset.y);
            
            SubGraphNode node = (SubGraphNode)NodeBase.Create(typeof(SubGraphNode), DashEditorCore.EditorConfig.editingGraph);

            if (node != null)
            {
                node.rect = new Rect(position.x, position.y, 0, 0);
                DashEditorCore.EditorConfig.editingGraph.Nodes.Add(node);
            }

            node.Model.useAsset = true;
            node.Model.graphAsset = (DashGraph)p_graph;

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