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
    }
}