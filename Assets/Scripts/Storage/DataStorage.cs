using System;
using System.Collections.Generic;
using UnityEngine;

public class DataStorage : Database<DataStorage> , IDisposable
{
    [SerializeField]
    public List<Type> dataListType = new List<Type>();
    [SerializeField]
    private List<Data> showData;
    public Dictionary<string, Data> databaseById = new Dictionary<string, Data>();
    public void OnGUI()
    {
        if(showData == null)
        {
            showData = new List<Data>();
        }else
        {
            showData.Clear();
        }
        foreach (var type in databaseById.Values)
        {
            showData.Add(type);
        }
    }
    public static DataStorage GetDatabase
    {
        get { return Singleton<DataStorage>.Instance; }
    }
    public static Dictionary<string, Data> DatabaseDic
    {
        get { return Singleton<DataStorage>.Instance.databaseById; }
    }
    public static List<Type> Datas
    {
        get { return Singleton<DataStorage>.Instance.dataListType; }
    }
    public static bool isDataType (string key)
    {
        return GetDatabase.databaseById.ContainsKey(key);
    }
    public static Data GetData (string key)
    {
        return GetDatabase.databaseById.TryGetValue(key, out var data) ? data : null;
    }
    public static T GetData<T>(string key) where T : Data
    {
        T rs = null;
        if (GetDatabase.databaseById.ContainsKey(key))
        {
            rs = GetDatabase.databaseById[key] as T;
            if(rs != null)
            {
                return rs;
            }
            Debug.LogError("Data type is not match with key: " + key + " and type: " + typeof(T).Name);
        }
        else
        {
            Debug.LogError("Key not found: " + key);
        }
        return rs;
    }
    public static void AddData (string key, Data data)
    {
        GetDatabase.databaseById.TryAdd(key, data);
    }
    public static void RemoveData (string key)
    {
        GetDatabase.databaseById.Remove(key);
    }
    public override void Dispose()
    {
        foreach (var data in dataListType)
        {
            if (data is IDisposable disposableData)
            {
                disposableData.Dispose();
            }
        }
        dataListType.Clear();
        databaseById.Clear();
    }
}
