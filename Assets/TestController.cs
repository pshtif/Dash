using Dash;
using UnityEngine;
using UnityEngine.UI;

public class TestController : MonoBehaviour
{
    public Sprite[] sprites;
    public int type = 0;
    
    void Start()
    {
        
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            NodeFlowData data = NodeFlowDataFactory.Create();
            
            Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            RectTransform rect = GetComponent<RectTransform>();
            Vector2 point;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, mousePos, Camera.main, out point);
            data.SetAttribute<Vector2>("spawnPosition", point);
            
            RectTransform r = transform.GetChild(type).GetComponent<RectTransform>();
            data.SetAttribute<Vector2>("toPosition", TransformUtils.FromToRectTransform(r, rect));
            
            data.SetAttribute<int>("type", type);
            
            data.SetAttribute<Sprite>("sprite", sprites[type]);
            DashCore.Instance.SendEvent("Click", data);
        }    
    }
}
