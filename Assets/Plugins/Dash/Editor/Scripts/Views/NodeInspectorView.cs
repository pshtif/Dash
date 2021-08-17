/*
 *	Created by:  Peter @sHTiF Stefcek
 */

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

        public NodeBase SelectedNode => Graph == null
            ? null
            : DashEditorCore.selectedNodes != null && DashEditorCore.selectedNodes.Count == 1
            ? Graph.Nodes[DashEditorCore.selectedNodes[0]]
            : null;

        public NodeInspectorView()
        {

        }

        public override void DrawGUI(Event p_event, Rect p_rect)
        {
            if (SelectedNode != null)
            {
                DrawNodeGUI(p_rect);
                if (_previouslyInspected != SelectedNode) GUI.FocusControl("");
                _previouslyInspected = SelectedNode;
            } else if (DashEditorCore.selectedBox != null)
            {
                DrawBoxGUI(p_rect);
                if (_previouslyInspected != DashEditorCore.selectedBox) GUI.FocusControl("");
                _previouslyInspected = DashEditorCore.selectedBox;
            }
        }

        private void DrawBoxGUI(Rect p_rect)
        {
            Rect rect = new Rect(p_rect.width - 350, 30, 340, 340);
            
            DrawBoxGUI(rect, "Properties", TextAnchor.UpperRight, Color.white);
            
            // GUI.Label(new Rect(rect.x + 5, rect.y, 100, 100), "Properties", DashEditorCore.Skin.GetStyle("NodePropertiesTitle"));
            
            GUILayout.BeginArea(new Rect(rect.x+5, rect.y+30, rect.width-10, rect.height-35));

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);

            DashEditorCore.selectedBox.DrawInspector();

            GUILayout.EndScrollView();
            GUILayout.EndArea();

            UseEvent(rect);
        }

        private void DrawNodeGUI(Rect p_rect) 
        {
            InspectorHeightAttribute heightAttibute = SelectedNode.GetType().GetCustomAttribute<InspectorHeightAttribute>();
            float height = heightAttibute != null ? heightAttibute.height : 340;

            Rect rect = new Rect(p_rect.width - 350, 30, 340, height);
            
            DrawBoxGUI(rect, "Properties", TextAnchor.UpperRight, Color.white);

            string nodeType = NodeBase.GetNodeNameFromType(SelectedNode.GetType());
            GUI.Label(new Rect(rect.x + 5, rect.y, 100, 100), nodeType, DashEditorCore.Skin.GetStyle("NodePropertiesTitle"));
            
            DrawScriptButton(rect);

            GUILayout.BeginArea(new Rect(rect.x+5, rect.y+30, rect.width-10, rect.height-35));

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);

            SelectedNode.DrawInspector();

            GUILayout.EndScrollView();
            GUILayout.EndArea();
            
            UseEvent(rect);
        }
        
        void DrawScriptButton(Rect p_rect)
        {
            if (GUI.Button(new Rect(p_rect.x+242, p_rect.y+7, 16, 16),
                IconManager.GetIcon("Script_Icon"), GUIStyle.none))
            {
                AssetDatabase.OpenAsset(EditorUtils.GetScriptFromType(SelectedNode.GetType()), 1);
            }
        }
    }
}