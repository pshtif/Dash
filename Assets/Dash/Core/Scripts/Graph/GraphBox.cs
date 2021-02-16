/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using Dash.Attributes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Dash
{
    [Serializable]
    public class GraphBox
    {
        public DashGraph Graph => DashEditorCore.Config.editingGraph;
        
        public string comment;

        public Color color = Color.white;
        
        [HideInInspector]
        public Rect rect;

        public Rect titleRect => new Rect(rect.x, rect.y, rect.width, 45);

        private List<NodeBase> _draggedNodes = new List<NodeBase>();
        private double _lastClickTime = 0;

        private Dictionary<string, bool> groupsMinized;

        public GraphBox(string p_comment, Rect p_rect)
        {
            comment = p_comment;
            rect = p_rect;
        }

        public void DrawGUI()
        {
            Rect offsetRect = new Rect(rect.x + Graph.viewOffset.x, rect.y + Graph.viewOffset.y, rect.width, rect.height);

            GUI.color = color;
            
            GUI.Box(offsetRect, "", DashEditorCore.Skin.GetStyle("GraphRegion"));

            Rect titleRect = new Rect(offsetRect.x + 12, offsetRect.y, offsetRect.width, 40);
            if (Event.current.type == EventType.MouseDown && titleRect.Contains(Event.current.mousePosition))
            {
                if (EditorApplication.timeSinceStartup - _lastClickTime < 0.3)
                {
                    DashEditorCore.editingBoxComment = this;
                }
                _lastClickTime = EditorApplication.timeSinceStartup;
            }

            GUI.color = Color.white;
            GUIStyle style = new GUIStyle();
            style.fontStyle = FontStyle.Bold;
            style.fontSize = 24;
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.LowerLeft;
            if (DashEditorCore.editingBoxComment == this)
            {
                comment = GUI.TextField(titleRect, comment, style);
            }
            else
            {
                GUI.Label(titleRect, comment, style);
            }
            
        }

        public void StartDrag()
        {
            _draggedNodes = Graph.Nodes.FindAll(n =>
                rect.Contains(new Vector2(n.rect.x, n.rect.y)) &&
                rect.Contains(new Vector2(n.rect.x + n.rect.width, n.rect.y + n.rect.height)));
        }

        public void Drag(Vector2 p_offset)
        {
            _draggedNodes.ForEach(n => n.rect.position += p_offset);

            rect.position += p_offset;
        }

        public virtual bool DrawInspector()
        {
            bool initializeMinimization = false;
            if (groupsMinized == null)
            {
                initializeMinimization = true;
                groupsMinized = new Dictionary<string, bool>();
            }
            
            GUILayout.Space(5);
            
            GUIStyle minStyle = GUIStyle.none;
            minStyle.normal.textColor = Color.white;
            minStyle.fontSize = 16;
            
            var fields = this.GetType().GetFields();
            Array.Sort(fields, GUIPropertiesUtils.GroupSort);
            string lastGroup = "";
            bool lastGroupMinimized = false;
            bool invalidate = false;
            foreach (var field in fields)
            {
                if (field.IsConstant()) continue;

                TitledGroupAttribute ga = field.GetCustomAttribute<TitledGroupAttribute>();
                string currentGroup = ga != null ? ga.Group : "Other";
                if (currentGroup != lastGroup)
                {
                    if (initializeMinimization || !groupsMinized.ContainsKey(currentGroup))
                    {
                        groupsMinized[currentGroup] = ga != null ? ga.Minimized : false;
                    }

                    GUIPropertiesUtils.Separator(16, 2, 4, new Color(0.1f, 0.1f, 0.1f));
                    GUILayout.Label(currentGroup, DashEditorCore.Skin.GetStyle("PropertyGroup"),
                        GUILayout.Width(120));
                    Rect lastRect = GUILayoutUtility.GetLastRect();


                    if (GUI.Button(new Rect(lastRect.x + 302, lastRect.y - 25, 20, 20), groupsMinized[currentGroup] ? "+" : "-",
                        minStyle))
                    {
                        groupsMinized[currentGroup] = !groupsMinized[currentGroup];
                    }

                    lastGroup = currentGroup;
                    lastGroupMinimized = groupsMinized[currentGroup];
                }

                if (lastGroupMinimized)
                    continue;

                invalidate = GUIPropertiesUtils.PropertyField(field, this);
            }

            return invalidate;
        }
    }
}