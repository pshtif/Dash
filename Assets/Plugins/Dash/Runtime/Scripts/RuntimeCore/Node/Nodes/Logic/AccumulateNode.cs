/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using Dash.Accumulate;
using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    [Experimental]
    [Help("Accumulates inputs and executes after set of conditions.")]
    [Category(NodeCategoryType.LOGIC)]
    [OutputCount(1)]
    public class AccumulateNode : NodeBase<AccumulateNodeModel>
    {
        private int _currentInputMask = 0;
        private int _executedInputsCount = 0;
        private int _executedUniqueCount = 0;
        private bool _accumulated = false;

        protected override void InitializeAttributes()
        {
            ((INodeAccess) this).OnConnectionRemoved -= OnConnectionRemovedHandler;
            ((INodeAccess) this).OnConnectionRemoved += OnConnectionRemovedHandler;
            
            base.InitializeAttributes();
        }

        void OnConnectionRemovedHandler(NodeConnection p_connection)
        {
            if (Graph.Connections.Exists(nc =>
                nc.inputNode == p_connection.inputNode && nc.inputIndex == p_connection.inputIndex))
                return;
            
            var otherConnections = Graph.Connections.FindAll(nc => nc.inputNode == this && nc.inputIndex > p_connection.inputIndex);
            otherConnections.Sort((nc1, nc2) => nc1.inputIndex.CompareTo(nc2.inputIndex));
            otherConnections.ForEach(nc => nc.inputIndex--);
        }
        
        public override int InputCount
        {
            get
            {
                var connected = Graph.Connections.FindAll(nc => this == nc.inputNode);
                if (connected.Count == 0)
                    return 1;

                int currentInputCount = 1;
                connected.ForEach(nc =>
                    currentInputCount = nc.inputIndex > currentInputCount - 1 ? nc.inputIndex + 1 : currentInputCount);
                
                return currentInputCount+1;
            }
        }

        private int GetInputMask()
        {
            return (1 << InputCount - 1) - 1;
        }
        
        protected override void Initialize()
        {
            
        }

        protected override void OnExecuteStart(NodeFlowData p_flowData)
        {
            int powerindex = 1 << p_flowData.inputIndex;
            if ((_currentInputMask & powerindex) != powerindex)
            {
                _currentInputMask += powerindex;
                _executedUniqueCount++;
            }

            _executedInputsCount++;

            OnExecuteEnd();

            if (_accumulated && !GetParameterValue(Model.repeatAccumulation, p_flowData))
                return;
            
            _accumulated = false;
            int accumulationCount = GetParameterValue(Model.accumulationCount, p_flowData);
            int mask = GetInputMask();
            switch (Model.accumulationType)
            {
                case AccumulateType.ALL:
                    if ((_currentInputMask & mask) == mask)
                    {
                        _accumulated = true;
                    }
                    break;
                case AccumulateType.ANY:
                    _accumulated = true;
                    break;
                case AccumulateType.ANY_COUNT:
                    if (_executedInputsCount >= accumulationCount)
                    {
                        _accumulated = true;
                    }
                    break;
                case AccumulateType.UNIQUE_COUNT:
                    if (_executedUniqueCount >= accumulationCount)
                    {
                        _accumulated = true;
                    }
                    break;
            }
            
            if (_accumulated)
            {
                _currentInputMask = _executedInputsCount = _executedUniqueCount = 0;
                OnExecuteOutput(0, p_flowData);
            }
        }

#if UNITY_EDITOR
        public override Vector2 Size => new Vector2(150, 85 + (InputCount > 2 ? (InputCount - 2) * 25 : 0));
        
        protected override void DrawCustomGUI(Rect p_rect)
        {
            base.DrawCustomGUI(p_rect);
            Rect offsetRect = new Rect(rect.x + _graph.viewOffset.x, rect.y + _graph.viewOffset.y, rect.width,
                rect.height);

            GUI.Label(
                new Rect(new Vector2(offsetRect.x + offsetRect.width * .5f - 50, offsetRect.y + offsetRect.height - 52),
                    new Vector2(100, 20)), Model.accumulationType.ToString(), DashEditorCore.Skin.GetStyle("NodeText"));

            if (Model.accumulationType == AccumulateType.ANY_COUNT ||
                Model.accumulationType == AccumulateType.UNIQUE_COUNT)
            {
                GUI.color = Model.accumulationCount.isExpression ? Color.cyan : Color.white;
                GUI.Label(
                    new Rect(
                        new Vector2(offsetRect.x + offsetRect.width * .5f - 50, offsetRect.y + offsetRect.height - 32),
                        new Vector2(100, 20)),
                    Model.accumulationCount.isExpression ? "[EXP]" : Model.accumulationCount.GetValue(null).ToString(),
                    DashEditorCore.Skin.GetStyle("NodeText"));
                GUI.color = Color.white;
            }
        }
#endif
    }
}