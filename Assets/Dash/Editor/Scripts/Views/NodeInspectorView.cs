/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System.Reflection;
using Dash.Attributes;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace Dash
{
    public class NodeInspectorView : ViewBase
    {
        private Vector2 scrollPosition;

        protected NodeBase _previousSelectedNode;

        public NodeBase SelectedNode => Graph == null
            ? null
            : DashEditorCore.selectedNodes.Count == 1
            ? Graph.Nodes[DashEditorCore.selectedNodes[0]]
            : null;

        public NodeInspectorView()
        {

        }

        public override void UpdateGUI(Event p_event, Rect p_rect)
        {
            float height = 0;

            if (SelectedNode == null)
            {
                _previousSelectedNode = null;
                return;
            }

            InspectorHeightAttribute heightAttibute = SelectedNode.GetType().GetCustomAttribute<InspectorHeightAttribute>();
            height = heightAttibute != null ? heightAttibute.height : 340;

            if (_previousSelectedNode != SelectedNode)
            {
                GUI.FocusControl("");
                _previousSelectedNode = SelectedNode;
            }

            Rect rect = new Rect(p_rect.width - 350, 30, 340, height);
            
            DrawBoxGUI(rect, "Properties", TextAnchor.UpperRight);

            string nodeType = NodeBase.GetNodeNameFromType(SelectedNode.GetType());
            GUI.Label(new Rect(rect.x + 5, rect.y, 100, 100), nodeType, DashEditorCore.Skin.GetStyle("NodePropertiesTitle"));
            
            DrawScriptButton(rect);
            

            GUILayout.BeginArea(new Rect(rect.x+5, rect.y+30, rect.width-10, rect.height-35));

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);

            SelectedNode.DrawInspector();

            GUILayout.EndScrollView();
            GUILayout.EndArea();

            if (rect.Contains(p_event.mousePosition) &&
                p_event.type == EventType.MouseDown)
            {
                p_event.type = EventType.Used;
            }
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