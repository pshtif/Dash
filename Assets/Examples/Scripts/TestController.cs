using Dash;
using PixelFederation.Common.Attributes;
using UnityEngine;

public class TestController : MonoBehaviour
{
    [Button()]
    public void ShowSection1()
    {
        DashCore.Instance.SendEvent("Hide2", null);
        DashCore.Instance.SendEvent("Show1", null);
    }
    
    [Button]
    public void HideSection1()
    {
        DashCore.Instance.SendEvent("Hide1", null);
    }
    
    [Button]
    public void ShowSection2()
    {
        DashCore.Instance.SendEvent("Hide1", null);
        DashCore.Instance.SendEvent("Show2", null);
    }
    
    [Button]
    public void HideSection2()
    {
        DashCore.Instance.SendEvent("Hide2", null);
    }
}
