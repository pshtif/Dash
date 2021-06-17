/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    [Help("Changes an active state of target.")]
    [Category(NodeCategoryType.MODIFIER)]
    [OutputCount(1)]
    [InputCount(1)]
    public class SetActiveNode : RetargetNodeBase<SetActiveNodeModel>
    {
        protected override void ExecuteOnTarget(Transform p_target, NodeFlowData p_flowData)
        {
            if (p_target != null)
            {
                p_target.gameObject.SetActive(GetParameterValue(Model.active, p_flowData));
            }

            OnExecuteEnd();
            OnExecuteOutput(0, p_flowData);
        }
        
#if UNITY_EDITOR
        protected override void DrawCustomGUI(Rect p_rect)
        {
            base.DrawCustomGUI(p_rect);
            Rect offsetRect = new Rect(rect.x + _graph.viewOffset.x, rect.y + _graph.viewOffset.y, rect.width,
                rect.height);

            GUI.Label(
                new Rect(new Vector2(offsetRect.x + offsetRect.width * .5f - 50, offsetRect.y + offsetRect.height - 32),
                    new Vector2(100, 20)), Model.active.isExpression ? "EXP" : Model.active.GetValue(null) ? "True" : "False", DashEditorCore.Skin.GetStyle("NodeText"));
        }
#endif
    }
}