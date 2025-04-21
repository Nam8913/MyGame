using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using Mono.Cecil.Cil;
using Unity.VisualScripting;
using UnityEngine;

public class BuilderProcessDeserialize
{
    public List<BuilderProcessDeserialize> children;
    public BuilderProcessDeserialize parent;
    public XmlNode node;
    public string type;
    public Action<BuilderProcessDeserialize> actionDeferredProcess;
    public Dictionary<string,object> dataFields = new Dictionary<string,object>();

    /// <summary>
    /// Validates that the XML node has the expected name
    /// </summary>
    public BuilderProcessDeserialize checkValidRootName(string name,bool continueOnError = false)
    {
        if (node == null)
        {
            Debug.LogError($"Cannot validate root name: node is null for type: {type}");
            return continueOnError ? this : null;
        }

        if (node.Name != name && name != "all")
        {
            Debug.LogError($"Root node name is not valid. Expected: {name}, Actual: {node.Name}, Type: {type}");
            return continueOnError ? this : null;
        }
        return this;
    }
    /// <summary>
    /// Executes the deferred process action
    /// </summary>
    public void Invoke()
    {
        actionDeferredProcess?.Invoke(this);
    }
    /// <summary>
    /// Sets the XML node to process
    /// </summary>
    public BuilderProcessDeserialize createProcess(XmlNode node)
    {
        this.node = node;
        return this;
    }
    /// <summary>
    /// Asynchronously sets the XML node to process
    /// </summary>
    public async Task<BuilderProcessDeserialize> CreateProcessAsync(XmlNode node)
    {
        await Task.Run(() => { this.node = node; });
        return this;
    }
    public BuilderProcessDeserialize Make(Action<BuilderProcessDeserialize> action)
    {
        actionDeferredProcess += action;
        return this;
    }

    public BuilderProcessDeserialize AddChild(BuilderProcessDeserialize child)
    {
        if (child == null)
        {
            Debug.LogWarning("Attempted to add null child to builder");
            return this;
        }

        children ??= new List<BuilderProcessDeserialize>();
        child.parent = this;
        children.Add(child);
        return this;
    }
    public BuilderProcessDeserialize AddParent(BuilderProcessDeserialize parent)
    {
        if (parent == null)
        {
            Debug.LogWarning("Attempted to add null parent to builder");
            return this;
        }

        this.parent = parent;
        parent.AddChild(this);
        return this;
    }
    public BuilderProcessDeserialize CheckBool(Func<BuilderProcessDeserialize,bool> func,bool continueOnError = false)
    {
        if (func == null)
        {
            Debug.LogError("CheckBool received null function");
            return continueOnError ? this : null;
        }

        try
        {
            bool flag = func.Invoke(this);
            if (!flag)
            {
                Debug.LogError($"Check bool failed for node: {(node != null ? node.Name : "null")}");
                return continueOnError ? this : null;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Exception in CheckBool for node {(node != null ? node.Name : "null")}: {ex.Message}");
            return continueOnError ? this : null;
        }
        return this;
    }

    /// <summary>
    /// Checks if this builder is a root node
    /// </summary>
    public bool IsRoot() => parent == null;

    /// <summary>
    /// Checks if this builder has children
    /// </summary>
    public bool HasChildren() => children != null && children.Count > 0;
}

public static class BuilderProcess
{
    public static Dictionary<string,Func<XmlNode,System.Type,BuilderProcessDeserialize>> ProcessOrder = new Dictionary<string,Func<XmlNode,System.Type,BuilderProcessDeserialize>>();

