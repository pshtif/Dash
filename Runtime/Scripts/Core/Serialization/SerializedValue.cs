/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;

namespace Dash
{
    [Serializable]
    public class SerializedValue
    {
        public byte[] bytes;
        public List<UnityEngine.Object> references;

        public SerializedValue()
        {
            references = new List<UnityEngine.Object>();
        }
    }
}