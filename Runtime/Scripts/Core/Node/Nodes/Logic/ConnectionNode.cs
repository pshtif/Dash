/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    [Attributes.Tooltip("Connection node without functionality helps with graph and connection management.")]
    [Category(NodeCategoryType.LOGIC)]
    [OutputCount(1)]
    [InputCount(1)]
    [DisableBaseGUI()]
    [Size(40,15)]
    public class ConnectionNode : NodeBase<NullNodeModel>
    {
        protected override void OnExecuteStart(NodeFlowData p_flowData)
        {
            OnExecuteEnd();
            OnExecuteOutput(0, p_flowData);
        }

        protected override void DrawCustomGUI(Rect p_rect)
        {
            GUI.DrawTexture(p_rect, Texture2D.whiteTexture);
        }
    }
}