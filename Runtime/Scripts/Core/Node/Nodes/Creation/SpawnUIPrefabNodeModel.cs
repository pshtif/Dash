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
        [HideInInspector]
        public RectTransform prefab;

        [Label("prefab")]
        public Parameter<RectTransform> prefabP = new Parameter<RectTransform>(null);

        public Parameter<Vector2> position = new Parameter<Vector2>(Vector2.zero);

        [HideInInspector]
        public bool setTargetAsParent = true;

        [Label("setTargetAsParent")]
        public Parameter<bool> setTargetAsParentP = new Parameter<bool>(true);

        [HideInInspector]
        public bool retargetToSpawned = false;
        
        [Label("retargetToSpawned")]
        public Parameter<bool> retargetToSpawnedP = new Parameter<bool>(false);
        
        [Dependency("retargetToSpawned", false)]
        public bool createSpawnedAttribute = false;
        
        [Dependency("createSpawnedAttribute", true)]
        public string spawnedAttributeName = "image";
        
        [HideInInspector]
        [TitledGroup("Pooling")]
        public bool usePooling = false;

        [TitledGroup("Pooling")] 
        [Label("usePooling")]
        public Parameter<bool> usePoolingP = new Parameter<bool>(false);
        
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
        
        [OnDeserialized]
        void OnDeserialized()
        {
#pragma warning disable 612, 618
            if (prefabP == null)
            {
                prefabP = new Parameter<RectTransform>(prefab);
            }
            
            if (setTargetAsParentP == null)
            {
                setTargetAsParentP = new Parameter<bool>(setTargetAsParent);
            }
            
            if (retargetToSpawnedP == null)
            {
                retargetToSpawnedP = new Parameter<bool>(retargetToSpawned);
            }
            
            if (usePoolingP == null)
            {
                usePoolingP = new Parameter<bool>(usePooling);
            }
#pragma warning restore 612, 618 
        }
    }
}