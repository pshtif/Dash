/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEngine;

namespace Dash
{
    public interface IVariableBindable : IExposedPropertyTable
    {
        DashGraph Graph { get; }
        
        GameObject gameObject { get; }
    }
}