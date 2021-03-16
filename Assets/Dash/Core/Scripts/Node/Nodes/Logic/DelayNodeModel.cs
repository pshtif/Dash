/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using UnityEngine;

namespace Dash
{
    [Serializable]
    public class DelayNodeModel : NodeModelBase
    {
        [SerializeField] public Parameter<float> time = new Parameter<float>(0);
    }
}