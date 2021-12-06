/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System.Collections.Generic;
using Dash.Attributes;
using OdinSerializer;
using UnityEngine;

namespace Dash
{
    public class SubGraphNodeModel : NodeModelBase
    {
        public bool useAsset = false;
        
        [Dependency("useAsset", true)]
        public DashGraph graphAsset;
    }
}