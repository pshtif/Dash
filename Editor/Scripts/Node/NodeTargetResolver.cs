/*
 *	Created by:  Peter @sHTiF Stefcek
 */

#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;

namespace Dash.Editor
{
    public class NodeTargetResolver
    {
        static private NodeBase _lastResolvedNode;
        static private Transform _lastResolvedTarget;
        static private int _resolveCount;
        
        public static bool ResolveInputNodeChain(NodeBase p_node, ref List<NodeConnection> p_chain)
        {
            _resolveCount++;
            var connections = p_node.Graph.GetInputConnections(p_node);
            if (connections.Count > 0)
            {
                if (p_chain.Contains(connections[0]))
                    return false;
                
                p_chain.Insert(0, connections[0]);
                return ResolveInputNodeChain(connections[0].outputNode, ref p_chain);
            }

            return true;
        }

        internal static Transform ResolveEditorTarget(NodeBase p_node)
        {
            if (DashEditorCore.EditorConfig.editingController == null)
                return null;
        
            if (_lastResolvedNode == p_node)
                return _lastResolvedTarget;
            
            _lastResolvedTarget = null;
            _lastResolvedNode = p_node;
            List<NodeConnection> chain = new List<NodeConnection>();
            Transform retarget = DashEditorCore.EditorConfig.editingController.transform;
            if (retarget == null)
                return null;

            _resolveCount = 0;
            if (ResolveInputNodeChain(p_node, ref chain))
            {
                foreach (var connection in chain)
                {
                    retarget = connection.outputNode.ResolveNodeRetarget(retarget, connection);
                }

                _lastResolvedTarget = p_node.ResolveNodeRetarget(retarget, null);
            }
            else
            {
                _lastResolvedTarget = null;
            }

            return _lastResolvedTarget;
        }
    }
}
#endif