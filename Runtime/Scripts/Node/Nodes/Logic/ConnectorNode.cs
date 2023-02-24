/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;
using Dash.Editor;
using UnityEngine;

namespace Dash
{
    [Attributes.Tooltip("Connector node without functionality helps with graph and connection management.")]
    [Category(NodeCategoryType.GRAPH)]
    [OutputCount(1)]
    [InputCount(1)]
    [DisableBaseGUI]
    [Size(45,25)]
    public class ConnectorNode : NodeBase<NullNodeModel>
    {
        protected override void OnExecuteStart(NodeFlowData p_flowData)
        {
            OnExecuteEnd();
            OnExecuteOutput(0, p_flowData);
        }

        #if UNITY_EDITOR
        public override Rect GetConnectorRect(NodeConnectorType p_type, int p_index)
        {
            return new Rect(rect.x + Graph.viewOffset.x + Size.x / 2 - 12,
                rect.y + Graph.viewOffset.y + Size.y / 2 - 12, 24, 24);
        }

        protected override void DrawConnectors(Rect p_rect)
        {
            GUISkin skin = DashEditorCore.Skin;
            
            bool isConnected = Graph.HasInputConnected(this, 0);
            GUI.color = isConnected
                ? DashEditorCore.EditorConfig.theme.ConnectorInputConnectedColor
                : DashEditorCore.EditorConfig.theme.ConnectorInputDisconnectedColor;

            if (IsExecuting)
                GUI.color = Color.cyan;

            var connectorRect = new Rect(rect.x + Graph.viewOffset.x + Size.x / 2 - 12,
                rect.y + Graph.viewOffset.y + Size.y / 2 - 12, 24, 24);

            if (SelectionManager.connectingNode != null && SelectionManager.connectingNode != this)
            {
                GUI.Label(connectorRect, "", skin.GetStyle(isConnected ? "NodeConnectorOn" : "NodeConnectorOff"));
            }

            isConnected = Graph.HasOutputConnected(this, 0);
            GUI.color = isConnected
                ? DashEditorCore.EditorConfig.theme.ConnectorOutputConnectedColor
                : DashEditorCore.EditorConfig.theme.ConnectorOutputDisconnectedColor;

            if (SelectionManager.connectingNode == this && SelectionManager.connectingIndex == 0)
                GUI.color = Color.green;

            connectorRect = new Rect(rect.x + Graph.viewOffset.x + Size.x / 2 - 12,
                rect.y + Graph.viewOffset.y + Size.y / 2 - 12, 24, 24);
                
            if (connectorRect.Contains(Event.current.mousePosition - new Vector2(p_rect.x, p_rect.y)))
                GUI.color = Color.green;

            GUI.Label(connectorRect, "", skin.GetStyle(isConnected ? "NodeConnectorOn" : "NodeConnectorOff"));
            GUI.color = Color.white;
        }
        #endif
    }
}