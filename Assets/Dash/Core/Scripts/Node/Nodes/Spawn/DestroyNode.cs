/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    [Category(NodeCategoryType.SPAWN)]
    [OutputCount(1)]
    [InputCount(1)]
    public class DestroyNode : RetargetNodeBase<DestroyNodeModel>
    {
        protected override void ExecuteOnTarget(Transform p_target, NodeFlowData p_flowData)
        {
            if (p_target == null)
                return;

            if (Model.immediate || !Application.isPlaying)
            {
                GameObject.DestroyImmediate(p_target.gameObject);
            }
            else
            {
                GameObject.Destroy(p_target.gameObject);
            }
            
            OnExecuteEnd();
            OnExecuteOutput(0, p_flowData);
        }
    }
}