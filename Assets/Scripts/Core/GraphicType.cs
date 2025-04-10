using System.Collections.Generic;
using UnityEngine;

public abstract class GraphicType
{
   public string path;
}

public class GraphicSingleType : GraphicType
{
    public TextureData texture;
}
public class GraphicStackType : GraphicType
{
    public List<StackTextureCollector> textureStack;
}
public class GraphicMultiType : GraphicType
{
    public bool isAnimation;// if true it will be animated
    public List<TextureData> textures = new List<TextureData>();
}
public class StackTextureCollector
{
    public int minStack;
    public TextureData texture;
}