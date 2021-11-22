/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System.Reflection;

namespace Dash
{
    public static class MethodInfoExtensions
    {
        public static bool IsSetAccessor(this MethodInfo p_methodInfo, bool p_includeInheritance = true)
        {
            var type = p_methodInfo.ReflectedType;
            do
            {
                var properties = type.GetProperties();
                foreach (var property in properties)
                {
                    if (property.GetSetMethod() == p_methodInfo)
                        return true;
                }

                type = type.BaseType;
            } while (type != null || !p_includeInheritance);

            return false;
        }
        
        public static bool IsGetAccessor(this MethodInfo p_methodInfo, bool p_includeInheritance = true)
        {
            var type = p_methodInfo.ReflectedType;
            do
            {
                var properties = type.GetProperties();
                foreach (var property in properties)
                {
                    if (property.GetGetMethod() == p_methodInfo)
                        return true;
                }

                type = type.BaseType;
            } while (type != null || !p_includeInheritance);

            return false;
        }
    }
}