/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Dash
{
    [Attributes.Tooltip("Connector node without functionality helps with graph and connection management.")]
    [Category(NodeCategoryType.LOGIC)]
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
        public override Rect GetConnectorRect(bool p_input, int p_index)
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

            if (Graph.connectingNode != null && Graph.connectingNode != this)
            {
                if (GUI.Button(connectorRect, "", skin.GetStyle(isConnected ? "NodeConnectorOn" : "NodeConnectorOff")))
                {
                    if (Event.current.button == 0)
                    {
                        Undo.RegisterCompleteObjectUndo(_graph, "Connect node");
                        Graph.Connect(this, 0, Graph.connectingNode, Graph.connectingOutputIndex);
                        DashEditorCore.SetDirty();
                        Graph.connectingNode = null;
                    }
                }
            }

            isConnected = Graph.HasOutputConnected(this, 0);
            GUI.color = isConnected
                ? DashEditorCore.EditorConfig.theme.ConnectorOutputConnectedColor
                : DashEditorCore.EditorConfig.theme.ConnectorOutputDisconnectedColor;

            if (Graph.connectingNode == this && Graph.connectingOutputIndex == 0)
                GUI.color = Color.green;

            connectorRect = new Rect(rect.x + Graph.viewOffset.x + Size.x / 2 - 12,
                rect.y + Graph.viewOffset.y + Size.y / 2 - 12, 24, 24);
                
            if (connectorRect.Contains(Event.current.mousePosition - new Vector2(p_rect.x, p_rect.y)))
                GUI.color = Color.green;

            if (GUI.Button(connectorRect, "", skin.GetStyle(isConnected ? "NodeConnectorOn" : "NodeConnectorOff")))
            {
                if (Event.current.button == 0)
                {
                    Graph.connectingOutputIndex = 0;
                    Graph.connectingNode = this;
                }
            }
        }
        #endif
    }
}