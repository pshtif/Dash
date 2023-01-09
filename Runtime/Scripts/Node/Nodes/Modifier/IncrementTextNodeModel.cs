/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Runtime.Serialization;
using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    public class IncrementTextNodeModel : RetargetNodeModel
    {
        public bool useDotFormating = true;

        [HideInInspector]
        public Parameter<int> incrementP = new Parameter<int>(0);

        public Parameter<int> increment = new Parameter<int>(0);

        [OnDeserialized]
        void OnDeserialized()
        {
#pragma warning disable 612, 618
            
            ParameterUtils.MigrateParameter(ref incrementP, ref increment);

#pragma warning restore 612, 618 
        }
    }
}