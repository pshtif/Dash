/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEngine;
 
[RequireComponent (typeof (Camera))]
public class PlaymodeWireframe : MonoBehaviour
{
    public Color wireframeBackgroundColor = Color.black;
    public bool enableWireframe = false;
    public Camera Camera => GetComponent<Camera>();
    public bool isWireframe { get; private set; }
    
    private CameraClearFlags _defaultFlags;
    private Color _defaultColor;

    private void Start ()
    {
        _defaultFlags = Camera.clearFlags;
        _defaultColor = Camera.backgroundColor;
        isWireframe = enableWireframe;
    }
 
    private void Update ()
    {
        if (isWireframe == enableWireframe)
            return;
        
        
        isWireframe = enableWireframe;
        if (isWireframe)
        {
            _defaultFlags = Camera.clearFlags;
            _defaultColor = Camera.backgroundColor;
            Camera.clearFlags = CameraClearFlags.Color;
            Camera.backgroundColor = wireframeBackgroundColor;
        }
        else
        {
            Camera.clearFlags = _defaultFlags;
            Camera.backgroundColor = _defaultColor;
        }
    }
 
    private void OnPreRender ()
    {
        if (isWireframe)
        {
            GL.wireframe = true;
        }
    }
 
    private void OnPostRender ()
    {
        if (isWireframe)
        {
            GL.wireframe = false;
        }
    }
}