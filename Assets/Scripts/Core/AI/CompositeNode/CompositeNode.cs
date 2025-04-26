using System.Collections.Generic;
using UnityEngine;

public abstract class CompositeNode : NodeAbstract
{
    public List<NodeAbstract> children = new List<NodeAbstract>();
}
