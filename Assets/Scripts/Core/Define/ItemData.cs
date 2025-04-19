using System.Collections.Generic;
using UnityEngine;

public class ItemData : BuildableData
{
    public bool isStackable
    {
        get
        {
            return maxStack > 1;
        }
    }
    public int maxStack = 1;
    public Vector2 size = Vector2.one;

    public float equippedDistanceOffset;

    public bool destroyable = true;
    public bool rotatable = true;
    public bool canBeUsedUnderRoof = true;

    public ToolQuality toolQuality;

    public List<RecipeData> recipes;

    
}
