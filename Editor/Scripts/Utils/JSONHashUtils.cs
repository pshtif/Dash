/*
 *	Created by:  Peter @sHTiF Stefcek
 */
#if UNITY_EDITOR

namespace Dash.Editor
{
    public class JSONHashUtils
    {
        public static string GetJSONHash(string p_json)
        {
            var sha1 = System.Security.Cryptography.SHA1.Create();
            byte[] buf = System.Text.Encoding.UTF8.GetBytes(p_json);
            byte[] hash = sha1.ComputeHash(buf, 0, buf.Length);
            return System.BitConverter.ToString(hash).Replace("-", "");
        }
    }
}
#endif