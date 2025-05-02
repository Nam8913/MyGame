using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Blackboard
{
    protected Dictionary<string, object> data = new Dictionary<string, object>();
    ContextTree contextTree;
    // Lấy giá trị từ blackboard
    public T GetValue<T>(string key)
    {
        if (data.TryGetValue(key, out var value))
        {
            return (T)value;
        }
        return default;
    }

    // Đặt giá trị vào blackboard
    public void SetValue<T>(string key, T value)
    {
        data[key] = value;
    }

    // Kiểm tra xem key có tồn tại không
    public bool ContainsKey(string key)
    {
        return data.ContainsKey(key);
    }

    // Xóa giá trị theo key
    public void Remove(string key)
    {
        data.Remove(key);
    }

    public ContextTree GetContextTree
    {
        get { return contextTree; }
    }
    public ContextTree SetContextTree
    {
        
        set { contextTree = value; }
    }

    public GameObject GetOwner
    {
        get { return contextTree.gameObject; }
    }
}
