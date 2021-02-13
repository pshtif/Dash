/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEngine;

namespace Dash
{
    public class SpawnImageNodeModel : NodeModelBase
    {
        public Parameter<Sprite> sprite = new Parameter<Sprite>(null);

        public Parameter<Vector2> position = new Parameter<Vector2>(Vector3.zero);

        public bool setTargetAsParent = true;
        public bool retargetToSpawned = true;
    }
}