/*
 *	Created by:  Peter @sHTiF Stefcek
 */

#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Dash.Editor
{
    public class RuntimeGenericMenuItem
    {
        public GUIContent content;
        public bool separator;
        public bool state;
        public Action callback1;
        public Action<object> callback2;
        public object data;

        public RuntimeGenericMenuItem()
        {
            
        }
        
        public RuntimeGenericMenuItem(GUIContent p_content, bool p_separator, bool p_state, Action p_callback)
        {
            content = p_content;
            separator = p_separator;
            state = p_state;
            callback1 = p_callback;
        }
        
        public RuntimeGenericMenuItem(GUIContent p_content, bool p_separator, bool p_state, Action<object> p_callback, object p_data)
        {
            content = p_content;
            separator = p_separator;
            state = p_state;
            callback2 = p_callback;
            data = p_data;
        }
    }
    
    public class RuntimeGenericMenu
    {
        public List<RuntimeGenericMenuItem> Items { get; private set; } = new List<RuntimeGenericMenuItem>();
        
        public void AddItem(GUIContent p_content, bool p_state, Action p_callback)
        {
            Items.Add(new RuntimeGenericMenuItem(p_content, false, p_state, p_callback));
        }

        public void AddItem(GUIContent p_content, bool p_state, Action<object> p_callback, object p_data)
        {
            Items.Add(new RuntimeGenericMenuItem(p_content, false, p_state, p_callback, p_data));
        }

        public void AddSeparator(string p_path)
        {
            Items.Add(new RuntimeGenericMenuItem(new GUIContent(p_path), true, false, null));   
        }
        
        public void ShowAsEditorMenu()
        {
            GenericMenu editorMenu = new GenericMenu();

            foreach (var item in Items)
            {
                if (!item.separator)
                {
                    if (item.callback2 != null)
                    {
                        editorMenu.AddItem(item.content, item.state, (data) => item.callback2.Invoke(data), item.data);   
                    }
                    else
                    {
                        editorMenu.AddItem(item.content, item.state, () => item.callback1.Invoke());
                    }
                }
                else
                {
                    editorMenu.AddSeparator(item.content.text);
                }
            }
            
            editorMenu.ShowAsContext();
        } 
        
        // public static MenuItemNode GenerateMenuItemNodeTree(RuntimeGenericMenu p_menu)
        // {
        //     MenuItemNode rootNode = new MenuItemNode();
        //     if (p_menu == null)
        //         return rootNode;
        //     
        //     var menuItemsField = p_menu.GetType().GetField("menuItems", BindingFlags.Instance | BindingFlags.NonPublic);
        //
        //     if (menuItemsField == null)
        //     {
        //         menuItemsField = p_menu.GetType().GetField("m_MenuItems", BindingFlags.Instance | BindingFlags.NonPublic);
        //     }
        //     
        //     if (menuItemsField == null)
        //         return rootNode;
        //
        //     var menuItems = menuItemsField.GetValue(p_menu) as IEnumerable;
        //
        //     foreach (var menuItem in menuItems)
        //     {
        //         var menuItemType = menuItem.GetType();
        //         GUIContent content = (GUIContent)menuItemType.GetField("content").GetValue(menuItem);
        //         
        //         bool separator = (bool)menuItemType.GetField("separator").GetValue(menuItem);
        //         string path = content.text;
        //         string[] splitPath = path.Split('/');
        //         MenuItemNode currentNode = rootNode;
        //         for (int i = 0; i < splitPath.Length; i++)
        //         {
        //             currentNode = (i < splitPath.Length - 1)
        //                 ? currentNode.GetOrCreateNode(splitPath[i])
        //                 : currentNode.CreateNode(splitPath[i]);
        //         }
        //
        //         if (separator)
        //         {
        //             currentNode.separator = true;
        //         }
        //         else
        //         {
        //             currentNode.content = content;
        //             currentNode.func = (GenericMenu.MenuFunction) menuItemType.GetField("func").GetValue(menuItem);
        //             currentNode.func2 = (GenericMenu.MenuFunction2) menuItemType.GetField("func2").GetValue(menuItem);
        //             currentNode.userData = menuItemType.GetField("userData").GetValue(menuItem);
        //             currentNode.on = (bool) menuItemType.GetField("on").GetValue(menuItem);
        //         }
        //     }
        //
        //     return rootNode;
        // }
    }
}
#endif