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
    public class GraphContextMenu
    {
        static private DashGraph Graph => DashEditorCore.Config.editingGraph;
        
        static private Vector2 _lastMousePosition;
        static public void Show()
        {
            _lastMousePosition = Event.current.mousePosition;
            
            GenericMenu menu = new GenericMenu();
            
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
            
            menu.ShowAsContext();
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
    }
}