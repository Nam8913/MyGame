using System;
using System.IO;
using System.Xml;
using UnityEngine;
using System.Xml.Serialization;
using System.Linq;
public static class DirectXmlLoader
{
    public static T SimpleLoadFromXmlFile<T>(string filePath) where T : new()
    {
        T rs = default(T);
        if (!new FileInfo(filePath).Exists)
        {
            return Activator.CreateInstance<T>();
        }
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        using (Stream reader = new FileStream(filePath, FileMode.Open))
        {
            // Call the Deserialize method to restore the object's state.
            rs = (T)serializer.Deserialize(reader);
        }
        return rs;
    }

     

    public static T ProcessLoadFromXmlFile<T>(string filePath) where T : new()
    {
        T rs = default(T);
        string xmlContent = "";
        if (!new FileInfo(filePath).Exists)
        {
            return Activator.CreateInstance<T>();
        }else
        {
            xmlContent = File.ReadAllText(filePath);
            Debug.LogWarning(xmlContent);
        }
        try
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xmlContent);
            T t = ObjectFromXml<T>(xmlDocument.DocumentElement);
            rs = t;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Exception loading file at " + filePath + ". Loading defaults instead. Exception was: " + ex.ToString());
			rs = Activator.CreateInstance<T>();
        }
        return rs;
    }

    public static T ObjectFromXml<T>(XmlNode xmlRoot)
    {
        T rs = default(T);
        try
        {
            BuilderProcessDeserialize builder = BuilderProcess.GetBuilderForType(typeof(T),xmlRoot);
            builder.Invoke();
            rs = (T)builder.dataFields.Values.First();
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex);
        }
        return rs;
    }
    private static Type ClassTypeOf<T>(XmlNode xmlRoot)
    {
        XmlAttribute xmlAttribute = xmlRoot.Attributes["class"];
        if (xmlAttribute == null)
        {
            return typeof(T);
        }
        Type typeInAnyAssembly = TypePatch.GetTypeInAnyAssembly(xmlAttribute.Value, typeof(T).Namespace);
        if (typeInAnyAssembly == null)
        {
            Debug.LogError("Could not find type named " + xmlAttribute.Value + " from node " + xmlRoot.OuterXml);
            return typeof(T);
        }
        return typeInAnyAssembly;
    }
}
