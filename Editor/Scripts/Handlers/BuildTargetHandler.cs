/*
 *	Created by:  Peter @sHTiF Stefcek
 */
#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.Build;

namespace Dash.Editor
{
    public class BuildTargetHandler : IActiveBuildTargetChanged
    {
        public int callbackOrder { get { return 0; } }
        public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
        {
            DashEditorCore.SetDefineSymbols();
        }
    }
}
#endif