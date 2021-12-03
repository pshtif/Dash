/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dash
{
    [Serializable]
    public class TransformStorage
    {
        private Dictionary<Transform, TransformStorageData> _storage;
        private Transform _rootTransform;
        private TransformStorageOption _storageOptions;

        public TransformStorage()
        {
            _storage = new Dictionary<Transform, TransformStorageData>();
        }

        public void Store(Transform p_transform, TransformStorageOption p_storageOptions, bool p_includeTransform = true)
        {
            _storage?.Clear();
            
            _rootTransform = p_transform;
            _storageOptions = p_storageOptions;

            if (p_includeTransform)
            {
                _storage.Add(p_transform, new TransformStorageData(_rootTransform, _storageOptions));
            }
            
            StoreChildren(_rootTransform);
        }
        
        protected void StoreChildren(Transform p_parent)
        {
            foreach (Transform child in p_parent)
            {
                _storage.Add(child, new TransformStorageData(child.transform, _storageOptions));
                
                StoreChildren(child);
            }
        }
    }
}