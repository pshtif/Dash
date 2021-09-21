using System.Collections.Generic;
using Dash;
using PixelFederation.Common.Attributes;
using UnityEngine;

public class TestController : MonoBehaviour
{
    [Button()]
    public void ShowSection1()
    {
        DashCore.Instance.SendEvent("Hide", null);
        DashCore.Instance.SendEvent("Show", new NodeFlowData(new Dictionary<string, object>(){{"screen","Screen1"}}));
    }

    [Button]
    public void ShowSection2()
    {
        DashCore.Instance.SendEvent("Hide", null);
        DashCore.Instance.SendEvent("Show", new NodeFlowData(new Dictionary<string, object>(){{"screen","Screen2"}}));
    }
    
    [Button]
    public void Hide()
    {
        DashCore.Instance.SendEvent("Hide", null);
    }
    
    [Button]
    public void Popup()
    {
        DashCore.Instance.SendEvent("Popup", null);
    }
}
