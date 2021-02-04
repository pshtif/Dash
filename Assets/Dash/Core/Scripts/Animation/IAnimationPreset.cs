/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEngine;

namespace Dash
{
    public interface IAnimationPreset
    {
        void ExecuteOnTarget(Transform p_target);
    }
}