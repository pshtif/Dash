/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash;
using OdinSerializer;

[assembly: RegisterFormatter(typeof(ParameterFormatter<>))]

namespace Dash
{
    public class ParameterFormatter<T> : MinimalBaseFormatter<Parameter<T>>
    {
        private static readonly Serializer<T> TSerializer = Serializer.Get<T>();
        private static readonly Serializer<string> StringSerializer = Serializer.Get<string>();
        private static readonly Serializer<bool> BoolSerializer = Serializer.Get<bool>();

        static ParameterFormatter()
        {
            // To avoid IL2CPP strip

            new ParameterFormatter<bool>();
        }
        
        public ParameterFormatter()
        {
        }
        
        protected override Parameter<T> GetUninitializedObject()
        {
            return new Parameter<T>(default(T));
        }
        
        protected override void Read(ref Parameter<T> p_value, IDataReader p_reader)
        {
            string name;

            p_value.isExpression = BoolSerializer.ReadValue(p_reader);
            p_value.expression = StringSerializer.ReadValue(p_reader);
            
            p_reader.PeekEntry(out name);
            if (name == "debug")
            {
                bool debug = BoolSerializer.ReadValue(p_reader);
                string debugName = StringSerializer.ReadValue(p_reader);
                string debugId = StringSerializer.ReadValue(p_reader);
                //p_value.SetDebug(debug, debugName, debugId);
            }
            
            p_value._value = TSerializer.ReadValue(p_reader);
        }
        
        protected override void Write(ref Parameter<T> p_value, IDataWriter p_writer)
        {
            BoolSerializer.WriteValue(p_value.isExpression, p_writer);
            
            StringSerializer.WriteValue(p_value.expression, p_writer);
            
            TSerializer.WriteValue(p_value._value, p_writer);
        }

        // protected override void DeserializeImplementation(ref Parameter<T> p_value, IDataReader p_reader)
        // {
        //     //Debug.Log("DeserializeParameter");
        //     string name;
        //
        //     p_value.isExpression = BoolSerializer.ReadValue(p_reader);
        //     p_value.expression = StringSerializer.ReadValue(p_reader);
        //
        //     p_reader.PeekEntry(out name);
        //     if (name == "debug")
        //     {
        //         bool debug = BoolSerializer.ReadValue(p_reader);
        //         string debugName = StringSerializer.ReadValue(p_reader);
        //         string debugId = StringSerializer.ReadValue(p_reader);
        //         //p_value.SetDebug(debug, debugName, debugId);
        //     }
        //
        //     p_value.value = TSerializer.ReadValue(p_reader);
        // }
	       //
        // protected override void SerializeImplementation(ref Parameter<T> p_value, IDataWriter p_writer)
        // {
        //     BoolSerializer.WriteValue(p_value.isExpression, p_writer);
        //     
        //     StringSerializer.WriteValue(p_value.expression, p_writer);
        //
        //     TSerializer.WriteValue(p_value.value, p_writer);
        // }
    }
}