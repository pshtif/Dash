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
using UnityEngine.Serialization;

namespace Dash
{
    public class SetActiveNodeModel : RetargetNodeModelBase
    {
        public Parameter<bool> active = new Parameter<bool>(true);
    }
}