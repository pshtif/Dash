/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dash
{
    static public class ReflectionUtils
    {
        public static bool IsConstant(this FieldInfo field) {
            return field.IsReadOnly() && field.IsStatic;
        }
        
        public static bool IsStatic(this EventInfo info) {
            var m = info.GetAddMethod();
            return m != null ? m.IsStatic : false;
        }
        
        public static bool IsReadOnly(this FieldInfo field) {
            return field.IsInitOnly || field.IsLiteral;
        }

        public static bool IsStatic(this PropertyInfo info) {
            var m = info.GetGetMethod();
            return m != null ? m.IsStatic : false;
        }
        
        public static List<Type> GetAllTypes(Type p_type)
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                .Where(x => p_type.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract).ToList();
        }
        
        public static Type GetType(string p_typeName)
        {
            var type = Type.GetType(p_typeName);
            
            if(type != null)
                return type;

            var assemblyName = p_typeName.Substring(0, p_typeName.LastIndexOf('.'));
            
            var assembly = Assembly.LoadWithPartialName(assemblyName);
            if( assembly == null )
                return null;
            
            return assembly.GetType(p_typeName);
        }

        public static string GetTypeNameWithoutAssembly(string p_type)
        {
            return p_type.Substring(p_type.LastIndexOf('.')+1);
        }
    }
}