/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System.Collections.Generic;
using UnityEngine;

namespace Dash
{
    public class PrefabPool
    {
        private Stack<Transform> _pool;
        private Transform _prefab;

        public PrefabPool(Transform p_prefab)
        {
            _pool = new Stack<Transform>();
            _prefab = p_prefab;
        }
        
        public Transform Get()
        {
            var instance = _pool.Count > 0 ? _pool.Pop() : GameObject.Instantiate(_prefab);
            instance.gameObject.SetActive(true);
            return instance;
        }

        public void Return(Transform p_instance)
        {
            p_instance.gameObject.SetActive(false);
            _pool.Push(p_instance);
        }

        public void Clean()
        {
            while (_pool.Count > 0)
            {
                GameObject.DestroyImmediate(_pool.Pop().gameObject);
            }
        }
    }
}