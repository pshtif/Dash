/*
 *	Created by:  Peter @sHTiF Stefcek
 */

#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Dash.Editor
{
    public class MenuItemNode
    {
        public RuntimeGenericMenuItem item;

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
            if (item.callback1 != null)
            {
                item.callback1?.Invoke();
            }
            else
            {
                item.callback2?.Invoke(item.data);
            }
        }
    }
    
    public class GenericMenuPopup : PopupWindowContent
    {
        public static GenericMenuPopup Get(RuntimeGenericMenu p_menu, string p_title)
        {
            var popup = new GenericMenuPopup(p_menu, p_title);
            return popup;
        }
        
        public static GenericMenuPopup Show(RuntimeGenericMenu p_menu, string p_title, Vector2 p_position, int p_width = 200, int p_height = 200, bool p_showSearch = true, bool p_showTooltip = true) {
            var popup = new GenericMenuPopup(p_menu, p_title);
            popup.width = p_width;
            popup.height = p_height;
            popup.showSearch = p_showSearch;
            popup.showTooltip = p_showTooltip;
            popup.showTitle = !string.IsNullOrEmpty(p_title);
            PopupWindow.Show(new Rect(p_position.x, p_position.y, 0, 0), popup);
            return popup;
        }

        private GUIStyle _backStyle;
        public GUIStyle BackStyle 
        {
            get
            {
                if (_backStyle == null)
                {
                    _backStyle = new GUIStyle(GUI.skin.button);
                    _backStyle.alignment = TextAnchor.MiddleLeft;
                    _backStyle.hover.background = Texture2D.grayTexture;
                    _backStyle.normal.textColor = Color.black;
                }

                return _backStyle;
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
        private int _contentHeight;
        private bool _useScroll;
        
        public int width = 200;
        public int height = 200;
        public int maxHeight = 300;
        public bool resizeToContent = true;
        public bool showOnStatus = true;
        public bool showSearch = true;
        public bool showTooltip = false;
        public bool showTitle = false;
        

        public GenericMenuPopup(RuntimeGenericMenu p_menu, string p_title)
        {
            _title = p_title;
            showTitle = !string.IsNullOrWhiteSpace(_title);
            _currentNode = _rootNode = GenerateMenuItemNodeTree(p_menu);
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(width, height);
        }

        public void Show(float p_x, float p_y)
        {
            PopupWindow.Show(new Rect(p_x, p_y, 0, 0), this);
        }
        
        public void Show(Vector2 p_position)
        {
            PopupWindow.Show(new Rect(p_position.x, p_position.y, 0, 0), this);
        }

        public override void OnGUI(Rect p_rect)
        {
            if (Event.current.type == EventType.Layout)
            {
                _useScroll = _contentHeight > maxHeight || (!resizeToContent && _contentHeight > height);
            }

            _contentHeight = 0;
            GUIStyle style = new GUIStyle();
            style.normal.background = Texture2D.whiteTexture;
            GUI.color = new Color(0.1f, 0.1f, 0.1f, 1);
            GUI.Box(p_rect, string.Empty, style);
            GUI.color = Color.white;
            
            if (showTitle)
            {
                DrawTitle(new Rect(p_rect.x, p_rect.y, p_rect.width, 24));
            }

            if (showSearch)
            {
                DrawSearch(new Rect(p_rect.x, p_rect.y + (showTitle ? 24 : 0), p_rect.width, 20));
            }

            DrawMenuItems(new Rect(p_rect.x, p_rect.y + (showTitle ? 24 : 0) + (showSearch ? 22 : 0), p_rect.width, p_rect.height - (showTooltip ? 60 : 0) - (showTitle ? 24 : 0) - (showSearch ? 22 : 0)));

            if (showTooltip)
            {
                DrawTooltip(new Rect(p_rect.x + 5, p_rect.y + p_rect.height - 58, p_rect.width - 10, 56));
            }

            if (resizeToContent)
            {
                height = Mathf.Min(_contentHeight, maxHeight);
            }
            EditorGUI.FocusTextInControl("Search");
        }

        private void DrawTitle(Rect p_rect)
        {
            _contentHeight += 24;
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.fontStyle = FontStyle.Bold;
            style.fontSize = 16;
            style.alignment = TextAnchor.LowerCenter;
            GUI.Label(p_rect, _title, style);
        }

        private void DrawSearch(Rect p_rect)
        {
            _contentHeight += 22;
            GUI.SetNextControlName("Search");
            _search = GUI.TextArea(p_rect, _search);
        }

        private void DrawTooltip(Rect p_rect)
        {
            _contentHeight += 60;
            if (_hoverNode == null || _hoverNode.item == null || _hoverNode.item.content == null ||
                string.IsNullOrWhiteSpace(_hoverNode.item.content.tooltip))
            {
                return;
            }

            GUIStyle style = new GUIStyle();
            style.fontSize = 9;
            style.wordWrap = true;
            style.normal.textColor = Color.white;
            GUI.Label(p_rect, _hoverNode.item.content.tooltip, style);
        }

        private void DrawMenuItems(Rect p_rect)
        {
            GUILayout.BeginArea(p_rect);
            if (_useScroll) 
            {
                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUIStyle.none, GUI.skin.verticalScrollbar);
            }

            GUILayout.BeginVertical();

            if (string.IsNullOrWhiteSpace(_search) || _search.Length<2)
            {
                DrawNodeTree(p_rect);
            }
            else
            {
                DrawNodeSearch(p_rect);
            }

            GUILayout.EndVertical();
            if (_useScroll)
            {
                EditorGUILayout.EndScrollView();
            }

            GUILayout.EndArea();
        }
        
        private void DrawNodeSearch(Rect p_rect)
        {
            List<MenuItemNode> search = _rootNode.Search(_search);
            search.Sort((n1, n2) =>
            {
                string p1 = n1.parent.GetPath();
                string p2 = n2.parent.GetPath();
                if (p1 == p2)
                    return n1.name.CompareTo(n2.name);

                return p1.CompareTo(p2);
            });

            string lastPath = "";
            foreach (var node in search)
            {
                string nodePath = node.parent.GetPath();
                if (nodePath != lastPath)
                {
                    _contentHeight += 21;
                    GUILayout.Label(nodePath);
                    lastPath = nodePath;
                }

                _contentHeight += 21;
                GUI.color = _hoverNode == node ? Color.white : Color.gray;
                GUIStyle style = new GUIStyle();
                style.normal.background = Texture2D.grayTexture;
                GUILayout.BeginHorizontal(style);

                if (showOnStatus)
                {
                    style = new GUIStyle("box");
                    style.normal.background = Texture2D.whiteTexture;
                    GUI.color = node.item.state ? new Color(0, .6f, .8f) : new Color(.2f, .2f, .2f);
                    GUILayout.Box("", style, GUILayout.Width(14), GUILayout.Height(14));
                }

                GUI.color = _hoverNode == node ? Color.white : Color.white;
                GUILayout.Label(node.name);
                
                GUILayout.EndHorizontal();
                
                var nodeRect = GUILayoutUtility.GetLastRect();
                if (Event.current.type == EventType.Repaint || Event.current.isMouse)
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
                                base.editorWindow.Close();
                                node.Execute();
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
                _contentHeight += 22;
                GUILayout.Space(2);
                if (GUILayout.Button(_currentNode.GetPath(), BackStyle, GUILayout.Height(20)))
                {
                    _currentNode = _currentNode.parent;
                }
            }
            
            foreach (var node in _currentNode.Nodes)
            {
                if (node.item != null && node.item.separator)
                {
                    GUILayout.Space(4);
                    _contentHeight += 4;
                    continue;
                }

                _contentHeight += 21;
                GUI.color = _hoverNode == node ? Color.white : Color.gray;
                GUIStyle style = new GUIStyle();
                style.normal.background = Texture2D.grayTexture;
                GUILayout.BeginHorizontal(style);

                if (showOnStatus)
                {
                    style = new GUIStyle("box");
                    style.normal.background = Texture2D.whiteTexture;
                    GUI.color = (node.item != null && node.item.state) ? new Color(0, .6f, .8f, .5f) : new Color(.2f, .2f, .2f, .2f);
                    GUILayout.Box("", style, GUILayout.Width(14), GUILayout.Height(14));
                }

                GUI.color = _hoverNode == node ? Color.white : Color.white;
                style = new GUIStyle("label");
                style.fontStyle = node.Nodes.Count > 0 ? FontStyle.Bold : FontStyle.Normal;
                GUILayout.Label(node.name, style);
                
                GUILayout.EndHorizontal();
                
                var nodeRect = GUILayoutUtility.GetLastRect();
                if (Event.current.type == EventType.Repaint || Event.current.isMouse)
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
                                base.editorWindow.Close();
                                node.Execute();
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
        public static MenuItemNode GenerateMenuItemNodeTree(RuntimeGenericMenu p_menu)
        {
            MenuItemNode rootNode = new MenuItemNode();
            if (p_menu == null)
                return rootNode;

            foreach (var menuItem in p_menu.Items)
            {
                string path = menuItem.content.text;
                string[] splitPath = path.Split('/');
                MenuItemNode currentNode = rootNode;
                for (int i = 0; i < splitPath.Length; i++)
                {
                    currentNode = (i < splitPath.Length - 1)
                        ? currentNode.GetOrCreateNode(splitPath[i])
                        : currentNode.CreateNode(splitPath[i]);
                }

                currentNode.item = menuItem;
                //
                // if (menuItem.separator)
                // {
                //     currentNode.item.separator = true;
                // }
                // else
                // {
                //     currentNode.content = content;
                //     currentNode.func = (GenericMenu.MenuFunction) menuItemType.GetField("func").GetValue(menuItem);
                //     currentNode.func2 = (GenericMenu.MenuFunction2) menuItemType.GetField("func2").GetValue(menuItem);
                //     currentNode.userData = menuItemType.GetField("userData").GetValue(menuItem);
                //     currentNode.on = (bool) menuItemType.GetField("on").GetValue(menuItem);
                // }
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

#endif