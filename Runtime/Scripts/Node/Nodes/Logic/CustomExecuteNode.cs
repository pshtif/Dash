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
    public class CustomExecuteNode : NodeBase<CustomExecuteNodeModel>
    {
        protected override void OnExecuteStart(NodeFlowData p_flowData)
        {
            Model.executeClass.Execute(p_flowData, () => OnCustomExecuteEnd(p_flowData));
        }
        
        void OnCustomExecuteEnd(NodeFlowData p_flowData) 
        {
            OnExecuteEnd();
            OnExecuteOutput(0, p_flowData);
        }
    }
}