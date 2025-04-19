using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class TextureData : Data
{
    public int x;
    public int y;
    public int w;//width
    public int h;//height
    public int pixelPerUnit;
    public Vector2 pivot;

    public float frameRate; // for animated textures
}