using System;
using System.Collections.Generic;
using UnityEngine;

public class BodyPart : BuildableData
{
    public bool isIndependentFormParent;
    public bool isContinen;
    public bool willDeadWithout;
    public bool isOrgan;
    public bool isCoverUnderSkin;
    public int amount = 1;
    public List<BodyPart> bodyParts;

    public List<BodyPart> GetValidShowBodyParts()
    {
        List<BodyPart> result = new List<BodyPart>();
        foreach (var item in bodyParts)
        {
            if (item.isContinen || item.isIndependentFormParent)
            {
                result.Add(item);
                TraverseBodyParts(item, (bp) =>
                {
                    if (bp.isContinen || bp.isIndependentFormParent)
                    {
                        result.Add(bp);
                    }
                });
            }
        }
        return result;
    }
    public void TraverseBodyParts(BodyPart parent, Action<BodyPart> visiter)
    {
        if(parent != null)
        {
            visiter.Invoke(parent);
            foreach (var item in parent.bodyParts)
            {
                var bodyPartsChild = GetBodyPartsInside(item);
                if (bodyPartsChild != null && bodyPartsChild.Count > 0)
                {
                    bodyPartsChild.ForEach((bp) => TraverseBodyParts(bp, visiter));
                }
            }
        }
    }
    public List<BodyPart> GetBodyPartsInside(BodyPart parent)
    {
        
        List<BodyPart> result = new List<BodyPart>();
        if (parent != null)
        {
            result.Add(parent);
            result.AddRange(parent.bodyParts);
        }
        return result;
    }
    public List<BodyPart> GetAllBodyParts()
    {
        List<BodyPart> result = new List<BodyPart>();
        TraverseBodyParts(this, (bp) => result.Add(bp));
        return result;
    }
}
