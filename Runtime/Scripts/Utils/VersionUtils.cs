/*
 *	Created by:  Peter @sHTiF Stefcek
 */

namespace Dash
{
    public class VersionUtils
    {
        public static string GetVersionString(int p_number)
        {
            string result = "";
            int number = p_number;
            while (number > 0)
            {
                result = "." + (number % 1000) + result;
                number /= 1000;
            }
            
            result = p_number <= 1000000 ? "0" + result : result.Substring(1);
        
            return result;
        }
    }
}