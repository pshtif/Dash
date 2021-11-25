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
            } else if (DashEditorCore.selectedBox != null)
            {
                DrawGraphBoxGUI(p_rect);
                if (_previouslyInspected != DashEditorCore.selectedBox) GUI.FocusControl("");
                _previouslyInspected = DashEditorCore.selectedBox;
            }
        }

        private void DrawGraphBoxGUI(Rect p_rect)
        {
            Rect rect = new Rect(p_rect.width - 400, 30, 390, 340);
            
            DrawBoxGUI(rect, "Properties", TextAnchor.UpperRight, Color.white);
            
            // GUI.Label(new Rect(rect.x + 5, rect.y, 100, 100), "Properties", DashEditorCore.Skin.GetStyle("NodePropertiesTitle"));
            
            GUILayout.BeginArea(new Rect(rect.x+5, rect.y+30, rect.width-10, rect.height-35));

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);

            DashEditorCore.selectedBox.DrawInspector();

            GUILayout.EndScrollView();
            GUILayout.EndArea();

            UseEvent(rect);
        }

        private void DrawGraphNodeGUI(Rect p_rect) 
        {
            var selectedNode = SelectionManager.GetSelectedNode(Graph);
            
            InspectorHeightAttribute heightAttibute = selectedNode.GetType().GetCustomAttribute<InspectorHeightAttribute>();
            float height = heightAttibute != null ? heightAttibute.height : 340;

            Rect rect = new Rect(p_rect.width - 400, 30, 390, height);
            
            DrawBoxGUI(rect, "Properties", TextAnchor.UpperRight, Color.white);

            string nodeType = NodeBase.GetNodeNameFromType(selectedNode.GetType());
            GUI.Label(new Rect(rect.x + 5, rect.y, 100, 100), nodeType, DashEditorCore.Skin.GetStyle("NodePropertiesTitle"));
            
            DrawScriptButton(rect, selectedNode.GetType());
            
            GUILayout.BeginArea(new Rect(rect.x+5, rect.y+30, rect.width-10, rect.height-35));
            
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);
            
            selectedNode.DrawInspector();
            
            GUILayout.EndScrollView();
            GUILayout.EndArea();

            selectedNode.DrawInspectorControls(rect);
            
            UseEvent(rect);
        }
        
        void DrawScriptButton(Rect p_rect, Type p_type)
        {

            if (GUI.Button(new Rect(p_rect.x+290, p_rect.y+7, 16, 16),
                IconManager.GetIcon("Script_Icon"), GUIStyle.none))
            {
                AssetDatabase.OpenAsset(EditorUtils.GetScriptFromType(p_type), 1);
            }
        }
    }
}