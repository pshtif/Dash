/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;

namespace Dash
{
    public static class TypeExtensions
    {
        public static object GetDefaultValue(this Type t)
        {
            return (t.IsValueType && Nullable.GetUnderlyingType(t) == null) ?
                Activator.CreateInstance(t) :
                null;
        }
        
        public static string GetReadableTypeName(this Type p_type)
        {
            string typeName = p_type.Name;
            if (p_type.IsGenericType)
            {
                typeName = typeName.Substring(0, typeName.Length - 2) + "<";
                foreach (var type in p_type.GetGenericArguments())
                {
                    typeName += GetReadableTypeName(type) + ",";
                }

                typeName = typeName.Substring(0, typeName.Length - 1) + ">";
            }

            return typeName;
        }
    }
}