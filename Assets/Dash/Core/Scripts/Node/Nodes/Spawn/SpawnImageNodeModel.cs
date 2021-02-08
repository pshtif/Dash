/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEngine;

namespace Dash
{
    public class SpawnImageNodeModel : NodeModelBase
    {
        public Sprite sprite;

        public Parameter<Vector2> position = new Parameter<Vector2>(Vector3.zero);

        public bool setTargetAsParent = true;
        public bool retargetToSpawned = true;
    }
}