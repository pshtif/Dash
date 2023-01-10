/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System.Linq;
using Dash;
using OdinSerializer;
using OdinSerializer.Utilities;

[assembly: RegisterFormatterLocator(typeof(NodeFormatterLocator), -70)]

namespace Dash
{
    using System;

    internal class NodeFormatterLocator : IFormatterLocator
    {
        public bool TryGetFormatter(Type p_type, FormatterLocationStep step, ISerializationPolicy p_policy, bool p_allowWeakFallbackFormatters, out IFormatter p_formatter)
        {
            if (!typeof(NodeBase).IsAssignableFrom(p_type))
            {
                p_formatter = null;
                return false;
            }
            
            var modelType = p_type.GetBaseClasses().First().GetGenericArguments()[0];
            p_formatter = (IFormatter)Activator.CreateInstance(typeof(NodeFormatter<,>).MakeGenericType(p_type, modelType));
            return true;
        }
    }
}