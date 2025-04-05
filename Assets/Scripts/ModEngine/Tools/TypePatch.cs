using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

public static class TypePatch
{
    public static void PrintObjectInfo(object obj)
    {
        if(obj ==null)
        {
            Debug.Log("Object is null");
            return;
        }
        Type type = obj.GetType();
        Debug.Log($"🔍 Kiểm tra object: {type.Name}");

        // Lấy và in các fields (biến trực tiếp)
        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (var field in fields)
        {
            object value = field.GetValue(obj);
            if (value is System.Collections.IEnumerable list && value is not string)
            {
                Debug.Log($"📂 {field.Name} (List):");
                foreach (var item in list)
                {
                   Debug.Log($"   - {item}");
                }
            }
            else
            {
                Debug.Log($"📌 {field.Name} = {value}");
            }
        }

        // Lấy và in các properties (có getter/setter)
        PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var prop in properties)
        {
            object value = prop.GetValue(obj);
            Debug.Log($"🔹 {prop.Name} = {value}");
        }
    }

    public static bool HasAttribute<TAttribute>(Type type) where TAttribute : Attribute
    {
        return type.GetCustomAttribute(typeof(TAttribute)) != null;
    }

    public static bool IsList(Type type)
    {
        if (type.IsGenericType)
        {
            Type genericType = type.GetGenericTypeDefinition();
            if (genericType == typeof(List<>) || genericType == typeof(IList<>) || genericType == typeof(ICollection<>))
            {
                return true;
            }
        }
        return false;
    }
    public static bool IsDictionary(Type type)
    {
        if (type.IsGenericType)
        {
            Type genericType = type.GetGenericTypeDefinition();
            if (genericType == typeof(Dictionary<,>) || genericType == typeof(IDictionary<,>))
            {
                return true;
            }
        }
        return false;
    }

    public static object HelpChangeType(string value, Type targetType)
    {
        object rs = null;
        try
        {
            rs = Convert.ChangeType(value, targetType);
        }
        catch (InvalidCastException invalidCast)
        {
            Debug.LogError($"Can't convert {value} to {targetType.Name}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Can't convert {value} to {targetType.Name}: {ex.Message}");
            Debug.LogError(ex.StackTrace);
            throw;
        } 
        return rs;
    }
    public static Type ParseType(string str)
    {
        if (str == null) return null;
        str = Regex.Replace(str, @"\s+", " ").Trim(); // Xóa khoảng trắng thừa
        //Type type = Type.GetType(str);
        Type type = GenTypes.GetTypeInAnyAssembly(str);
        if (type == null)
        {
            Debug.LogError($"Type '{str}' không tồn tại.");
            return null;
        }
        return type;
    }
    public static float ParseFloat(string str)
    {
        return float.Parse(str, CultureInfo.InvariantCulture);
    }
    public static long ParseLong(string str)
    {
        return long.Parse(str, CultureInfo.InvariantCulture);
    }
    public static double ParseDouble(string str)
    {
        return double.Parse(str, CultureInfo.InvariantCulture);
    }
    public static sbyte ParseSByte(string str)
    {
        return sbyte.Parse(str, CultureInfo.InvariantCulture);
    }
    public static int ParseInt(string str)
    {
        return int.Parse(str);
    }
    public static bool ParseBool(string str)
    {
        return bool.Parse(str);
    }
}
