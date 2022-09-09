/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    public class SpawnImageNodeModel : NodeModelBase
    {
        [TitledGroup("Pooling")] 
        public Parameter<bool> usePooling = new Parameter<bool>(false);
        
        [TitledGroup("Pooling")]
        [Dependency("usePooling", true)]
        public Parameter<string> poolId = new Parameter<string>("");
        
        [TitledGroup("Pooling")]
        [Dependency("usePooling", true)]
        public Parameter<bool> createPoolIdAttribute = new Parameter<bool>(true);

        [TitledGroup("Pooling")] 
        [Dependency("usePooling", true)]
        [Dependency("createPoolIdAttribute", true)]
        public Parameter<string> poolIdAttributeName = new Parameter<string>("pool");
        
        public string spawnedName = "image";
        
        public Parameter<Sprite> sprite = new Parameter<Sprite>(null);

        public Parameter<Vector2> position = new Parameter<Vector2>(Vector3.zero);
        
        public Parameter<bool> setNativeSize = new Parameter<bool>(false);
        public Parameter<bool> isMaskable = new Parameter<bool>(true);
        public Parameter<bool> isRaycastTarget = new Parameter<bool>(true);

        public bool setTargetAsParent = true;
        public bool retargetToSpawned = false;

        [Dependency("retargetToSpawned", false)]
        public bool createSpawnedAttribute = false;
        
        [Dependency("createSpawnedAttribute", true)]
        public string spawnedAttributeName = "image";
    }
}