using System;
using System.Collections.Generic;
using UnityEngine;

//Buildable here does not mean building something, but means this can be constructed like a building, created by hand or interacted with in the world.
// this is a base class for all buildable data, like item, block, etc 
public class BuildableData : Data
{
    public string entityClass = "Entity";
    public List<string> comps;

    public GraphicType graphicType;

    public float weight;
    public float volume;
    public float hitPoint; // how many hit points this object has, if it is 0, it will be destroyed instantly

    public List<string> flags;
}
