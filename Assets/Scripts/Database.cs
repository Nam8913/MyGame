using System;
using UnityEngine;

public abstract class Database<T> : MonoBehaviour,IDisposable where T : Database<T>
{
    protected virtual void Init()
    {
        // Initialization logic here
    }

    public static Database<T> Get()
    {
        return Singleton<Database<T>>.Instance;
    }

    public virtual void Dispose()
    {
        throw new NotImplementedException();
    }
}
