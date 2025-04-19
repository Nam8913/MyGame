using System;
using System.Collections.Generic;

public class ObjectPool<T> where T : class
{
    private readonly Func<T> _objectGenerator;
    private readonly Stack<T> _pool;

    public int Count => _pool.Count;

    public ObjectPool(Func<T> objectGenerator)
    {
        _objectGenerator = objectGenerator ?? throw new ArgumentNullException(nameof(objectGenerator));
        _pool = new Stack<T>();
    }

    public T Get()
    {
        return _pool.Count > 0 ? _pool.Pop() : _objectGenerator();
    }

    public void Return(T obj)
    {
        if (obj == null) return;
        _pool.Push(obj);
    }

    public void Prewarm(int count)
    {
        for (int i = 0; i < count; i++)
        {
            _pool.Push(_objectGenerator());
        }
    }
}
