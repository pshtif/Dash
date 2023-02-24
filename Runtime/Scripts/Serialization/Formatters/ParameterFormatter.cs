/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash;
using OdinSerializer;

#if DASH_FORMATTERS
[assembly: RegisterFormatter(typeof(ParameterFormatter<>))]
#endif

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

            while (p_reader.PeekEntry(out name) != EntryType.EndOfNode)
            {
                switch (name)
                {
                    case "isExpression":
                        p_value.isExpression = BoolSerializer.ReadValue(p_reader);
                        break;
                    case "expression":
                        p_value.expression = StringSerializer.ReadValue(p_reader);
                        break;
                    case "_value":
                        p_value._value = TSerializer.ReadValue(p_reader);
                        break;
                    default:
                        // To avoid old serialized data
                        p_reader.SkipEntry();
                        break;
                }
            }
            // bool debug = BoolSerializer.ReadValue(p_reader);
            // string debugName = StringSerializer.ReadValue(p_reader);
            // string debugId = StringSerializer.ReadValue(p_reader);
            //p_value.SetDebug(debug, debugName, debugId);
        }
        
        protected override void Write(ref Parameter<T> p_value, IDataWriter p_writer)
        {
            BoolSerializer.WriteValue(p_value.isExpression, p_writer);
            
            StringSerializer.WriteValue(p_value.expression, p_writer);
            
            TSerializer.WriteValue(p_value._value, p_writer);
        }
    }
}