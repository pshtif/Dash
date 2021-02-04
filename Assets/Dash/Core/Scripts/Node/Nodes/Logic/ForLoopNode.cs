/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using Dash.Attributes;

namespace Dash
{
    [Category(NodeCategoryType.LOGIC)]
    [OutputCount(2)]
    [InputCount(1)]
    [OutputLabels("OnItem", "OnFinished")]
    public class ForLoopNode : NodeBase<ForLoopNodeModel>
    {
        protected override void OnExecuteStart(NodeFlowData p_flowData)
        {
            int length = Model.lastIndex - Model.firstIndex;
            if (length == 0)
                EndLoop(p_flowData);

            for (int i = Model.firstIndex; i != Model.lastIndex; i += Math.Abs(length) / length)
            {
                p_flowData.SetAttribute(Model.indexVariable, i);
                OnExecuteOutput(0, p_flowData);
            }
            
            OnExecuteEnd();
            OnExecuteOutput(1, p_flowData);
        }

        void EndLoop(NodeFlowData p_flowData)
        {
            OnExecuteEnd();
            OnExecuteOutput(1, p_flowData);
        }
    }
}