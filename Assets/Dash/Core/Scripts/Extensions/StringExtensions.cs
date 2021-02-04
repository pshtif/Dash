/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Linq;

namespace Dash
{
    public static class StringExtensions
    {
        public static string RemoveWhitespace(this string input)
        {
            return new string(input.Where(c => !Char.IsWhiteSpace(c)).ToArray());
        }
    }
}