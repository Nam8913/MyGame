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

    public static object HelpParseType(string str, Type targetType)
    {
        object rs = null;
        try
        {
            rs = ParseSimpleValue(str, targetType);
            if(rs != null) return rs;
            rs = Convert.ChangeType(str, targetType);
        }
        catch (InvalidCastException invalidCast)
        {
            Debug.LogError($"Can't convert {str} to {targetType.Name}");
            Debug.LogError(invalidCast.Message);
            Debug.LogError(invalidCast.StackTrace);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Can't convert {str} to {targetType.Name}: {ex.Message}");
            Debug.LogError(ex.StackTrace);
            throw;
        } 
        return rs;
    }
    public static object ParseSimpleValue(string str, Type type)
    {
        if (type == typeof(string)) return str;
        if (type == typeof(float)) return ParseFloat(str);
        if (type == typeof(long)) return ParseLong(str);
        if (type == typeof(double)) return ParseDouble(str);
        if (type == typeof(sbyte)) return ParseSByte(str);
        if (type == typeof(int)) return ParseInt(str);
        if (type == typeof(bool)) return ParseBool(str);
        if (type == typeof(Vector2Int)) return ParseIntVec2(str);
        if (type == typeof(Vector2)) return ParseVec2(str);
        if (type == typeof(Vector3)) return ParseVec3(str);
        if (type == typeof(Vector4)) return ParseIntVec3(str);
        if (type.IsEnum) return Enum.Parse(type, str, true);

        //Debug.LogError($"Type '{type.Name}' không được hỗ trợ cho việc phân tích cú pháp.");
        return null;
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
    public static Vector2Int ParseIntVec2(string str)
    {
        string[] parts = str.Trim('(', ')').Split(',');
        if (parts.Length == 2 && int.TryParse(parts[0], out int x) && int.TryParse(parts[1], out int y))
        {
            return new Vector2Int(x, y);
        }
        throw new FormatException($"Invalid format for Vector2Int: {str}");
    }
    public static Vector2 ParseVec2(string str)
    {
        string[] parts = str.Trim('(', ')').Split(',');
        if (parts.Length == 2 && float.TryParse(parts[0], out float x) && float.TryParse(parts[1], out float y))
        {
            return new Vector2(x, y);
        }
        throw new FormatException($"Invalid format for Vector2: {str}");
    }
    public static Vector3 ParseVec3(string str)
    {
        string[] parts = str.Trim('(', ')').Split(',');
        if (parts.Length == 3 && float.TryParse(parts[0], out float x) && float.TryParse(parts[1], out float y) && float.TryParse(parts[2], out float z))
        {
            return new Vector3(x, y, z);
        }
        throw new FormatException($"Invalid format for Vector3: {str}");
    }
    public static Vector4 ParseIntVec3(string str)
    {
        string[] parts = str.Trim('(', ')').Split(',');
        if (parts.Length == 3 && int.TryParse(parts[0], out int x) && int.TryParse(parts[1], out int y) && int.TryParse(parts[2], out int z))
        {
            return new Vector3(x, y, z);
        }
        throw new FormatException($"Invalid format for Vector3: {str}");
    }
}
