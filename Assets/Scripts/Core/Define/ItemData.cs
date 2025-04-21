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
    

    public List<RecipeData> recipes;

    
}
