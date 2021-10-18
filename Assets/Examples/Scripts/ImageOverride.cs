using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageOverride : Image
{
    private List<UIVertex> _vertexBuffer = new List<UIVertex>();
    private List<int> _indexBuffer = new List<int>();

    private bool _initialized = false;

    public int tesselation = 1;

    // No tesselation support yet
    /*public Rect imageRect
    {
        get
        {
            return new Rect(topLeft.x, topLeft.y, bottomRight.x-bottomLeft.x, bottomRight.y - topRight.y);
        }

        set
        {
            topLeft = new Vector2(value.x, value.y);
            topRight = new Vector2(value.x + value.width, value.y);
            bottomLeft = new Vector2(value.x, value.y + value.height);
            bottomRight = new Vector2(value.x + value.width, value.y + value.height);
        }
    } */

    public Vector2 bottomLeft
    {
        get
        {
            return _vertexBuffer[0].position;
        }
        set
        {
            UIVertex vertex = _vertexBuffer[0];
            vertex.position = value;
            _vertexBuffer[0] = vertex;
        }
    }
    
    public Vector2 bottomRight
    {
        get
        {
            return _vertexBuffer[tesselation+1].position;
        }
        set
        {
            UIVertex vertex = _vertexBuffer[tesselation+1];
            vertex.position = value;
            _vertexBuffer[tesselation+1] = vertex;
        }
    }
    
    public Vector2 topLeft
    {
        get
        {
            return _vertexBuffer[(tesselation+2) * (tesselation+1)].position;
        }
        set
        {
            UIVertex vertex = _vertexBuffer[(tesselation+2) * (tesselation+1)];
            vertex.position = value;
            _vertexBuffer[(tesselation+2) * (tesselation+1)] = vertex;
        }
    }

    public Vector2 topRight
    {
        get { return _vertexBuffer[(tesselation+2) * (tesselation + 2) - 1].position; }
        set
        {
            UIVertex vertex = _vertexBuffer[(tesselation+2) * (tesselation + 2) - 1];
            vertex.position = value;
            _vertexBuffer[(tesselation+2) * (tesselation + 2) - 1] = vertex;
        }
    }
    
    public Vector3 QuadLerp(Vector3 p_a, Vector3 p_b, Vector3 p_c, Vector3 p_d, float p_u, float p_v)
    {
        Vector3 abu = Vector3.Lerp(p_a, p_b, p_u);
        Vector3 cdu = Vector3.Lerp(p_c, p_d, p_u);
        return Vector3.Lerp(cdu, abu, p_v);
    }

    public void Invalidate()
    {
        CreateVertexBuffer();
        CreateIndexBuffer();
    }

    public void Interpolate()
    {
        for (int i = 0; i < 2 + tesselation; i++)
        {
            for (int j = 0; j < 2 + tesselation; j++)
            {
                int index = i * (tesselation + 2) + j;
                UIVertex vertex = _vertexBuffer[index];
                vertex.position = QuadLerp(topLeft, topRight, bottomLeft, bottomRight, j / (tesselation + 1f),
                    i / (tesselation + 1f));
                _vertexBuffer[index] = vertex;
            }
        }
    }

    protected override void OnPopulateMesh(VertexHelper p_helper)
    {
        /*List<UIVertex> stream = new List<UIVertex>();
        p_helper.GetUIVertexStream(stream);
        stream.ForEach(v => Debug.Log(v.uv0));*/

        p_helper.Clear();
        p_helper.AddUIVertexStream(_vertexBuffer, _indexBuffer);
    }

    void CreateVertexBuffer()
    {
        _vertexBuffer = new List<UIVertex>();
        _indexBuffer = new List<int>();
        
        var vertices = new UIVertex[(2 + tesselation) * (2 + tesselation)];
        float width = rectTransform.sizeDelta.x;
        float height = rectTransform.sizeDelta.y;
        for (int i = 0; i < 2 + tesselation; i++)
        {
            for (int j = 0; j < 2 + tesselation; j++)
            {
                int index = i * (tesselation + 2) + j;
                vertices[index].position = new Vector2(-width / 2 + j * width / (tesselation + 1),
                    -height / 2 + i * height / (tesselation + 1));
                //Debug.Log(vertices[index].position);
                vertices[index].uv0 = new Vector2(j / (tesselation + 1f), i / (tesselation + 1f));
                //Debug.Log(vertices[index].uv0);
                vertices[index].color = Color.white;
            }
        }
        /*vertices[0].position = new Vector2(-rectTransform.sizeDelta.x / 2, -rectTransform.sizeDelta.y / 2);
        vertices[1].position = new Vector2(-rectTransform.sizeDelta.x / 2, rectTransform.sizeDelta.y / 2);
        vertices[2].position = new Vector2(rectTransform.sizeDelta.x / 2, rectTransform.sizeDelta.y / 2);
        vertices[3].position = new Vector2(rectTransform.sizeDelta.x / 2, -rectTransform.sizeDelta.y / 2);

        vertices[0].uv0 = new Vector2(0, 0);
        vertices[1].uv0 = new Vector2(0, 1);
        vertices[2].uv0 = new Vector2(1, 1);
        vertices[3].uv0 = new Vector2(1, 0);

        vertices[0].color = vertices[1].color = vertices[2].color = vertices[3].color = Color.white;*/
        _vertexBuffer.AddRange(vertices);
    }
    
    void CreateIndexBuffer()
    {
        _indexBuffer = new List<int>();//{ 0, 1, 3, 1, 3, 2 };

        for (int i = 0; i < 1 + tesselation; i++)
        {
            for (int j = 0; j < 1 + tesselation; j++)
            {
                _indexBuffer.Add(i * (tesselation + 2) + j);
                _indexBuffer.Add(i * (tesselation + 2) + j + 1);
                _indexBuffer.Add((i + 1) * (tesselation + 2) + j);
                
                _indexBuffer.Add(i * (tesselation + 2) + j + 1);
                _indexBuffer.Add((i + 1) * (tesselation + 2) + j + 1);
                _indexBuffer.Add((i + 1) * (tesselation + 2) + j);
            }
        }
    }
}
