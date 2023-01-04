/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Reflection;
using Dash.Attributes;
using UnityEditor;
using UnityEngine;

namespace Dash.Editor
{
    public class NodeInspectorView : ViewBase
    {
        private Vector2 scrollPosition;

        protected object _previouslyInspected;
        private float _lastHeight = -1;

        public NodeInspectorView()
        {

        }

        public override void DrawGUI(Event p_event, Rect p_rect)
        {
            if (Graph == null)
                return;

            var selectedNode = SelectionManager.GetSelectedNode(Graph);
            
            if (selectedNode != null)
            {
                DrawGraphNodeGUI(p_rect);
                if (_previouslyInspected != selectedNode) GUI.FocusControl("");
                _previouslyInspected = selectedNode;
            } else if (SelectionManager.selectedBox != null)
            {
                DrawGraphBoxGUI(p_rect);
                if (_previouslyInspected != SelectionManager.selectedBox) GUI.FocusControl("");
                _previouslyInspected = SelectionManager.selectedBox;
            }
        }

        private void DrawGraphBoxGUI(Rect p_rect)
        {
            Rect rect = new Rect(p_rect.width - 400, 30, 390, 80);
            
            DrawBoxGUI(rect, "Properties", TextAnchor.MiddleRight, Color.white, new Color(.8f,.6f,.4f), Color.gray);
            GUI.Label(new Rect(rect.x + 5, rect.y, 100, 100), "Group", DashEditorCore.Skin.GetStyle("NodePropertiesTitle"));

            GUILayout.BeginArea(new Rect(rect.x+5, rect.y+30, rect.width-10, rect.height-35));

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);

            SelectionManager.selectedBox.DrawInspector();

            GUILayout.EndScrollView();
            GUILayout.EndArea();

            UseEvent(rect);
        }

        private void DrawGraphNodeGUI(Rect p_rect) 
        {
            var selectedNode = SelectionManager.GetSelectedNode(Graph);
            
            InspectorHeightAttribute heightAttibute = selectedNode.GetType().GetCustomAttribute<InspectorHeightAttribute>();
            //float height = heightAttibute != null ? heightAttibute.height : _lastHeight;
            
            Rect rect = new Rect(p_rect.width - 400, 30, 390, _lastHeight + 40);
            
            DrawBoxGUI(rect, "Properties", TextAnchor.MiddleRight, Color.white, new Color(.8f,.6f,.4f), Color.gray);

            string nodeType = NodeBase.GetNodeNameFromType(selectedNode.GetType());
            GUI.Label(new Rect(rect.x + 5, rect.y, 100, 100), nodeType, DashEditorCore.Skin.GetStyle("NodePropertiesTitle"));
            
            DrawDocumentationButton(rect, selectedNode.GetType());
            
            DrawScriptButton(rect, selectedNode.GetType());
            
            GUILayout.BeginArea(new Rect(rect.x+5, rect.y+30, rect.width-10, rect.height-35));
            
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);
            
            selectedNode.DrawInspector();

            if (Event.current.type == EventType.Repaint)
            {
                var lastRect = GUILayoutUtility.GetLastRect();
                GUILayout.EndScrollView();
                GUILayout.EndArea();
                var lastHeight = lastRect.y + lastRect.height;
                lastHeight = lastHeight > 380 ? 380 : lastHeight;

                if (lastHeight != _lastHeight)
                {
                    _lastHeight = lastHeight;
                    // Hack for faster repaint correction
                    DashEditorWindow.Instance.Repaint();
                }
            }
            else
            {
                GUILayout.EndScrollView();
                GUILayout.EndArea();
            }

            selectedNode.DrawInspectorControls(rect);
            
            UseEvent(rect);
        }

        void DrawDocumentationButton(Rect p_rect, Type p_type)
        {
            DocumentationAttribute documentation = p_type.GetCustomAttribute<DocumentationAttribute>();

            if (documentation != null)
            {
                if (GUI.Button(new Rect(p_rect.x + 270, p_rect.y + 7, 16, 16),
                    IconManager.GetIcon("help_icon"), GUIStyle.none))
                {
                    if (documentation.url.StartsWith("http"))
                    {
                        Application.OpenURL(documentation.url);
                    }
                    else
                    {
                        Application.OpenURL(
                            "https://github.com/pshtif/Dash/blob/main/Documentation/" + documentation.url);
                    }
                }
            }
        }
        
        void DrawScriptButton(Rect p_rect, Type p_type)
        {
            if (GUI.Button(new Rect(p_rect.x+290, p_rect.y+7, 16, 16),
                IconManager.GetIcon("script_icon"), GUIStyle.none))
            {
                AssetDatabase.OpenAsset(EditorUtils.GetScriptFromType(p_type), 1);
            }
        }
    }
}