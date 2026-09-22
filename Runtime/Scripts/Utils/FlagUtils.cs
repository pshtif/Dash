/*
 *	Created by:  Peter @sHTiF Stefcek
 */

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
}