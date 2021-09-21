/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System.Collections.Generic;
using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    [Help("Changes a current target within NodeFlowData with advanced option.")]
    [Category(NodeCategoryType.MODIFIER)]
    [OutputCount(1)]
    [InputCount(1)]
    [Size(160,85)]
    public class RetargetAdvancedNode : NodeBase<RetargetAdvancedNodeModel>
    {
        override protected void OnExecuteStart(NodeFlowData p_flowData)
        {
            List<Transform> transforms = new List<Transform>();
            Transform transform;
            
            if (!string.IsNullOrEmpty(Model.target))
            {
                if (!p_flowData.HasAttribute("target") && Model.isChild)
                {
                    Debug.LogWarning("Cannot retarget to a child of null in node "+_model.id);
                    
                    hasErrorsInExecution = true;
                    OnExecuteEnd();

                    return;
                }
                
                if (Model.isChild)
                {
                    if (Model.findAll)
                    {
                        transforms = Controller.transform.DeepFindAll(Model.target);
                    }
                    else
                    {
                        transform = Controller.transform.DeepFind(Model.target);
                        if (transform != null) transforms.Add(transform);
                    }
                }
                else
                {
                    if (Model.findAll)
                    {
                        transforms = Controller.transform.root.DeepFindAll(Model.target);
                    }
                    else
                    {
                        transform = Controller.transform.root.DeepFind(Model.target);
                        if (transform != null) transforms.Add(transform);
                    }
                }

                if (transforms.Count == 0)
                {
                    Debug.LogWarning("Zero valid retargets found in node "+_model.id);
                    
                    hasErrorsInExecution = true;
                    OnExecuteEnd();

                    return;
                }

                for (int i = 0; i < transforms.Count; i++)
                {
                    transform = Model.inReverse ? transforms[transforms.Count - i - 1] : transforms[i];
                    NodeFlowData data = p_flowData.Clone();
                    data.SetAttribute("target", transform);

                    if (Model.delay.GetValue(ParameterResolver) == 0)
                    {
                        OnExecuteOutput(0, data);
                    }
                    else
                    {
                        float time = Model.delay.GetValue(ParameterResolver) * i;
                        DashTween tween = DashTween.To(Graph.Controller, 0, 1, time);
                        tween.OnComplete(() => OnExecuteOutput(0, data));
                        tween.Start();
                        ((IInternalGraphAccess)Graph).AddActiveTween(tween);
                    }
                }

                if (Model.delay.GetValue(ParameterResolver) == 0)
                {
                    OnExecuteEnd();
                }
                else
                {
                    float time = Model.delay.GetValue(ParameterResolver) * transforms.Count;
                    DashTween tween = DashTween.To(Graph.Controller, 0, 1, time);
                    tween.OnComplete(() => OnExecuteEnd());
                    tween.Start();
                    ((IInternalGraphAccess)Graph).AddActiveTween(tween);
                }
            }
            else
            {
                Debug.LogWarning("Zero valid retargets found in node " + _model.id);

                hasErrorsInExecution = true;
                OnExecuteEnd();
            }
        }

        public override bool IsSynchronous()
        {
            return !Model.delay.isExpression && Model.delay.GetValue(null) == 0;
        }
        
#if UNITY_EDITOR
        protected override void DrawCustomGUI(Rect p_rect)
        {
            GUI.color = Color.white;
            var style = new GUIStyle();
            style.alignment = TextAnchor.MiddleLeft;

            Rect labelRect = new Rect(p_rect.x + 24, p_rect.y + DashEditorCore.TITLE_TAB_HEIGHT, p_rect.width-48, 20);

            
            style.normal.textColor = Color.white;

            GUI.Label(labelRect, Model.target, style);
            // Model.targetPath = GUI.TextField(new Rect(offsetRect.x + 24, offsetRect.y + 80, Size.x - 48, 20),
            //     Model.targetPath);
        }
#endif
    }
}