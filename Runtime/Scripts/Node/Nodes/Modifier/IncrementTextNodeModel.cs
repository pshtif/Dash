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
        
        [Obsolete]
        [HideInInspector]
        public int increment = 1;

        [Label("increment")]
        public Parameter<int> incrementP = new Parameter<int>(0);

        [OnDeserialized]
        void OnDeserialized()
        {
#pragma warning disable 612, 618
            if (incrementP == null)
            {
                incrementP = new Parameter<int>(increment);
            }
#pragma warning restore 612, 618 
        }
    }
}