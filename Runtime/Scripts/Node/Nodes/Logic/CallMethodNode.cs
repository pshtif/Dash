/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Reflection;
using Dash.Attributes;
using Dash.Editor;
using OdinSerializer.Utilities;
using UnityEngine;

namespace Dash
{
    [Experimental]
    [Attributes.Tooltip("Call a method using reflection on any component on DashController GameObject")]
    [Category(NodeCategoryType.LOGIC)]
    [OutputCount(1)]
    [InputCount(1)]
    public class CallMethodNode : NodeBase<CallMethodNodeModel>
    {
        protected override void OnExecuteStart(NodeFlowData p_flowData)
        {
            CallMethod();
            
            OnExecuteEnd();
            OnExecuteOutput(0, p_flowData);
        }
        
        void CallMethod()
        {
            if (Model.componentName.IsNullOrWhitespace() || Model.methodName.IsNullOrWhitespace())
                return;
            
            Type componentType = ReflectionUtils.GetTypeByName(Model.componentName);
            Component component = Controller.GetComponent(componentType);

            if (component == null)
                return;

            var method = component.GetType().GetMethod(Model.methodName, BindingFlags.Instance | BindingFlags.Public);
            method.Invoke(component, new object[]{});
        }
        
#if UNITY_EDITOR
        public override Vector2 Size
        {
            get
            {
                var textWidth = DashEditorCore.Skin.GetStyle("NodeText")
                    .CalcSize(new GUIContent(Model.shortComponentName + "." + Model.methodName)).x + 35;
                
                return new Vector2(textWidth > 150 ? textWidth : 150, 85);
            }
        }

        protected override void DrawCustomGUI(Rect p_rect)
        {

            Rect offsetRect = new Rect(rect.x + _graph.viewOffset.x, rect.y + _graph.viewOffset.y, rect.width,
                rect.height);

            if (Model.componentName.IsNullOrWhitespace())
            {
                GUI.Label(
                    new Rect(
                        new Vector2(offsetRect.x + offsetRect.width * .5f - 50, offsetRect.y + offsetRect.height - 32),
                        new Vector2(100, 20)), "No Method", DashEditorCore.Skin.GetStyle("NodeText"));
            } else {

                GUI.Label(
                    new Rect(
                        new Vector2(offsetRect.x + offsetRect.width * .5f - 50, offsetRect.y + offsetRect.height - 32),
                        new Vector2(100, 20)), Model.shortComponentName+"."+Model.methodName, DashEditorCore.Skin.GetStyle("NodeText"));
            }
        }
#endif

    }
}