    static BuilderProcess()
    {
        ProcessOrder.Clear();

        ProcessOrder.Add("List",(XmlNode nodeHolder ,System.Type Otype)=>
        {
            BuilderProcessDeserialize builder = new BuilderProcessDeserialize()
            {
                type = "List"
            }.createProcess(nodeHolder)
            .CheckBool((BuilderProcessDeserialize prc) => {
                // Check if node has type attribute with value "List"
                if (prc.node.Attributes?["type"] != null)
                {
                    return prc.node.Attributes["type"].Value == "List";
                }
                // Check if node has child nodes named "li"
                return prc.node.ChildNodes.Cast<XmlNode>().Any(child => child.Name == "li");
            }).Make((BuilderProcessDeserialize prc) =>
            {
                object list = Activator.CreateInstance(Otype);
                MethodInfo addMethod = Otype.GetMethod("Add");
                foreach(XmlNode nodeValue in nodeHolder.ChildNodes)
                {
                    GetStringValue(nodeValue, out string str);
                    if(CanSimpleParser(Otype.GetGenericArguments()[0]))
                    {
                        if(nodeValue.Name == "li")
                        {
                            if(prc.parent != null)
                            {

                                var value = TypePatch.HelpParseType(str, Otype.GetGenericArguments()[0]);
                                addMethod.Invoke(list, new object[] { value });
                            
                            }else
                            {
                                Debug.LogError("Parent is null, cannot add dataFields.");
                                return;
                            }
                        }else
                        {
                            //make exception for node with only 1 child node that could be without <li> node
                            if(nodeHolder.ChildNodes.Count == 1)
                            {
                                if(prc.parent != null)
                                {
                                    var value = TypePatch.HelpParseType(str, Otype.GetGenericArguments()[0]);
                                    addMethod.Invoke(list, new object[] { value });
                                }else
                                {
                                    Debug.LogError("Parent is null, cannot add dataFields.");
                                    return;
                                }
                            }

                            Debug.LogError("Invalid child node type, expected 'li', but got: " + nodeValue.Name);
                        }
                    }else
                    {
                        if(CanOrderProcess(Otype.GetGenericArguments()[0]))
                        {
                            BuilderProcessDeserialize builder;
                            // Determine the correct node to process
                            builder = nodeValue.Name != "li"
                                ? GetBuilderForType(Otype.GetGenericArguments()[0], nodeValue)
                                : GetBuilderForType(Otype.GetGenericArguments()[0], nodeValue.FirstChild);
                           
                            prc.AddChild(builder);
                            builder.Invoke();
                            var value = builder.dataFields.Values.FirstOrDefault();
                            addMethod.Invoke(list, new object[] { value });
                        }else
                        {
                            Debug.LogError("Invalid child node type, expected 'li', but got: " + nodeValue.Name);
                        }
                    }
                    
                }

                if(prc.parent != null)
                {
                    var parent =  prc.parent.dataFields.Values.First();
                    parent.GetType().GetField(nodeHolder.Name).SetValue(parent,list);
                }else
                {
                    Debug.LogError("Parent is null, cannot add list data.");
                    return;
                }
                
            });
            return builder;
        });

        ProcessOrder.Add("class",(XmlNode rootNode,System.Type Otype) => {
            return new BuilderProcessDeserialize()
            {
                type = "class"
            }
            .createProcess(rootNode)
            .Make((BuilderProcessDeserialize prc) => {
                XmlNode node = prc.node;
                object obj = Activator.CreateInstance(Otype);
                prc.dataFields.Add(obj.GetType().ToString(),obj);
                foreach(XmlNode child in node.ChildNodes)
                {
                    try
                    {
                        FieldInfo field = obj.GetType().GetField(child.Name);
                        if(field != null)
                        {
                            try
                            {
                                if(field.GetValue(obj) == null && (TypePatch.IsList(field.FieldType) || TypePatch.IsDictionary(field.FieldType)))
                                {
                                    object instance = Activator.CreateInstance(field.FieldType);
                                    field.SetValue(obj, instance);
                                }
                            }
                            catch (System.Exception ex)
                            {
                                Debug.LogError($"Failed to create default constructor for {field.Name} \n {ex.Message}");
                            }


                            string str = child.InnerText;
                            if (string.IsNullOrEmpty(str))
                            {
                                str = child.Attributes["value"]?.Value;
                            }
                            if(CanSimpleParser(field.FieldType))
                            {
                                var value = TypePatch.HelpParseType(str, field.FieldType);
                                obj.GetType().GetField(child.Name).SetValue(obj, value);
                            }else
                            {
                                if(CanOrderProcess(field.FieldType))
                                {
                                    BuilderProcessDeserialize builder = GetBuilderForType(field.FieldType,child);
                                    prc.AddChild(builder);
                                    builder.Invoke();
                                }
                            }
                        }else
                        {
                            Debug.LogError("Invalid name node:" + child.Name + " with value [" + child.OuterXml + "] for get field in type object: " + obj.GetType().FullName);
                            continue;
                        }
                        
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError($"Error processing child node '{child.Name}' in type '{obj.GetType().FullName}': {ex.Message}");
                        continue;
                    }
                }

                foreach (XmlAttribute attr in node.Attributes)
                {
                    FieldInfo field = obj.GetType().GetField(attr.Name);
                    if(field != null)
                    {
                        var value = TypePatch.HelpParseType(attr.Value, field.FieldType);
                        var old = field.GetValue(obj);
                        bool flag = old != null ? true : false;
                        if(flag)
                        {
                            Debug.LogWarning(String.Concat(
                            "Variable: ",
                            attr.Name,
                            " already have value: ",
                            old,
                            ", but it's being set to a new value: ",
                            attr.Value,
                            " by attribute in node: ",
                            node.Name,
                            " in object:",
                            obj.GetType().FullName,
                            "\n in file: ",
                            node.OwnerDocument.BaseURI
                            ));
                        }
                        field.SetValue(obj, value);
                    }
                    
                }
               
                if(prc.parent != null)
                {
                    if(prc.parent.dataFields.Any())
                    {
                        var parent = prc.parent.dataFields.Values.First();
                        parent.GetType().GetField(node.Name).SetValue(parent,obj);
                    }
                }
                if(obj != null && GenTypes.IsData(obj.GetType()))
                {
                    
                    Data data = ((Data)obj);
                    string key = data.id;
                    if(!string.IsNullOrEmpty(key))
                    {
                        if(!DataStorage.DatabaseDic.ContainsKey(key))
                        {
                            DataStorage.AddData(key, (Data)obj);
                        }else
                        {
                            DataStorage.RemoveData(key);
                            DataStorage.AddData(key, (Data)obj);
                        }
                    }
                    
                }
            
            });
        });

        ProcessOrder.Add("Dictionary",(XmlNode nodeHolder ,System.Type Otype)=>
        {
            BuilderProcessDeserialize builder = new BuilderProcessDeserialize()
            {
                type = "Dictionary"
            }.createProcess(nodeHolder)
            .CheckBool(prc => prc.node.ChildNodes.Cast<XmlNode>().All(child => child.Name == "li"))
            .Make(prc =>
            {
                object dictionary = Activator.CreateInstance(Otype);
                MethodInfo addMethod = Otype.GetMethod("Add");
                foreach(XmlNode nodeValue in nodeHolder.ChildNodes)
                {
                    if(nodeValue.Name == "li")
                    {
                        XmlNode keyNode = nodeValue["key"];
                        XmlNode valueNode = nodeValue["value"];
                        if(keyNode != null && valueNode != null)
                        {
                            var key = TypePatch.HelpParseType(keyNode.InnerText, Otype.GetGenericArguments()[0]);
                            var value = TypePatch.HelpParseType(valueNode.InnerText, Otype.GetGenericArguments()[1]);
                            addMethod.Invoke(dictionary, new object[] { key, value });
                        }
                        else
                        {
                            Debug.LogError("Invalid dictionary entry, missing key or value");
                        }
                    }
                }
                if(prc.parent != null)
                {
                    var parent =  prc.parent.dataFields.Values.First();
                    parent.GetType().GetField(nodeHolder.Name).SetValue(parent, dictionary);
                }
            });
            return builder;
        });

        ProcessOrder.Add("data",(XmlNode rootNode,System.Type Otype) =>
        {
            return new BuilderProcessDeserialize()
            {
                type = "data"
            }.createProcess(rootNode)
            .Make( prc => {
                XmlNode node = prc.node;
                if(node.HasChildNodes)
                {
                    foreach(XmlNode childNode in node.ChildNodes)
                    {
                        BuilderProcessDeserialize builder = GetBuilderForType(GenTypes.GetTypeInAnyAssembly(childNode.Name,childNode.Attributes["namespace"]?.InnerText),childNode);
                        //BuilderProcessDeserialize builder = ProcessOrder["data"](childNode , GenTypes.GetTypeInAnyAssembly(childNode.Name,childNode.Attributes["namespace"]?.InnerText));
                        if(builder != null)
                        {
                            Debug.Log("Serializing " + childNode.Name + " to " + prc.type);
                            prc.AddChild(builder);
                            builder.Invoke();
                        }else
                        {
                            Debug.LogError($"No builder found for type {childNode.Name}");
                        }
                    }
                }
                
            });
        });
    }
    public static BuilderProcessDeserialize GetBuilderForType(System.Type type,XmlNode xmlNode)
    {
        if(type == null)
        {
            Debug.LogError("Type doesn't not exits for node: " + xmlNode.Name + " \n" + xmlNode.OuterXml);
            return null;
        }
        //string name = type.ToString().Split('+').Last();
        if(xmlNode.Attributes["class"] != null)
        {
            type = GenTypes.GetTypeInAnyAssembly(xmlNode.Attributes["class"].Value,xmlNode.Attributes["namespace"]?.InnerText);
        }
        string name = GetOrderWith(type);
       if (ProcessOrder.TryGetValue(name, out var processor))
        {
            try
            {
                return processor(xmlNode, type);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error creating builder for type {name}: {ex.Message}");
                return null;
            }
        }
        
        Debug.LogError($"Not found builder for {name}");
        return null;
    }

