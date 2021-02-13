/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System.Reflection;
using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    [Category(NodeCategoryType.HIDDEN)]
    [OutputCount(1)]
    [InputCount(1)]
    [Size(160,85)]
    [InspectorHeight(380)]
    public abstract class RetargetNodeBase<T> : NodeBase<T> where T:RetargetNodeBaseModel, new()
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
                    target = Model.targetReference.Resolve(Controller);
                }
                else
                {
                    if (Model.isChild)
                    {
                        target = target.Find(Model.target);
                    }
                    else
                    {
                        GameObject go = GameObject.Find(Model.target);
                        target = go == null ? null : go.transform;
                    }
                }
            }

            if (CheckException(target, "No valid target found in node "+_model.id))
                return;
            
            ExecuteOnTarget(target, p_flowData);
        }

        protected abstract void ExecuteOnTarget(Transform p_target, NodeFlowData p_flowData);
        
#if UNITY_EDITOR
        protected override void DrawCustomGUI(Rect p_rect)
        {
            GUI.color = Color.white;
            var style = new GUIStyle();
            style.alignment = TextAnchor.MiddleLeft;

            Rect labelRect = new Rect(p_rect.x + 24, p_rect.y + DashEditorCore.TITLE_TAB_HEIGHT, p_rect.width-48, 20);

            if (Model.retarget)
            {
                if (Model.useReference)
                {
                    FieldInfo fi = Model.GetType().GetField("targetReference");
                    var exposedReference = fi.GetValue(Model);
                    Object exposedValue = (Object)exposedReference.GetType().GetMethod("Resolve")
                        .Invoke(exposedReference, new object[] {Controller});

                    style.fontStyle = FontStyle.Bold;
                    style.normal.textColor = new Color(0.4f, .7f, 1);
                    string exposedLabel = exposedValue != null ? exposedValue.name : "None"; 
                    GUI.Label(labelRect, "["+exposedLabel+"]", style);

                    // PropertyName exposedName = (PropertyName)exposedReference.GetType().GetField("exposedName").GetValue(exposedReference);
                    // bool isDefault = PropertyName.IsNullOrEmpty(exposedName);

                    // GUIPropertiesUtils.ExposedReferenceField(
                    //      new Rect(offsetRect.x + 24, offsetRect.y + 80, Size.x - 48, 20), fi, Model);
                }
                else
                {
                    style.normal.textColor = Color.white;

                    GUI.Label(labelRect, ShortenPath(Model.target), style);
                    // Model.targetPath = GUI.TextField(new Rect(offsetRect.x + 24, offsetRect.y + 80, Size.x - 48, 20),
                    //     Model.targetPath);
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
#endif
    }
}
