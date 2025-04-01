using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Class)]
public class SupLoadXmlAttribute : Attribute
{
    public SupLoadXmlAttribute(typeSup type)
    {
        this.type = type;
    }
    public typeSup type;
    public enum typeSup
    {
        noSup,
        simple,
        process
    }
}

