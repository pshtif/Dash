/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using Dash.Attributes;
using UnityEditor;
using UnityEngine;

namespace Dash
{
    [Help("Restore targets and children states.")]
    [Category(NodeCategoryType.OTHER)]
    [OutputCount(1)]
    [InputCount(1)]
    [Size(120,85)]
    [Serializable]
    [Experimental]
    public class RestoreStateNode : RetargetNodeBase<RestoreStateNodeModel>
    {
        protected override void ExecuteOnTarget(Transform p_target, NodeFlowData p_flowData)
        {
            Dictionary<Transform, TransformStorageData> storage = null;

            switch (Model.storageType)
            {
                case StorageType.VARIABLE:
                    if (Graph.variables.HasVariable(Model.storageName))
                    {
                        storage = Graph.variables
                            .GetVariable<Dictionary<Transform, TransformStorageData>>(Model.storageName)
                            .value;
                    }
                    else
                    {
                        SetError("Storage variable "+Model.storageName+" not found.");
                        return;
                    }
                    break;
                case StorageType.ATTRIBUTE:
                    storage = p_flowData.GetAttribute(Model.storageName) as Dictionary<Transform, TransformStorageData>;
                    if (storage == null)
                    {
                        SetError("Storage attribute "+Model.storageName+" not found.");
                        return;
                    }
                    break;
            }

            foreach (var pair in storage)
            {
                pair.Value.Restore(pair.Key);
            }
            
            OnExecuteEnd();
            OnExecuteOutput(0,p_flowData);
        }
        
#if UNITY_EDITOR
        protected override void DrawCustomGUI(Rect p_rect)
        {
            GUI.DrawTexture(new Rect(p_rect.x + p_rect.width / 2 - 24, p_rect.y + 28, 48, 48),
                IconManager.GetIcon("RollIn_Icon"));
        }
#endif
    }
}