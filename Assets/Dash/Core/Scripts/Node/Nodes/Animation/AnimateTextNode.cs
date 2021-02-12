/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Dash
{
    [Experimental]
    [Category(NodeCategoryType.ANIMATION)]
    [OutputCount(1)]
    [InputCount(1)]
    public class AnimateTextNode : AnimationNodeBase<AnimateTextNodeModel>
    {
        protected override void ExecuteOnTarget(Transform p_target, NodeFlowData p_flowData)
        {
            TMP_Text text = p_target.GetComponent<TMP_Text>();

            if (CheckException(text, () => ExecuteEnd(p_flowData), "No TMP_Text component found on target in node "+_model.id))
                return;
            
            text.ForceMeshUpdate();
            for (int i = 0; i < text.text.Length; i++)
                 TMPTweenExtension.Scale(text, i, 0);
            
            
            for (int i = 0; i < text.text.Length; i++)
            {
                 int index = i; // Rescope variable to avoid modified closure trap
                 Tween tween = DOTween.To((f) => TMPTweenExtension.Scale(text, index, f), 0, 1, Model.time).SetDelay(index * Model.characterDelay);
                 DOPreview.StartPreview(tween);
            }

            DOPreview.DelayedCall(text.text.Length * .1f, () => ExecuteEnd(p_flowData));
        }
        
        void ExecuteEnd(NodeFlowData p_flowData)
        {
            OnExecuteEnd();
            OnExecuteOutput(0,p_flowData);
        }
    }
}