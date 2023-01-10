/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash;
using OdinSerializer;
using UnityEngine;

[assembly: RegisterFormatter(typeof(NodeConnectionFormatter))]

namespace Dash
{
    public class NodeConnectionFormatter : BaseFormatter<NodeConnection>
    {
        private static readonly Serializer<NodeBase> NodeSerializer = Serializer.Get<NodeBase>();
        private static readonly Serializer<bool> BoolSerializer = Serializer.Get<bool>();
        private static readonly Serializer<int> IntSerializer = Serializer.Get<int>();

        static NodeConnectionFormatter()
        {
            // To avoid IL2CPP strip

            //new NodeFormatter<DebugNodeModel>();
        }
        
        public NodeConnectionFormatter()
        {
        }

        protected override void DeserializeImplementation(ref NodeConnection p_value, IDataReader p_reader)
        {
            string name;

            p_value.active = BoolSerializer.ReadValue(p_reader);
            p_value.inputIndex = IntSerializer.ReadValue(p_reader);
            // <outputIndex>k__BackingField
            p_value.outputIndex = IntSerializer.ReadValue(p_reader);
            // <inputNode>k__BackingField
            p_value.inputNode = NodeSerializer.ReadValue(p_reader);
            // <outputNode>k__BackingField
            p_value.outputNode = NodeSerializer.ReadValue(p_reader);
            // Debug.Log(p_value.inputNode);
            // Debug.Log(p_value.outputNode);
            //
            // Debug.Log(p_reader.PeekEntry(out name));
            // Debug.Log(name);
        }
        
        protected override void SerializeImplementation(ref NodeConnection p_value, IDataWriter p_writer)
        {
            BoolSerializer.WriteValue(p_value.active, p_writer);

            IntSerializer.WriteValue(p_value.inputIndex, p_writer);
            IntSerializer.WriteValue(p_value.outputIndex, p_writer);
            
            NodeSerializer.WriteValue(p_value.inputNode, p_writer);
            NodeSerializer.WriteValue(p_value.outputNode, p_writer);
        }
    }
}