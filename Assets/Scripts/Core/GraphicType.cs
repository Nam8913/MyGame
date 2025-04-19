using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public abstract class GraphicType
{
   public string path;
}

public class GraphicSingleType : GraphicType
{
    public TextureData texture;
}
public class GraphicMultiType : GraphicType
{
    public bool isAtlas;
    public bool isAnimation;
    public List<TextureData> textures = new List<TextureData>();
}