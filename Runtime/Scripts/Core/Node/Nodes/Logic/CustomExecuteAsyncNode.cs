/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;

namespace Dash
{
    [Experimental]
    [Tooltip("Call a method using reflection on any component on DashController GameObject")]
    [Category(NodeCategoryType.LOGIC)]
    [OutputCount(1)]
    [InputCount(1)]
    public class CustomExecuteAsyncNode : NodeBase<CustomExecuteAsyncNodeModel>
    {
        protected async override void OnExecuteStart(NodeFlowData p_flowData)
        {
            var task = Model.executeClass.Execute(p_flowData);

            await task;

            OnExecuteEnd();
            OnExecuteOutput(0, p_flowData);
        }
    }
}