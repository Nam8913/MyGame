using System;
using System.Collections.Generic;
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
    public static SupLoadXmlAttribute.typeSup SupportLoad(Type type)
    {
        if (HasAttribute<SupLoadXmlAttribute>(type))
        {
            SupLoadXmlAttribute supLoadXmlAttribute = (SupLoadXmlAttribute)type.GetCustomAttribute(typeof(SupLoadXmlAttribute));
            return supLoadXmlAttribute.type;
        }
        return SupLoadXmlAttribute.typeSup.noSup;
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

    public static Type GetTypeInAnyAssembly(string typeName, string nameSpace)
    {        
        Type type = null;
        if (!typeCache.TryGetValue(nameSpace, out type))
        {
            type = GetTypeInAnyAssemblyInt(typeName, nameSpace);
            typeCache.Add(nameSpace, type);
        }
        return type;
    }
    private static Type GetTypeInAnyAssemblyInt(string typeName, string nameSpace)
    {
        Type rs = GetTypeInAnyAssemblyRaw(typeName);
        if(rs != null)
        {
            return rs;
        }
        if(!string.IsNullOrEmpty(nameSpace) && IgnoredNamespaceNames.Contains(nameSpace))
        {
            rs = GetTypeInAnyAssemblyRaw(nameSpace+"."+typeName);
        }
        if(TryGetMixedAssemblyGenericType(typeName,out rs))
        {
            return rs;
        }
        return null;
    }
    private static bool TryGetMixedAssemblyGenericType(string typeName, out Type type)
		{
			type = GetTypeInAnyAssemblyRaw(typeName);
			if (type == null && typeName.Contains("`"))
			{
				try
				{
					Match match = Regex.Match(typeName, "(?<MainType>.+`(?<ParamCount>[0-9]+))(?<Types>\\[.*\\])");
					if (match.Success)
					{
						int capacity = int.Parse(match.Groups["ParamCount"].Value);
						string value = match.Groups["Types"].Value;
						List<string> list = new List<string>(capacity);
						foreach (object obj in Regex.Matches(value, "\\[(?<Type>.*?)\\],?"))
						{
							Match match2 = (Match)obj;
							if (match2.Success)
							{
								list.Add(match2.Groups["Type"].Value.Trim());
							}
						}
						Type[] array = new Type[list.Count];
						for (int i = 0; i < list.Count; i++)
						{
							Type type2;
							if (!TryGetMixedAssemblyGenericType(list[i], out type2))
							{
								return false;
							}
							array[i] = type2;
						}
						Type type3;
						if (TryGetMixedAssemblyGenericType(match.Groups["MainType"].Value, out type3))
						{
							type = type3.MakeGenericType(array);
						}
					}
				}
				catch (Exception ex)
				{
					Debug.LogError(string.Concat(new object[]
					{
						"Error in TryGetMixedAssemblyGenericType with typeName=",
						typeName,
						": ",
						ex
					}));
				}
			}
			return type != null;
		}
    public static Type GetTypeInAnyAssemblyRaw(string typeName)
    {
        Type type = SimpleGetType(typeName);
        if(type != null)
        {
            return type;
        }

        foreach(Assembly assembly in AllActiveAssemblies)
        {
            Type type2 = assembly.GetType(typeName, false, true);
            if (type2 != null)
            {
                return type2;
            }
        }
        Type type3 = Type.GetType(typeName, false, true);
        if (type3 != null)
        {
            return type3;
        }
        return null;
    }

    public static Type SimpleGetType(string typeName)
    {
        switch (typeName)
        {
            case "System.String":
                return typeof(string);
            case "System.Int32":
                return typeof(int);
            case "System.Boolean":
                return typeof(bool);
            case "System.Single":
                return typeof(float);
            case "System.Double":
                return typeof(double);
            case "System.Char":
                return typeof(char);
            case "System.Byte":
                return typeof(byte);
            case "System.SByte":
                return typeof(sbyte);
            case "System.Int16":
                return typeof(short);
            case "System.UInt16":
                return typeof(ushort);
            case "System.UInt32":
                return typeof(uint);
            case "System.Int64":
                return typeof(long);
            case "System.UInt64":
                return typeof(ulong);
            case "System.Decimal":
                return typeof(decimal);
            
            case "string":
                return typeof(string);
            case "int":
                return typeof(int);
            case "bool":
                return typeof(bool);
            case "float":
                return typeof(float);
            case "double":
                return typeof(double);
            case "char":
                return typeof(char);
            case "byte":
                return typeof(byte);
            case "sbyte":
                return typeof(sbyte);
            case "short":
                return typeof(short);
            case "ushort":
                return typeof(ushort);
            case "uint":
                return typeof(uint);
            case "long":
                return typeof(long);
            case "ulong":
                return typeof(ulong);
            case "decimal":
                return typeof(decimal);
            
            case "byte?":
                return typeof(byte?);
            case "sbyte?":
                return typeof(sbyte?);
            case "short?":
                return typeof(short?);
            case "ushort?":
                return typeof(ushort?);
            case "int?":
                return typeof(int?);
            case "uint?":
                return typeof(uint?);
            case "long?":
                return typeof(long?);
            case "ulong?":
                return typeof(ulong?);
            case "float?":
                return typeof(float?);
            case "double?":
                return typeof(double?);
            case "decimal?":
                return typeof(decimal?);
            case "char?":
                return typeof(char?);
            case "bool?":
                return typeof(bool?);
        }
        return null;
    }

    private static IEnumerable<Assembly> AllActiveAssemblies
    {
        get
        {
            yield return Assembly.GetExecutingAssembly();
            foreach (ModContentPack mod in ModEngineLoader.modsLoaded)
            {
                int num;
                for (int i = 0; i < mod.modAssembly.loadedAssemblies.Count; i = num + 1)
                {
                    yield return mod.modAssembly.loadedAssemblies[i];
                    num = i;
                }
            }
            yield break;
        }
    }

    public static void ClearCache()
    {
        cachedSubclasses.Clear();
        cachedSubclassesNonAbstract.Clear();
        AllTypes = null;
    }
    
    public static List<Type> AllTypes;
    public static Dictionary<string, Type> typeCache = new Dictionary<string, Type>();
    private static Dictionary<Type, List<Type>> cachedSubclasses = new Dictionary<Type, List<Type>>();
	private static Dictionary<Type, List<Type>> cachedSubclassesNonAbstract = new Dictionary<Type, List<Type>>();
    public static readonly List<string> IgnoredNamespaceNames = new List<string>
    {
        "System"
    };
}
