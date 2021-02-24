/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Dash.Attributes;
using Dash.Popup;
using OdinSerializer.Utilities;
using UnityEditor;
using UnityEngine;
using Object = System.Object;

namespace Dash
{ 
    public class GraphContextMenu
    {
        static private DashGraph Graph => DashEditorCore.Config.editingGraph;
        
        static private Vector2 _lastMousePosition;
        static public void Show()
        {
            _lastMousePosition = Event.current.mousePosition;

            Get().ShowAsContext();
        }

        static public void ShowAsPopup()
        {
            _lastMousePosition = Event.current.mousePosition;

            TypeMenuPopup.Show(Get(), _lastMousePosition, "Create Node");
        }
        
        static public GenericMenu Get()
        {
            GenericMenu menu = new GenericMenu();
            
            if (DashEditorCore.Config.editingGraph != null)
            {
                Type[] nodeTypes = ReflectionUtils.GetAllTypes(typeof(NodeBase)).ToArray();
                Array.Sort(nodeTypes, CategorySort);
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
                    if (category == NodeCategoryType.GRAPH)
                    {
                        menu.AddItem(new GUIContent(node), false, CreateNode, type);
                    }
                    else
                    {
                        menu.AddItem(new GUIContent(categoryString + "/" + node), false, CreateNode,
                            type);
                    }
                }
            }

            return menu;
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
            if (!NodeUtils.CanHaveMultipleInstances(p_type) && Graph.HasNodeOfType(p_type))
                return true;

            return false;
        }

        static void CreateNode(object p_nodeType)
        {
            Graph.CreateNode((Type)p_nodeType, _lastMousePosition);
        }
        
        static public int CategorySort(Type p_type1, Type p_type2)
        {
            CategoryAttribute attribute1 = p_type1.GetCustomAttribute<CategoryAttribute>();
            CategoryAttribute attribute2 = p_type2.GetCustomAttribute<CategoryAttribute>();
            string categoryString1 = attribute1 == null ? "Other" : NodeUtils.CategoryToString(attribute1.type);
            string categoryString2 = attribute2 == null ? "Other" : NodeUtils.CategoryToString(attribute2.type);
            string nodeString1 = p_type1.ToString().Substring(p_type1.ToString().IndexOf(".") + 1);
            string nodeString2 = p_type2.ToString().Substring(p_type2.ToString().IndexOf(".") + 1);

            if (categoryString1 == categoryString2)
                return nodeString1.CompareTo(nodeString2);

            if (categoryString1 == "Graph")
                return 1;
            
            if (categoryString2 == "Graph")
                return -1;

            return categoryString1.CompareTo(categoryString2);
        }
        
    }
}