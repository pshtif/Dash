/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Dash.Attributes;
using Dash.Editor;
using OdinSerializer.Utilities;
using UnityEditor;
using UnityEngine;

namespace Dash
{
    [Attributes.Tooltip("Stops graph flow.")]
    [Attributes.Category(NodeCategoryType.LOGIC)]
    [InputCount(1)]
    [OutputCount(1)]
    public class StopNode : NodeBase<StopNodeModel>
    {
        override protected void OnExecuteStart(NodeFlowData p_flowData)
        {
            var stopMode = GetParameterValue(Model.stopMode, p_flowData);

            switch (stopMode)
            {
                case StopMode.GRAPH:
                    _graph.Stop();
                    break;
                case StopMode.CONNECTED:
                    StopConnectedNodes(this);
                    break;
                default:
                    break;
            }

            var continueSelf = GetParameterValue(Model.continueSelf, p_flowData);

            OnExecuteEnd();

            if (continueSelf)
            {
                OnExecuteOutput(0, p_flowData);
            }
        }
        
        private List<NodeBase> _stoppedNodes;
        
        private void StopConnectedNodes(NodeBase p_node)
        {
            if (_stoppedNodes.Contains(p_node))
                return;

            p_node.Stop();
            
            _stoppedNodes.Add(p_node);
            
            var connections = Graph.Connections.FindAll(c => c.outputNode == p_node);
            
            foreach (var connection in connections)
            {
                StopConnectedNodes(connection.inputNode);   
            }
        }
    }
}