    public static bool CanOrderProcess(System.Type type)
    {
        string name = GetOrderWith(type);
        return ProcessOrder.ContainsKey(name);
    }
    public static bool CanSimpleParser(System.Type type)
    {
        bool rs = GenTypes.SimpleGetType(type.Name) != null;
        return rs;
    }
    private static string GetOrderWith(System.Type type)
    {
        string name = type.Name;
        
        if(type.IsClass && type != typeof(string))
        {
            name = "class";
        }
        if(TypePatch.IsList(type))
        {
            name = "List";
        }else if(TypePatch.IsDictionary(type))
        {
            name = "Dictionary";
        }
        return name;
    }

    private static void GetStringValue(XmlNode node, out string str)
    {
        str = node.InnerText;
        if (string.IsNullOrEmpty(str))
        {
            str = node.Attributes["value"]?.Value;   
        }
        if(string.IsNullOrEmpty(str))
        {
            if(node.Attributes.Count != 0)
            {
                if(node.Attributes.Count != 1)
                {
                    Debug.LogError(string.Concat("Invalid child node attributes, expected 1 attribute, but got: "
                    ,node.Attributes.Count
                    , "and by default the node will take the first attributes value \n"
                    ,node.Name
                    ,": "
                    ,node.Attributes[0].Value));
                }
                str = node.Attributes[0].Value;
            }
        }
        if(string.IsNullOrEmpty(str))
        {
            Debug.LogError("Invalid child node value, expected a value or attribute, but got: " + node.OuterXml);
            return;
        }
    }

   /// <summary>
    /// Initializes the BuilderProcess system
    /// </summary>
    public static void Init()
    {
        // This method exists for backwards compatibility
        // The static constructor already initializes everything
    }

}
