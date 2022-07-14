/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Reflection;
using Dash.Attributes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Dash
{
    [Category(NodeCategoryType.HIDDEN)]
    [OutputCount(1)]
    [InputCount(1)]
    [Size(160,85)]
    [InspectorHeight(380)]
    [DebugOverride]
    public abstract class RetargetNodeBase<T> : NodeBase<T> where T:RetargetNodeModelBase, new()
    {
        override protected void OnExecuteStart(NodeFlowData p_flowData)
        {
            Transform target = null;

            if (!p_flowData.HasAttribute("target") && Model.isChild)
            {
                Debug.LogWarning("Cannot retarget to a child of null in node "+_model.id);
                hasErrorsInExecution = true;

                return;
            }


            target = p_flowData.GetAttribute<Transform>("target");

            // Handle retargeting
            if (Model.retarget)
            {
                if (Model.useReference)
                {
                    if (Model.useExpression)
                    {
                        // var value = ExpressionEvaluator.EvaluateTypedExpression(Model.targetExpression, typeof(Transform),
                        //     ParameterResolver, p_flowData);
                        var value = ExpressionEvaluator.EvaluateUntypedExpression(Model.targetExpression,
                            ParameterResolver, p_flowData, false);
                        
                        if (ExpressionEvaluator.hasErrorInEvaluation)
                        {
                            SetError(ExpressionEvaluator.errorMessage);
                            return;
                        }
                        
                        if (value.GetType().GetGenericTypeDefinition() == typeof(ExposedReference<>))
                        {
                            value = (Object) value.GetType().GetMethod("Resolve")
                                .Invoke(value, new object[] {DashEditorCore.EditorConfig.editingController});
                        }
                        
                        target = value as Transform;

                        if (target == null && value.GetType() == typeof(GameObject))
                        {
                            target = (value as GameObject).transform;
                        } 
                        else if (target == null && value is Component)
                        {
                            target = (value as Component).transform;
                        }
                    }
                    else
                    {
                        target = Model.targetReference.Resolve(Controller);
                    }
                }
                else
                {
                    if (target != null)
                    {
                        if (Model.isChild)
                        {
                            string find = GetParameterValue(Model.target, p_flowData);
                            target = target.Find(find, true);
                        }
                        else
                        {
                            string find = GetParameterValue(Model.target, p_flowData);
                            GameObject go = GameObject.Find(find);
                            target = go == null ? null : go.transform;
                        }
                    }
                }
            }
            
            #if UNITY_EDITOR
            DashEditorDebug.Debug(new NodeDebugItem(NodeDebugItem.NodeDebugItemType.EXECUTE, Graph.Controller, Graph.GraphPath, _model.id, target));
            #endif

            if (CheckException(target, "No valid target found"))
                return;

            ExecuteOnTarget(target, p_flowData);
        }

        protected abstract void ExecuteOnTarget(Transform p_target, NodeFlowData p_flowData);
        
#if UNITY_EDITOR
        protected override void DrawCustomGUI(Rect p_rect)
        {
            GUI.color = Color.white;
            var style = new GUIStyle();
            style.alignment = TextAnchor.MiddleCenter;

            Rect labelRect = new Rect(p_rect.x + p_rect.width/2 - 50, p_rect.y - 23, 100, 24);

            if (Model.retarget)
            {
                GUI.color = new Color(0.35f, 0.35f, 0.35f);
                GUI.Box(labelRect, "", DashEditorCore.Skin.GetStyle(TitleSkinId));
                GUI.color = Color.white;
                
                if (Model.useReference)
                {
                    if (Model.useExpression)
                    {
                        style.normal.textColor = Color.cyan;
                        GUI.Label(labelRect, "Expression", style);
                    }
                    else
                    {
                        FieldInfo fi = Model.GetType().GetField("targetReference");
                        var exposedReference = fi.GetValue(Model);
                        Object exposedValue = (Object) exposedReference.GetType().GetMethod("Resolve")
                            .Invoke(exposedReference, new object[] {DashEditorCore.EditorConfig.editingController});

                        style.fontStyle = FontStyle.Bold;
                        style.normal.textColor = new Color(0.4f, .7f, 1);
                        string exposedLabel = exposedValue != null ? exposedValue.name : "None";
                        GUI.Label(labelRect, "[" + exposedLabel + "]", style);

                        // PropertyName exposedName = (PropertyName)exposedReference.GetType().GetField("exposedName").GetValue(exposedReference);
                        // bool isDefault = PropertyName.IsNullOrEmpty(exposedName);

                        // GUIPropertiesUtils.ExposedReferenceField(
                        //      new Rect(offsetRect.x + 24, offsetRect.y + 80, Size.x - 48, 20), fi, Model);
                    }
                }
                else
                {
                    if (Model.target != null)
                    {
                        if (Model.target.isExpression)
                        {
                            style.normal.textColor = Color.cyan;
                            GUI.Label(labelRect, "Expression", style);
                        }
                        else
                        {
                            style.normal.textColor = Color.white;
                            GUI.Label(labelRect, ShortenPath(Model.target.GetValue(ParameterResolver)), style);
                        }
                    }
                    else
                    {
                        Model.target = new Parameter<string>("");
                    }
                }
            }
        }

        string ShortenPath(string p_path)
        {
            if (string.IsNullOrEmpty(p_path))
                return "";
            
            string result = "";
            string[] split = p_path.Split('/');
            
            for (int i = 0; i < split.Length - 1; i++)
                result += "../";
            
            result += split[split.Length - 1];
            
            return result;
        }

        internal override Transform ResolveNodeRetarget(Transform p_transform, NodeConnection p_connection)
        {
            if (p_connection != null || !Model.retarget)
                return p_transform;

            // If we use reference
            if (Model.useReference)
            {
                if (!Model.useExpression)
                {
                    return Model.targetReference.Resolve(DashEditorCore.EditorConfig.editingController);
                }
            }
            else
            {
                if (!Model.target.isExpression && p_transform != null)
                {
                    return p_transform.Find(Model.target.GetValue(null), true);
                }
            }

            // If we use expressions don't try to resolve it
            return null;
        }
#endif
    }
}
