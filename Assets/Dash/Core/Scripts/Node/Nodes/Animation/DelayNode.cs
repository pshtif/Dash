/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;
using DG.Tweening;
using UnityEngine;

namespace Dash
{
    [Category(NodeCategoryType.ANIMATION)]
    [OutputCount(1)]
    [InputCount(1)]
    public class DelayNode : NodeBase<DelayNodeModel>
    {
        override protected void OnExecuteStart(NodeFlowData p_flowData)
        {
            float time = GetParameterValue(Model.time, p_flowData);

            if (time == 0)
            {
                OnExecuteEnd();
                OnExecuteOutput(0, p_flowData);
            }
            else
            {
                DOPreview.DelayedCall(time, () =>
                {
                    OnExecuteEnd();
                    OnExecuteOutput(0, p_flowData);
                });
            }
        }
        
        #if UNITY_EDITOR
        protected override void DrawCustomGUI(Rect p_rect)
        {
            GUI.Label(new Rect(p_rect.x + p_rect.width / 2 - 50, p_rect.y + p_rect.height - 32, 100, 20),
                "Time: " + Model.time.ToString() + "s", DashEditorCore.Skin.GetStyle("NodeText"));
        }
        #endif
    }
}