/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;

namespace Dash
{
    [Tooltip("Branches execution based on boolean expression.")]
    [Category(NodeCategoryType.LOGIC)]
    [OutputCount(2)]
    [InputCount(1)]
    [OutputLabels("True", "False")]
    public class BranchNode : NodeBase<BranchNodeModel>
    {
        override protected void OnExecuteStart(NodeFlowData p_flowData)
        {
            if (GetParameterValue(Model.expression, p_flowData))
            {
                OnExecuteEnd();
                OnExecuteOutput(0,p_flowData);
            }
            else
            {
                OnExecuteEnd();
                OnExecuteOutput(1,p_flowData);
            }
        }
    }
}