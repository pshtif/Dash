using System.Collections.Generic;
using Dash;
using PixelFederation.Common.Attributes;
using UnityEngine;

public class TestController : MonoBehaviour
{
    [Button()]
    public void ShowScreen1()
    {
        DashRuntimeCore.Instance.SendEvent("Hide", null);
        DashRuntimeCore.Instance.SendEvent("Show", new NodeFlowData(new Dictionary<string, object>(){{"screen","Screen1"}}));
    }

    [Button]
    public void ShowScreen2()
    {
        DashRuntimeCore.Instance.SendEvent("Hide", null);
        DashRuntimeCore.Instance.SendEvent("Show", new NodeFlowData(new Dictionary<string, object>(){{"screen","Screen2"}}));
    }
    
    [Button]
    public void HideScreen()
    {
        DashRuntimeCore.Instance.SendEvent("Hide", null);
    }
    
    [Button]
    public void ShowPopup1()
    {
        DashRuntimeCore.Instance.SendEvent("Popup", new NodeFlowData(new Dictionary<string, object>(){{"popup","Popup1"}}));
    }
    
    [Button]
    public void ShowPopup2()
    {
        DashRuntimeCore.Instance.SendEvent("Popup", new NodeFlowData(new Dictionary<string, object>(){{"popup","Popup2"}}));
    }
}
