/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    public class SpawnUIPrefabNodeModel : NodeModelBase
    {
        public RectTransform prefab;

        public Parameter<Vector2> position = new Parameter<Vector2>(Vector2.zero);

        public bool setTargetAsParent = true;
        public bool retargetToSpawned = false;
        
        [Dependency("retargetToSpawned", false)]
        public bool createSpawnedAttribute = false;
        
        [Dependency("createSpawnedAttribute", true)]
        public string spawnedAttributeName = "image";
    }
}