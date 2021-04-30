/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;
using TMPro;
using UnityEngine;

namespace Dash
{
    [Help("Animate TextMeshPro text per character.")]
    [Experimental]
    [Category(NodeCategoryType.ANIMATION)]
    [OutputCount(1)]
    [InputCount(1)]
    public class AnimateTextNode : AnimationNodeBase<AnimateTextNodeModel>
    {
        protected override DashTween AnimateOnTarget(Transform p_target, NodeFlowData p_flowData)
        {
            TMP_Text text = p_target.GetComponent<TMP_Text>();

            if (CheckException(text, "No TMP_Text component found on target"))
                return null;
            
            text.ForceMeshUpdate();
            for (int i = 0; i < text.text.Length; i++)
                 TMPTweenExtension.Scale(text, i, 0);
            
            float time = GetParameterValue(Model.time, p_flowData);
            EaseType easeType = GetParameterValue(Model.easeType, p_flowData);
            float characterDelay = GetParameterValue(Model.characterDelay, p_flowData);

            for (int i = 0; i < text.text.Length; i++)
            {
                 int index = i; // Rescope variable to avoid modified closure trap
                 DashTween.To(text, 0, 1, time)
                     .OnUpdate(f => TMPTweenExtension.Scale(text, index, f))
                     .SetDelay(index * characterDelay)
                     .SetEase(easeType)
                     .Start();
            }

            return DashTween.To(p_target, 0, 1, text.text.Length * .1f);
        }
    }
}