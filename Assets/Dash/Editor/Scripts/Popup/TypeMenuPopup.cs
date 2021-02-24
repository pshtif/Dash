/*
 *	Created by:  Peter @sHTiF Stefcek
 */

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
        
        public List<MenuItemNode> Nodes { get; private set; }
        
        public MenuItemNode(string p_name = "", MenuItemNode p_parent = null)
        {
            name = p_name;
            parent = p_parent;
            Nodes = new List<MenuItemNode>();
        }

        public MenuItemNode CreateNode(string p_name)
        {
            var node = new MenuItemNode(p_name, this);
            Nodes.Add(node);
            return node;
        }

        // TODO Optimize
        public MenuItemNode GetOrCreateNode(string p_name)
        {
            var node = Nodes.Find(n => n.name == p_name);
            if (node == null)
            {
                node = CreateNode(p_name);
            }

            return node;
        }

        public List<MenuItemNode> Search(string p_search)
        {
            p_search = p_search.ToLower();
            List<MenuItemNode> result = new List<MenuItemNode>();
            
            foreach (var node in Nodes)
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

        private GUIStyle _buttonStyle;
        public GUIStyle ButtonStyle 
        {
            get
            {
                if (_buttonStyle == null)
                {
                    _buttonStyle = new GUIStyle(GUI.skin.button);
                    _buttonStyle.alignment = TextAnchor.MiddleLeft;
                    _buttonStyle.hover.background = Texture2D.grayTexture;
                    _buttonStyle.normal.textColor = Color.black;
                }

                return _buttonStyle;
            }
        }

        private GUIStyle _plusStyle;
        public GUIStyle PlusStyle
        {
            get {
                if (_plusStyle == null)
                {
                    _plusStyle = new GUIStyle();
                    _plusStyle.fontStyle = FontStyle.Bold;
                    _plusStyle.normal.textColor = Color.white;
                    _plusStyle.fontSize = 16;
                }

                return _plusStyle;
            }
        }
        
        private string _title;

        private Vector2 _scrollPosition;
        private MenuItemNode _rootNode;
        private MenuItemNode _currentNode;
        private MenuItemNode _hoverNode;
        private string _search;
        private bool _repaint = false;

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
                DrawNodeTree(p_rect);
            }
            else
            {
                DrawNodeSearch(p_rect);
            }

            GUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();
        }
        
        private void DrawNodeSearch(Rect p_rect)
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
                
                ButtonStyle.fontStyle = node.Nodes.Count > 0 ? FontStyle.Bold : FontStyle.Normal;
                GUI.color = _hoverNode == node ? Color.white : Color.gray;
                GUIStyle style = new GUIStyle();
                style.normal.background = Texture2D.grayTexture;
                GUILayout.BeginHorizontal(style);
                GUI.color = _hoverNode == node ? Color.white : Color.white;
                GUILayout.Label(node.name);
                GUILayout.EndHorizontal();
                
                var nodeRect = GUILayoutUtility.GetLastRect();
                if (Event.current.isMouse)
                {
                    if (nodeRect.Contains(Event.current.mousePosition))
                    {
                        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                        {
                            if (node.Nodes.Count > 0)
                            {
                                _currentNode = node;
                                _repaint = true;
                            }
                            else
                            {
                                node.Execute();
                                base.editorWindow.Close();
                            }

                            break;
                        }
                        
                        if (_hoverNode != node)
                        {
                            _hoverNode = node;
                            _repaint = true;
                        }
                    }
                    else if (_hoverNode == node)
                    {
                        _hoverNode = null;
                        _repaint = true;
                    }
                }
            }

            if (search.Count == 0)
            {
                GUILayout.Label("No result found for specified search.");
            }
        }

        private void DrawNodeTree(Rect p_rect)
        {
            if (_currentNode != _rootNode)
            {
                if (GUILayout.Button(_currentNode.GetPath(), ButtonStyle))
                {
                    _currentNode = _currentNode.parent;
                }
            }

            int i = 0;
            foreach (var node in _currentNode.Nodes)
            {
                if (node.separator)
                {
                    GUILayout.Space(4);
                    continue;
                }
                
                ButtonStyle.fontStyle = node.Nodes.Count > 0 ? FontStyle.Bold : FontStyle.Normal;
                GUI.color = _hoverNode == node ? Color.white : Color.gray;
                GUIStyle style = new GUIStyle();
                style.normal.background = Texture2D.grayTexture;
                GUILayout.BeginHorizontal(style);
                GUI.color = _hoverNode == node ? Color.white : Color.white;
                GUILayout.Label(node.name);
                GUILayout.EndHorizontal();
                
                var nodeRect = GUILayoutUtility.GetLastRect();
                if (Event.current.isMouse)
                {
                    if (nodeRect.Contains(Event.current.mousePosition))
                    {
                        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                        {
                            if (node.Nodes.Count > 0)
                            {
                                _currentNode = node;
                                _repaint = true;
                            }
                            else
                            {
                                node.Execute();
                                base.editorWindow.Close();
                            }

                            break;
                        }
                        
                        if (_hoverNode != node)
                        {
                            _hoverNode = node;
                            _repaint = true;
                        }
                    }
                    else if (_hoverNode == node)
                    {
                        _hoverNode = null;
                        _repaint = true;
                    }
                }

                if (node.Nodes.Count > 0)
                {
                    Rect lastRect = GUILayoutUtility.GetLastRect();
                    GUI.Label(new Rect(lastRect.x+lastRect.width-16, lastRect.y-2, 20, 20), "+", PlusStyle);
                }
            }
        }
        
        void OnEditorUpdate() {
            if (_repaint)
            {
                _repaint = false;
                base.editorWindow.Repaint();
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
                
                bool separator = (bool)menuItemType.GetField("separator").GetValue(menuItem);
                string path = content.text;
                string[] splitPath = path.Split('/');
                MenuItemNode currentNode = rootNode;
                for (int i = 0; i < splitPath.Length; i++)
                {
                    currentNode = (i < splitPath.Length - 1)
                        ? currentNode.GetOrCreateNode(splitPath[i])
                        : currentNode.CreateNode(splitPath[i]);
                }

                if (separator)
                {
                    currentNode.separator = true;
                }
                else
                {
                    currentNode.content = content;
                    currentNode.func = (GenericMenu.MenuFunction) menuItemType.GetField("func").GetValue(menuItem);
                    currentNode.func2 = (GenericMenu.MenuFunction2) menuItemType.GetField("func2").GetValue(menuItem);
                    currentNode.userData = menuItemType.GetField("userData").GetValue(menuItem);
                    currentNode.on = (bool) menuItemType.GetField("on").GetValue(menuItem);
                }
            }

            return rootNode;
        }
        
        public override void OnOpen() 
        {
            EditorApplication.update -= OnEditorUpdate;
            EditorApplication.update += OnEditorUpdate;
        }
        
        public override void OnClose() 
        {
            EditorApplication.update -= OnEditorUpdate;
        }
    }
}