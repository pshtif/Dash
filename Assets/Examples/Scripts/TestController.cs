using Dash;
using UnityEngine;

public class TestController : MonoBehaviour
{
    public Sprite[] sprites;
    [Range(0,2)]
    public int type = 0;
    [Range(0,100)]
    public int count = 10;
    
    void Start()
    {
        
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Create NodeFlowData
            NodeFlowData data = NodeFlowDataFactory.Create();
            
            // Sending mouse position to Dash
            Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            RectTransform rect = GetComponent<RectTransform>();
            Vector2 point;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, mousePos, Camera.main, out point);
            data.SetAttribute<Vector2>("spawnPosition", point);
            
            // Sending to position
            RectTransform r = transform.GetChild(type).GetComponent<RectTransform>();
            data.SetAttribute<Vector2>("toPosition", TransformUtils.FromToRectTransform(r, rect));
            
            // Sending type
            data.SetAttribute<int>("type", type);
            
            // Sending sprite
            data.SetAttribute<Sprite>("sprite", sprites[type]);
            
            data.SetAttribute<int>("count", count);
            
            // Sending event
            DashCore.Instance.SendEvent("Click", data);
        }    
    }

    public void TestCall()
    {
        Debug.Log("Hello World");
    }

    public static void TestStatic()
    {
        
    }

    private void TestPrivate()
    {
        
    }
}
