/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System.Collections;

namespace Dash
{
    public interface IAttributeDataCollection : IEnumerable
    {
        bool HasAttribute(string p_name);

        T GetAttribute<T>(string p_name);

        object GetAttribute(string p_name);
    }
}