/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using OdinSerializer.Utilities;
using UnityEditor;
using UnityEngine;

namespace Dash
{
    public class MenuItemNode
    {
        public GUIContent content;
        public GenericMenu.MenuFunction func;
        public GenericMenu.MenuFunction2 func2;
        public object userData;
        // Not implemented yet
        public bool separator;
        // Not implemented yet
        public bool on;

        public string name { get; }
        public MenuItemNode parent { get; }
        
        public Dictionary<string, MenuItemNode> Nodes { get; private set; }
        
        public MenuItemNode(string p_name = "", MenuItemNode p_parent = null)
        {
            name = p_name;
            parent = p_parent;
            Nodes = new Dictionary<string, MenuItemNode>();
        }

        public MenuItemNode GetOrCreateNode(string p_name)
        {
            if (!Nodes.ContainsKey(p_name))
            {
                Nodes.Add(p_name, new MenuItemNode(p_name, this));
            }

            return Nodes[p_name];
        }

        public List<MenuItemNode> Search(string p_search)
        {
            p_search = p_search.ToLower();
            List<MenuItemNode> result = new List<MenuItemNode>();
            
            foreach (var node in Nodes.Values)
            {
                if (node.Nodes.Count == 0 && node.name.ToLower().Contains(p_search))
                {
                    result.Add(node);
                }
                
                result.AddRange(node.Search(p_search));
            }

            return result;
        }

        public string GetPath()
        {
            return parent == null ? "" : parent.GetPath() + "/" + name;
        }

        public void Execute()
        {
            if (func != null)
            {
                func?.Invoke();
            }
            else
            {
                func2?.Invoke(userData);
            }
        }
    }
    
    public class TypeMenuPopup : PopupWindowContent
    {
        public static TypeMenuPopup Show(GenericMenu p_menu, Vector2 p_position, string p_title) {
            var popup = new TypeMenuPopup(p_menu, p_title);
            PopupWindow.Show(new Rect(p_position.x, p_position.y, 0, 0), popup);
            return popup;
        }
        
        public GUIStyle ButtonStyle 
        {
            get
            {
                GUIStyle style = new GUIStyle(GUI.skin.button);
                style.alignment = TextAnchor.MiddleLeft;
                style.hover.background = Texture2D.grayTexture;
                style.normal.textColor = Color.black;
                return style;
            }
        }

        public GUIStyle PlusStyle
        {
            get {
                GUIStyle style = new GUIStyle();
                style.fontStyle = FontStyle.Bold;
                style.normal.textColor = Color.white;
                style.fontSize = 16;
                return style;
            }
        }
        
        private string _title;

        private Vector2 _scrollPosition;
        private MenuItemNode _rootNode;
        private MenuItemNode _currentNode;
        private MenuItemNode _hoverNode;
        private string _search;

        public TypeMenuPopup(GenericMenu p_menu, string p_title)
        {
            _title = p_title;
            _currentNode = _rootNode = GenerateMenuItemNodeTree(p_menu);
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(300, 300);
        }

        public override void OnGUI(Rect p_rect)
        {
            GUIStyle style = new GUIStyle();
            style.normal.background = Texture2D.whiteTexture;
            GUI.color = new Color(0.1f, 0.1f, 0.1f, 1);
            GUI.Box(p_rect, string.Empty, style);
            GUI.color = Color.white;

            DrawTitle(p_rect);
            DrawSearch(p_rect);
            DrawMenuItems(new Rect(p_rect.x, p_rect.y+46, p_rect.width, p_rect.height-106));
            DrawTooltip(new Rect(p_rect.x+5, p_rect.y + p_rect.height - 58, p_rect.width-10, 56));
            
            EditorGUI.FocusTextInControl("Search");
        }

        private void DrawTitle(Rect p_rect)
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.fontStyle = FontStyle.Bold;
            style.fontSize = 16;
            style.alignment = TextAnchor.LowerCenter;
            GUI.Label(new Rect(p_rect.x, p_rect.y, p_rect.width,24), _title, style);
        }

        private void DrawSearch(Rect p_rect)
        {
            GUI.SetNextControlName("Search");
            _search = GUI.TextArea(new Rect(p_rect.x, p_rect.y + 24, p_rect.width, 20), _search);
        }

