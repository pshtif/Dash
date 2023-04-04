/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    [Attributes.Tooltip("Executes outputs in a sequence")]
    [Category(NodeCategoryType.LOGIC)]
    [InputCount(1)]
    public class SequenceNode : NodeBase<SequenceNodeModel>
    {
        public override int OutputCount
        {
            get
            {
                var connected = Graph.Connections.FindAll(nc => this == nc.outputNode);
                if (connected.Count == 0)
                    return 1;

                int currentOutputCount = 1;
                connected.ForEach(nc =>
                    currentOutputCount = nc.outputIndex > currentOutputCount - 1 ? nc.outputIndex + 1 : currentOutputCount);
                
                return currentOutputCount+1;
            }
        }

        protected override void OnExecuteStart(NodeFlowData p_flowData)
        {
            OnExecuteEnd();
            
            for (int i = 0; i < OutputCount; i++)
            {
                OnExecuteOutput(i, p_flowData);
            }
        }

#if UNITY_EDITOR
        public override Vector2 Size => new Vector2(150, 85 + (OutputCount > 2 ? (OutputCount - 2) * 25 : 0));
        
        protected override void DrawCustomGUI(Rect p_rect)
        {
            base.DrawCustomGUI(p_rect);
        }
        
        protected override void InitializeAttributes()
        {
            OnConnectionRemoved -= OnConnectionRemovedHandler;
            OnConnectionRemoved += OnConnectionRemovedHandler;
            
            base.InitializeAttributes();
        }

        void OnConnectionRemovedHandler(NodeConnection p_connection)
        {
            if (Graph.Connections.Exists(nc =>
                nc.outputNode == p_connection.outputNode && nc.outputIndex == p_connection.outputIndex))
                return;
            
            var otherConnections = Graph.Connections.FindAll(nc => nc.outputNode == this && nc.outputIndex > p_connection.outputIndex);
            otherConnections.Sort((nc1, nc2) => nc1.outputIndex.CompareTo(nc2.outputIndex));
            otherConnections.ForEach(nc => nc.outputIndex--);
        }
#endif
    }
}