/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    [Help("Store targets and children states.")]
    [Category(NodeCategoryType.OTHER)]
    [OutputCount(1)]
    [InputCount(1)]
    [Size(120,85)]
    [Serializable]
    [Experimental]
    public class StoreStateNode : RetargetNodeBase<StoreStateNodeModel>
    {
        protected override void ExecuteOnTarget(Transform p_target, NodeFlowData p_flowData)
        {
            bool includeTarget = GetParameterValue(Model.includeTarget, p_flowData);
            
            Dictionary<Transform, TransformStorageData> storage = new Dictionary<Transform, TransformStorageData>();
            if (includeTarget)
            {
                storage.Add(p_target, new TransformStorageData(p_target.transform, GetStorageFlag()));
            }

            StoreChildren(p_target, storage);

            switch (Model.storageType)
            {
                case StorageType.ATTRIBUTE:
                    p_flowData.SetAttribute(Model.storageName, storage);
                    break;
                case StorageType.VARIABLE:
                    if (Graph.variables.HasVariable(Model.storageName))
                    {
                        var variable = Graph.variables.GetVariable(Model.storageName);
                        if (variable.GetVariableType() != typeof(Dictionary<Transform, TransformStorageData>))
                        {
                            SetError("Cannot store data to existing variable of incompatible type " +
                                     variable.GetVariableType());
                            return;
                        }

                        variable.value = storage;
                    }
                    else
                    {
                        Graph.variables.AddVariable(Model.storageName, storage);
                    }
                    break;
            }

            OnExecuteEnd();
            OnExecuteOutput(0,p_flowData);
        }

        protected TransformStorageOption GetStorageFlag()
        {
            TransformStorageOption flag = 0;
            
            if (Model.storeActive)
                FlagsUtils.Set(ref flag, TransformStorageOption.ACTIVE);
                
            if (Model.storePosition)
                FlagsUtils.Set(ref flag, TransformStorageOption.POSITION);

            return flag;
        }

        protected void StoreChildren(Transform p_parent, Dictionary<Transform, TransformStorageData> p_storage)
        {
            foreach (Transform child in p_parent)
            {
                p_storage.Add(child, new TransformStorageData(child.transform, GetStorageFlag()));
                
                StoreChildren(child, p_storage);
            }
        }
        
#if UNITY_EDITOR
        protected override void DrawCustomGUI(Rect p_rect)
        {
            GUI.DrawTexture(new Rect(p_rect.x + p_rect.width / 2 - 24, p_rect.y + 28, 48, 48),
                IconManager.GetIcon("RollOut_Icon"));
        }
#endif
    }
}