/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    public class SpawnImageNodeModel : NodeModelBase
    {
        public string spawnedName = "image";
        
        public Parameter<Sprite> sprite = new Parameter<Sprite>(null);

        public Parameter<Vector2> position = new Parameter<Vector2>(Vector3.zero);

        public bool setTargetAsParent = true;
        public bool retargetToSpawned = false;

        [Dependency("retargetToSpawned", false)]
        public bool createSpawnedAttribute = false;
        
        [Dependency("createSpawnedAttribute", true)]
        public string spawnedAttributeName = "image";
    }
}