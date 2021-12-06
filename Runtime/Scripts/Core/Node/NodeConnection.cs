/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using UnityEditor;
using UnityEngine;

namespace Dash
{
    [Serializable]
    public class NodeConnection
    {
        public bool active = true;

        public int inputIndex;
        public int outputIndex { get; }
        
        public NodeBase inputNode { get; private set; }
        public NodeBase outputNode { get; }
        
        #if UNITY_EDITOR
        [NonSerialized] 
        public float executeTime = 0;

        [NonSerialized] 
        public bool buttonDown = false;
        #endif

        public bool IsValid()
        {
            return inputNode != null && outputNode != null && inputIndex < inputNode.InputCount &&
                   outputIndex < outputNode.OutputCount;
        }

        public NodeConnection(int p_inputIndex, NodeBase p_inputNode, int p_outputIndex, NodeBase p_outputNode)
        {
            inputIndex = p_inputIndex;
            inputNode = p_inputNode;

            outputIndex = p_outputIndex;
            outputNode = p_outputNode;
        }

        public void Execute(NodeFlowData p_flowData)
        {
#if UNITY_EDITOR
            executeTime = 1;
#endif
            p_flowData.inputIndex = inputIndex;
            inputNode.Execute(p_flowData);
        }
        
        #if UNITY_EDITOR
        public void DrawGUI()
        {
            if (!IsValid())
                return;

            Rect outputRect = outputNode.GetConnectorRect(false, outputIndex);
            Vector3 startPos = new Vector3(outputRect.x + outputRect.width / 2, outputRect.y + outputRect.height / 2);

            Rect inputRect = inputNode.GetConnectorRect(true, inputIndex);
            Vector3 endPos = new Vector3(inputRect.x + inputRect.width / 2, inputRect.y + inputRect.height / 2);

            Color connectionColor = active
                ? DashEditorCore.EditorConfig.theme.ConnectionActiveColor
                : DashEditorCore.EditorConfig.theme.ConnectionInactiveColor;

            if (executeTime > 0)
            {
                executeTime -= .2f;
                connectionColor = Color.cyan;
            }
            
            DrawBezier(startPos, endPos, connectionColor, true);

            DrawButton(startPos, endPos, connectionColor);

            Handles.EndGUI();
        }

        void DrawButton(Vector2 p_startPos, Vector2 p_endPos, Color p_connectionColor)
        {
            var pos = (p_startPos + p_endPos) / 2;
            var buttonRect = new Rect(pos.x - 6, pos.y - 6, 12, 12);
            
            //GUI.color = buttonRect.Contains(Event.current.mousePosition) ? Color.green : p_connectionColor;
            GUI.color = p_connectionColor;
            GUI.Box(buttonRect, "", DashEditorCore.Skin.GetStyle("NodeReconnect"));

            if (Event.current.button == 0)
            {
                if (Event.current.type == EventType.MouseUp && buttonRect.Contains(Event.current.mousePosition))
                {
                    DashEditorCore.EditorConfig.editingGraph.Reconnect(this); 
                }
            }
        }

        static public void DrawBezier(Vector3 p_startPos, Vector3 p_endPos, Color p_color, bool p_shadow)
        {
            Handles.BeginGUI();
            
            Vector3 startTan = p_startPos + Vector3.right * 50;
            Vector3 mouseTan = p_endPos + Vector3.left * 50;
            Color shadowColor = new Color(0, 0, 0, .06f);

            if (p_shadow)
            {
                for (int i = 0; i < 3; ++i)
                {
                    Handles.DrawBezier(p_startPos, p_endPos, startTan, mouseTan, shadowColor, null, (i + 1) * 6);
                }
            }

            Handles.DrawBezier(p_startPos, p_endPos, startTan, mouseTan, p_color, null, 4);

            Handles.EndGUI();
        }
        
        static public void DrawConnectionToMouse(NodeBase p_outputNode, int p_outputIndex, Vector2 p_mousePosition)
        {
            if (p_outputNode == null)
                return;
            
            Rect outputRect = p_outputNode.GetConnectorRect(false, p_outputIndex);
            Vector3 startPos = new Vector3(outputRect.x + outputRect.width / 2, outputRect.y + outputRect.height / 2);

            DrawBezier(startPos, p_mousePosition, Color.green, false);
        }
        #endif
    }
}