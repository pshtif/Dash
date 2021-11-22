/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OdinSerializer.Utilities;
using UnityEngine;

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

        public static Type GetTypeByName(string p_typeName)
        {
            if (_typeCacheDictionary == null)
                GetAllTypes();

            return _typeCacheDictionary[p_typeName];
        }
        
        private static readonly object _AssemblyLock = new object();
        public static string GetTypeNameWithoutAssembly(string p_type)
        {
            return p_type.Substring(p_type.LastIndexOf('.')+1);
        }

        
        private static Type[] _typeCacheList;
        private static Dictionary<string, Type> _typeCacheDictionary;
        private static Assembly[] _assemblyCache;

        private static Assembly[] GetAllAssemblies()
        {
            return _assemblyCache != null ? _assemblyCache : _assemblyCache = AppDomain.CurrentDomain.GetAssemblies();   
        } 
        
        public static Type[] GetAllTypes() {
            if ( _typeCacheList != null ) {
                return _typeCacheList;
            }

            var assemblies = GetAllAssemblies();

            var result = new List<Type>();

            assemblies.Where(a => !a.IsDynamic).ForEach(a => result.AddRange(a.GetExportedTypes()));

            _typeCacheDictionary = new Dictionary<string, Type>();
            foreach (var type in result)
                if (!_typeCacheDictionary.ContainsKey(type.Name))
                    _typeCacheDictionary.Add(type.Name, type);
            
            return _typeCacheList = result.OrderBy(t => t.Namespace).ThenBy(t => t.Name).ToArray();
        }
    }
}