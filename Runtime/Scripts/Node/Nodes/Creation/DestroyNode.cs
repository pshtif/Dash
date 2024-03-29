/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    [Attributes.Tooltip("Destroy target GameObject.")]
    [Category(NodeCategoryType.CREATION)]
    [OutputCount(1)]
    [InputCount(1)]
    public class DestroyNode : RetargetNodeBase<DestroyNodeModel>
    {
        protected override void ExecuteOnTarget(Transform p_target, NodeFlowData p_flowData)
        {
            if (p_target == null)
                return;

            if (Model.usePooling)
            {
                var poolId = GetParameterValue(Model.poolId, p_flowData);
                var pool = DashCore.Instance.GetPrefabPool(poolId);
                if (pool == null)
                {
                    SetError("Prefab pool with id " + Model.poolId + " not found target not destroyed.");
                }
                else
                {
                    pool.Return(p_target);
                }
            }
            else
            {
                if (p_target.gameObject != null)
                {
                    if (Model.immediate || !Application.isPlaying)
                    {
                        GameObject.DestroyImmediate(p_target.gameObject);
                    }
                    else
                    {
                        GameObject.Destroy(p_target.gameObject);
                    }
                }
            }

            OnExecuteEnd();
            OnExecuteOutput(0, p_flowData);
        }
    }
}