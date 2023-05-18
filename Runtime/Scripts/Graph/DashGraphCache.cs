/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System.Collections.Generic;
using System.Linq;
using OdinSerializer;
using UnityEngine;

namespace Dash
{
    public class DashGraphCache
    {
        private static Dictionary<DashGraph, SerializedValue> _serializedGraphs =
            new Dictionary<DashGraph, SerializedValue>();
        
        public static DashGraph GetInstance(DashGraph p_graphAsset)
        {
            if (!_serializedGraphs.ContainsKey(p_graphAsset))
            {
                _serializedGraphs.Add(p_graphAsset, SerializeGraph(p_graphAsset));
            }
            
            return DeserializeGraph(_serializedGraphs[p_graphAsset]);
        }
        
        public static SerializedValue SerializeGraph(DashGraph p_graph, DataFormat p_format = DataFormat.Binary)
        {
            SerializedValue serialized = new SerializedValue();
            serialized.bytes = p_graph.SerializeToBytes(p_format, ref serialized.references);

            return serialized;
        }

        public static DashGraph DeserializeGraph(SerializedValue p_value)
        {
            DashGraph graph = ScriptableObject.CreateInstance<DashGraph>();
            //Debug.Log("DeserializeInfo: "+p_value.references.Count+" : "+graph.name);
            if (p_value.references.Count > 0) p_value.references[0] = graph;
            
            graph.DeserializeFromBytes(p_value.bytes, DataFormat.Binary, ref p_value.references);
            return graph;
        }

        public static void ReleaseCache()
        {
            _serializedGraphs = new Dictionary<DashGraph, SerializedValue>();
        }
    }
}