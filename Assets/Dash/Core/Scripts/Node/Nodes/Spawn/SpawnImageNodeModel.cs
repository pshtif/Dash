/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEngine;

namespace Dash
{
    public class SpawnImageNodeModel : NodeModelBase
    {
        public Sprite sprite;

        public Parameter<Vector3> position = new Parameter<Vector3>(Vector3.zero);

        public bool setTargetAsParent = true;
        public bool setSpawnedAsTarget = true;
    }
}