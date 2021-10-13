/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;
using OdinSerializer.Utilities;

namespace Dash
{
    [Help("End an event in sequencer.")]
    [Category(NodeCategoryType.EVENT)]
    [InputCount(1)]
    [OutputCount(1)]
    [Size(170,85)]
    public class EndEventNode : NodeBase<EndEventNodeModel>
    {
        protected override void OnExecuteStart(NodeFlowData p_flowData)
        {
            string eventName = GetParameterValue(Model.eventName, p_flowData);
            string sequencerId = GetParameterValue(Model.sequencerId, p_flowData);

            if (!sequencerId.IsNullOrWhitespace())
            {
                DashRuntimeCore.Instance.GetOrCreateSequencer(sequencerId).EndEvent(eventName);
            }
            
            OnExecuteEnd();
            OnExecuteOutput(0, p_flowData);
        }
    }
}