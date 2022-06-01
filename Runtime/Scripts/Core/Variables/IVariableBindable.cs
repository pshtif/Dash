/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEngine;

namespace Dash
{
    public interface IVariableBindable : IExposedPropertyTable
    {
        GameObject gameObject { get; }
    }
}