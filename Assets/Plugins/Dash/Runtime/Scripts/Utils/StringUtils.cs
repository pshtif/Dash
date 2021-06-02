/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System.Globalization;

namespace Dash.Runtime
{
    public class StringUtils
    {
        static NumberFormatInfo nfi = new NumberFormatInfo {NumberGroupSeparator = ".", NumberDecimalDigits = 0};

        public static string GetDotFormat(int p_number)
        {
            return p_number.ToString("n", nfi);
        }
    }
}