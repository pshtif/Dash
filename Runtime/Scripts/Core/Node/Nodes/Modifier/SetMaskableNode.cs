/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;
using OdinSerializer.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Dash
{
    [Attributes.Tooltip("Changes an active state of target.")]
    [Category(NodeCategoryType.MODIFIER)]
    [OutputCount(1)]
    [InputCount(1)]
    public class SetMaskableNode : RetargetNodeBase<SetMaskableNodeModel>
    {
        protected override void ExecuteOnTarget(Transform p_target, NodeFlowData p_flowData)
        {
            if (p_target != null)
            {
                bool maskable = GetParameterValue(Model.maskable, p_flowData);
                if (GetParameterValue(Model.wholeHierarchy, p_flowData))
                {
                    var graphics = p_target.GetComponentsInChildren<MaskableGraphic>();

                    graphics.ForEach(g => g.maskable = maskable);
                } else {
                    var graphics = p_target.GetComponent<MaskableGraphic>();
                    if (graphics != null)
                    {
                        graphics.maskable = maskable;
                    }
                }
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
                    new Vector2(100, 20)), Model.maskable.isExpression ? "EXP" : Model.maskable.GetValue(null) ? "True" : "False", DashEditorCore.Skin.GetStyle("NodeText"));
        }
#endif
    }
}