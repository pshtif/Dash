/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Reflection;
using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    [Experimental]
    [Help("Call a method using reflection on any component on DashController GameObject")]
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
            Type componentType = ReflectionUtils.GetTypeByName(Model.componentName);
            Component component = Controller.GetComponent(componentType);

            if (component == null)
                return;

            var method = component.GetType().GetMethod(Model.methodName, BindingFlags.Instance | BindingFlags.Public);
            method.Invoke(component, new object[]{});
        }

    }
}