/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Linq;
using System.Net;
using System.Reflection;
using Dash.Attributes;
using OdinSerializer;
using UnityEngine;

namespace Dash
{
    public class SetActiveNodeModel : RetargetNodeModelBase
    {
        [PreviouslySerializedAs("active")]
        [HideInInspector]
        public bool oldActive = true;
        
        [InitializeFrom("oldActive")]
        public Parameter<bool> active = new Parameter<bool>(true);
    }
}