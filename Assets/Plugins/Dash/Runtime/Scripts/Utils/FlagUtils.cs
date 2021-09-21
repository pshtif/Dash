/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;

public class FlagsUtils
{
    public static bool IsSet<T>(T p_flags, T p_flag) where T : struct
    {
        int flagsValue = (int)(object)p_flags;
        int flagValue = (int)(object)p_flag;

        return (flagsValue & flagValue) != 0;
    }
    
    public static void Set<T>(ref T p_flags, T p_flag) where T : struct
    {
        int flagsValue = (int)(object)p_flags;
        int flagValue = (int)(object)p_flag;

        p_flags = (T)(object)(flagsValue | flagValue);
    }

    public static void Unset<T>(ref T p_flags, T p_flag) where T : struct
    {
        int flagsValue = (int)(object)p_flags;
        int flagValue = (int)(object)p_flag;

        p_flags = (T)(object)(flagsValue & (~flagValue));
    }

    private static Dictionary<Type, Array> _cache;
    
    public static bool HasAny<T>(T p_flags, T p_flag) where T : struct
    {
        if (_cache == null)
            _cache = new Dictionary<Type, Array>();

        var type = p_flag.GetType();
        Array items;
        if (!_cache.ContainsKey(type))
        {
            _cache.Add(type, Enum.GetValues(p_flag.GetType()));
        }
        items = _cache[type];
        
        int flagsValue = (int)(object)p_flags;
        var counter = 0;
        foreach (var item in items)
        {
            int flagValue = (int)(object)p_flag;
            if ((flagValue & flagsValue) != 0)
            {
                counter++;
            }
            if (counter > 1)
            {
                return true;
            }
        }
        return false;
    }
}