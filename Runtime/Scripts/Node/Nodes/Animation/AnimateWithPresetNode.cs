/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using Dash.Attributes;
using OdinSerializer;
using UnityEngine;
#if UNITY_EDITOR

#endif

namespace Dash
{
    [Obsolete]
    [Documentation("Nodes.md#animatewithpreset")]
    [Attributes.Tooltip("Animate RectTransform using an IAnimationPreset implementation. Useful to write custom animation sequences in code for reuse.")]
    [Category(NodeCategoryType.ANIMATION)]
    [OutputCount(1)]
    [InputCount(1)]
    [Size(200,85)]
    [Serializable]
    public class AnimateWithPresetNode : RetargetNodeBase<AnimateWithPresetNodeModel>
    {
        [NonSerialized] 
        protected List<DashTween> _activeTweens;
        
        protected override void ExecuteOnTarget(Transform p_target, NodeFlowData p_flowData)
        {
            if (_activeTweens == null) _activeTweens = new List<DashTween>();
            
            if (p_target == null)
            {
                ExecuteEnd(p_flowData);
                return;
            }
            
            DashTween tween = Model.preset.Execute(p_target, ParameterResolver, p_flowData);

            if (tween == null)
            {
                ExecuteEnd(p_flowData);
            } else {
                _activeTweens.Add(tween);
                tween.OnComplete(() => ExecuteEnd(p_flowData, tween)).Start();
            }
        }
        
        protected void ExecuteEnd(NodeFlowData p_flowData, DashTween p_tween = null)
        {
            if (p_tween != null)
            {
                _activeTweens.Remove(p_tween);
            }

            OnExecuteEnd();
            OnExecuteOutput(0,p_flowData);
        }

        protected override void Stop_Internal()
        {
            
        }
    }
}
