/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using OdinSerializer;
using Object = UnityEngine.Object;

namespace Dash
{
    [Serializable]
    public class SerializedValue
    {
        public DataFormat format = DataFormat.Binary;
        public byte[] bytes;
        public List<UnityEngine.Object> references = new List<Object>();
    }
}