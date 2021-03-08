/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    [Category(NodeCategoryType.HIDDEN)]
    [OutputCount(1)]
    [InputCount(1)]
    [Size(160,85)]
    [InspectorHeight(380)]
    public class AnimationNodeBase<T> : RetargetNodeBase<T> where T:AnimationNodeBaseModel, new()
    {
        protected override void ExecuteOnTarget(Transform p_target, NodeFlowData p_flowData)
        {
            Debug.LogWarning("ExecuteOnTarget not implemented in AnimationNodeBase class.");
        }
        
#if UNITY_EDITOR
        protected override void DrawCustomGUI(Rect p_rect)
        {
            base.DrawCustomGUI(p_rect);

            //if (Model.time == null) Model.time = new Parameter<float>(1);
            if (Model.time.isExpression)
            {
                GUI.Label(new Rect(p_rect.x + p_rect.width / 2 - 50, p_rect.y + p_rect.height - 32, 100, 20),
                    "Time: [Exp]", DashEditorCore.Skin.GetStyle("NodeText"));
            }
            else
            {
                GUI.Label(new Rect(p_rect.x + p_rect.width / 2 - 50, p_rect.y + p_rect.height - 32, 100, 20),
                    "Time: " + Model.time.GetValue(null) + "s", DashEditorCore.Skin.GetStyle("NodeText"));
            }
        }
#endif
    }
}