        private void DrawTooltip(Rect p_rect)
        {
            if (_hoverNode == null || _hoverNode.content == null || _hoverNode.content.tooltip.IsNullOrWhitespace())
                return;

            GUIStyle style = new GUIStyle();
            style.fontSize = 9;
            style.wordWrap = true;
            style.normal.textColor = Color.white;
            GUI.Label(p_rect, _hoverNode.content.tooltip, style);
        }

        private void DrawMenuItems(Rect p_rect)
        {
            GUILayout.BeginArea(p_rect);
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, false, false);
            GUILayout.BeginVertical();

            if (_search.IsNullOrWhitespace() || _search.Length<2)
            {
                DrawNodeTree();
            }
            else
            {
                DrawNodeSearch();
            }

            GUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();
        }
        
        private void DrawNodeSearch()
        {
            List<MenuItemNode> search = _rootNode.Search(_search);

            string lastPath = "";
            foreach (var node in search)
            {
                string nodePath = node.parent.GetPath();
                if (nodePath != lastPath)
                {
                    GUILayout.Label(nodePath);
                    lastPath = nodePath;
                }
                
                if (GUILayout.Button(node.name, ButtonStyle))
                {
                    {
                        node.Execute();
                        base.editorWindow.Close();
                    }
                    break;
                }
                
                var buttonRect = GUILayoutUtility.GetLastRect();
                if (buttonRect.Contains(Event.current.mousePosition))
                {
                    _hoverNode = node;
                }
            }

            if (search.Count == 0)
            {
                GUILayout.Label("No result found for specified search.");
            }
        }

        private void DrawNodeTree()
        {
            if (_currentNode != _rootNode)
            {
                if (GUILayout.Button(_currentNode.GetPath(), ButtonStyle))
                {
                    _currentNode = _currentNode.parent;
                }
            }
            
            foreach (var node in _currentNode.Nodes)
            {
                if (GUILayout.Button(node.Key, ButtonStyle))
                {
                    if (node.Value.Nodes.Count > 0)
                    {
                        _currentNode = node.Value;
                    }
                    else
                    {
                        node.Value.Execute();
                        base.editorWindow.Close();
                    }
                    break;
                }
                
                var buttonRect = GUILayoutUtility.GetLastRect();
                if (buttonRect.Contains(Event.current.mousePosition))
                {
                    _hoverNode = node.Value;
                }
                
                if (node.Value.Nodes.Count > 0)
                {
                    Rect lastRect = GUILayoutUtility.GetLastRect();
                    GUI.Label(new Rect(lastRect.x+lastRect.width-16, lastRect.y-2, 20, 20), "+", PlusStyle);
                }
            }
        }

        // TODO Possible type caching? 
        public static MenuItemNode GenerateMenuItemNodeTree(GenericMenu p_menu)
        {
            MenuItemNode rootNode = new MenuItemNode();
            if (p_menu == null)
                return rootNode;
            
            var menuItemsField = p_menu.GetType().GetField("menuItems", BindingFlags.Instance | BindingFlags.NonPublic);
            var menuItems = menuItemsField.GetValue(p_menu) as ArrayList;
            
            foreach (var menuItem in menuItems)
            {
                var menuItemType = menuItem.GetType();
                GUIContent content = (GUIContent)menuItemType.GetField("content").GetValue(menuItem);
                
                string path = content.text;
                string[] splitPath = path.Split('/');
                MenuItemNode currentNode = rootNode;
                for (int i = 0; i < splitPath.Length; i++)
                {
                    currentNode = currentNode.GetOrCreateNode(splitPath[i]);
                }

                currentNode.content = content;
                currentNode.func = (GenericMenu.MenuFunction)menuItemType.GetField("func").GetValue(menuItem);
                currentNode.func2 = (GenericMenu.MenuFunction2)menuItemType.GetField("func2").GetValue(menuItem);
                currentNode.userData = menuItemType.GetField("userData").GetValue(menuItem);
                currentNode.separator = (bool)menuItemType.GetField("separator").GetValue(menuItem);
                currentNode.on = (bool)menuItemType.GetField("on").GetValue(menuItem);
            }

            return rootNode;
        }
    }
}