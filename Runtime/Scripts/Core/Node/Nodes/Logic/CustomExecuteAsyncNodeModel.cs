/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using Dash.Attributes;
using OdinSerializer.Utilities;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Dash
{
    
    public class CustomExecuteAsyncNodeModel : NodeModelBase
    {
        [ClassPopup(typeof(ICustomExecuteAsync), true)]
        public ICustomExecuteAsync executeClass;
    }
}