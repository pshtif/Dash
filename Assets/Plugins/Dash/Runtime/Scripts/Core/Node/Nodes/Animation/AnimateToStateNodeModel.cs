/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    [Serializable]
    public class AnimateToStateNodeModel : AnimationNodeModelBase
    {
        [Order(11)]
        [TitledGroup("Properties")]
        public bool useFrom = false;
        
        [Order(12)]
        [Dependency("useFrom", true)]
        [TitledGroup("Properties")]
        [Button("Fetch", "Refetch", "Fetch")]
        public Parameter<TransformStorage> fromState = new Parameter<TransformStorage>(null);

        [Order(14)]
        [TitledGroup("Properties")]
        [Button("Fetch", "Refetch", "Fetch")]
        public Parameter<TransformStorage> toState = new Parameter<TransformStorage>(null);
        
        private TransformStorage Fetch()
        {
            return new TransformStorage();
        }
    }
}