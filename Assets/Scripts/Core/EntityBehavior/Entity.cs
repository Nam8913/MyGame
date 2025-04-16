using System;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public virtual T Get<T>() where T : BuildableData
    {
        return this.dataDef as T;
    }
    public static GameObject MakeEntityFor(BuildableData build)
    {
        GameObject obj = new GameObject(build.name);
        Type type = System.Type.GetType(build.entityClass);
        obj.AddComponent(type);
        Entity ent = obj.GetComponent<Entity>();
        ent.dataDef = build;
        return obj;
    }
    [SerializeField]
    public BuildableData dataDef;
}
