using System;
using System.IO;
using System.Xml;
using UnityEngine;
using System.Xml.Serialization;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
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
        Type typeInAnyAssembly = GenTypes.GetTypeInAnyAssembly(xmlAttribute.Value, typeof(T).Namespace);
        if (typeInAnyAssembly == null)
        {
            Debug.LogError("Could not find type named " + xmlAttribute.Value + " from node " + xmlRoot.OuterXml);
            return typeof(T);
        }
        return typeInAnyAssembly;
    }

    public static XmlDocument CombineIntoUnifiedXML(List<FileInfo> files)
    {
        XmlDocument unifiedXml = new XmlDocument();
        XmlNode root = unifiedXml.CreateElement("Data");
        unifiedXml.AppendChild(root);
        XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
        xmlReaderSettings.IgnoreComments = true;
        xmlReaderSettings.CheckCharacters = false;
        foreach (FileInfo file in files)
        {
            using (StringReader stringReader = new StringReader(File.ReadAllText(file.FullName)))
            {
                using (XmlReader xmlReader = XmlReader.Create(stringReader, xmlReaderSettings))
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(xmlReader);

                    if(xmlDoc.DocumentElement.Name != "Data")
                    {
                        Debug.LogError(" root element named " + xmlDoc.DocumentElement.Name + "in file: "+ file.FullName +" should be named Data");
                        //continue;
                    }
                    foreach (XmlNode item in xmlDoc.DocumentElement.ChildNodes)
                    {
                        XmlNode node = item;
                        XmlNode fileNode = unifiedXml.ImportNode(node, true);
                        root.AppendChild(fileNode);
                    }
                }
            }
            
            
        }
        return unifiedXml;
    }

    public static void LoadUnifiedXML(XmlDocument unified , List<System.Type> dataTypes)
    {
        BuilderProcessDeserialize builder = BuilderProcess.ProcessOrder["data"](unified.DocumentElement , typeof(Data));
        builder.Invoke();
        if(builder != null)
        {
            if(builder.HasChildren())
            {
                foreach(var obj in builder.children)
                {
                    if(obj != null)
                    {
                        TypePatch.PrintObjectInfo(obj.dataFields.First().Value);
                    }else
                    {
                        Debug.LogError("Object is null " + obj.GetType().ToString());
                    }
                }
            }
        }else
        {
            Debug.LogError("Unified XML document is not valid");
        }
        BuilderProcess.TryToFinishOrderForDataUnfinishOrMissing();
        BuilderProcess.ClearUnfinishDataOrderProcess();
    }
}
