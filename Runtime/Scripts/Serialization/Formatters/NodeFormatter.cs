/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using OdinSerializer;
using UnityEngine;

namespace Dash
{
    public class NodeFormatter<T,K> : MinimalBaseFormatter<T> where T : NodeBase<K> where K : NodeModelBase, new()
    {
        private static readonly Serializer<K> KSerializer = Serializer.Get<K>();
        private static readonly Serializer<string> StringSerializer = Serializer.Get<string>();
        private static readonly Serializer<bool> BoolSerializer = Serializer.Get<bool>();
        private static readonly Serializer<Rect> RectSerializer = Serializer.Get<Rect>();
        private static readonly Serializer<DashGraph> GraphSerialized = Serializer.Get<DashGraph>();

        public NodeFormatter()
        {
        }
        
        protected override T GetUninitializedObject()
        {
            return null;
        }
        
        protected override void Read(ref T p_value, IDataReader p_reader)
        {
            string name;

            p_value = (T)Activator.CreateInstance(typeof(T));
            p_value._model = KSerializer.ReadValue(p_reader);
            
            int index;
            p_reader.ReadExternalReference(out index);
            p_value._graph = (DashGraph)p_reader.Context.GetExternalObject(index);

            while (p_reader.PeekEntry(out name) != EntryType.EndOfNode)
            {
                switch (name)
                {
                    case "rect":
                        #if UNITY_EDITOR
                        p_value.rect = RectSerializer.ReadValue(p_reader);
                        #else
                        p_reader.SkipEntry();
                        #endif
                        break;
                    case "<ExecutionCount>k__BackingField":
                    case "<Category>k__BackingField":
                    case "_bindFrom":
                    case "_bindTo":
                    default:
                        p_reader.SkipEntry();
                        break;
                }
            }
        }
        
        protected override void Write(ref T p_value, IDataWriter p_writer)
        {
            KSerializer.WriteValue((K)p_value._model, p_writer);
            
            int index;
            if (p_writer.Context.TryRegisterExternalReference(p_value._graph, out index))
            {
                p_writer.WriteExternalReference("_graph", index);
            }
            
            #if UNITY_EDITOR
            RectSerializer.WriteValue("rect", p_value.rect, p_writer);
            #endif
        }
    }
}