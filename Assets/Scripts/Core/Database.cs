using System;
using System.Collections.Generic;
using UnityEngine;

public class Database : MonoBehaviour , IDisposable
{
    public string test;
    [SerializeField]
    public List<Type> dataListType = new List<Type>();
    [SerializeField]
    private List<Data> showData;
    public Dictionary<string, Data> databaseById = new Dictionary<string, Data>();
    public void Update()
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
    public static Database GetDatabase
    {
        get { return Singleton<Database>.Instance; }
    }
    public static Dictionary<string, Data> DatabaseDic
    {
        get { return Singleton<Database>.Instance.databaseById; }
    }
    public static List<Type> Datas
    {
        get { return Singleton<Database>.Instance.dataListType; }
    }
    public static bool isDataType (string key)
    {
        return GetDatabase.databaseById.ContainsKey(key);
    }
    public static Data GetData (string key)
    {
        return GetDatabase.databaseById[key];
    }
    public static void AddData (string key, Data data)
    {
        GetDatabase.databaseById.TryAdd(key, data);
    }
    public static void RemoveData (string key)
    {
        GetDatabase.databaseById.Remove(key);
    }
    public void Dispose()
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
