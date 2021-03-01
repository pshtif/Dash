/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;

namespace Dash
{
    [Help("Branches execution based on boolean expression.")]
    [Category(NodeCategoryType.LOGIC)]
    [OutputCount(2)]
    [InputCount(1)]
    [OutputLabels("True", "False")]
    public class BranchNode : NodeBase<BranchNodeModel>
    {
        override protected void OnExecuteStart(NodeFlowData p_flowData)
        {
            if (Model.expression.GetValue(ParameterResolver, p_flowData))
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