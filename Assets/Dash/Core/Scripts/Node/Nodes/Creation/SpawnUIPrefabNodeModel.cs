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
        public bool retargetToSpawned = true;
    }
}