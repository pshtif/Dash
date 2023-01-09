/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Runtime.Serialization;
using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    public class SpawnUIPrefabNodeModel : NodeModelBase
    {
        public Parameter<RectTransform> prefab = new Parameter<RectTransform>(null);

        public Parameter<Vector2> position = new Parameter<Vector2>(Vector2.zero);

        public Parameter<bool> setTargetAsParent = new Parameter<bool>(true);

        public Parameter<bool> retargetToSpawned = new Parameter<bool>(false);
        
        
        [Dependency("retargetToSpawned", false)]
        public bool createSpawnedAttribute = false;
        
        [Dependency("createSpawnedAttribute", true)]
        public string spawnedAttributeName = "image";
        
        [TitledGroup("Pooling")]
        public Parameter<bool> usePooling = new Parameter<bool>(false);

        [TitledGroup("Pooling")]
        [Dependency("usePoolingP", true)]
        public Parameter<string> poolId = new Parameter<string>("");
        
        [TitledGroup("Pooling")]
        [Dependency("usePoolingP", true)]
        public Parameter<bool> createPoolIdAttribute = new Parameter<bool>(true);
        
        [TitledGroup("Pooling")]
        [Dependency("usePoolingP", true)]
        [Dependency("createPoolIdAttribute", true)]
        public Parameter<string> poolIdAttributeName = new Parameter<string>("pool");
    }
}