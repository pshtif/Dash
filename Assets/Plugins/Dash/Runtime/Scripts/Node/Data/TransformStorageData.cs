/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using UnityEngine;

namespace Dash
{
    [Flags]
    public enum TransformStorageOption
    {
        ACTIVE = 1,
        POSITION = 2,
    }
    
    public class TransformStorageData
    {
        public TransformStorageOption storageOptionOptionFlag;
        public bool active;
        public Vector3 position;

        public TransformStorageData(Transform p_transform, TransformStorageOption p_storageOptionFlag)
        {
            storageOptionOptionFlag = p_storageOptionFlag;

            if (storageOptionOptionFlag.HasFlag(TransformStorageOption.ACTIVE))
            {
                active = p_transform.gameObject.activeSelf;
            }

            if (storageOptionOptionFlag.HasFlag(TransformStorageOption.POSITION))
            {
                position = p_transform.position;
            }
                
        }

        public void Restore(Transform p_transform)
        {
            if (FlagsUtils.IsSet(storageOptionOptionFlag, TransformStorageOption.ACTIVE))
            {
                p_transform.gameObject.SetActive(active);
            }

            if (FlagsUtils.IsSet(storageOptionOptionFlag, TransformStorageOption.POSITION))
            {
                p_transform.position = position;   
            }
        }
    }
}