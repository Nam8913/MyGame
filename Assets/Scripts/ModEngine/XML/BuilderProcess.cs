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
                if (prc.node == null) return false;

                // Check if node has type attribute with value "List"
                if (prc.node.Attributes?["type"] != null)
                {
                    return prc.node.Attributes["type"].Value == "List";
                }
                
                // Check if node has child nodes named "li"
                if (prc.node.HasChildNodes)
                {
                    foreach (XmlNode child in prc.node.ChildNodes)
                    {
                        if (child.Name == "li")
                        {
                            return true;
                        }
                    }
                }
                
                return false;
            }).Make((BuilderProcessDeserialize prc) =>
            {
                object list = Activator.CreateInstance(Otype);
                MethodInfo addMethod = Otype.GetMethod("Add");
                foreach(XmlNode nodeValue in nodeHolder.ChildNodes)
                {
                    if(nodeValue.Name == "li")
                    {
                        if(prc.parent != null)
                        {
                            if(!string.IsNullOrEmpty(nodeValue.InnerText))
                            {
                                addMethod.Invoke(list,new object[]{nodeValue.InnerText});
                            }else if(nodeValue.Attributes.Count != 0)
                            {
                                if(nodeValue.Attributes.Count != 1)
                                {
                                    Debug.LogError(string.Concat("Invalid child node attributes, expected 1 attribute, but got: "
                                    ,nodeValue.Attributes.Count
                                    , "and by default the node will take the first attributes value \n"
                                    ,nodeValue.Name
                                    ,": "
                                    ,nodeValue.Attributes[0].Value));
                                }
                                addMethod.Invoke(list,new object[]{nodeValue.Attributes[0].Value});
                            }
                        }else
                        {
                            Debug.LogError("Parent is null, cannot add dataFields.");
                            return;
                        }
                    }else
                    {
                        Debug.LogError("Invalid child node type, expected 'li', but got: " + nodeValue.Name);
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
                        Debug.LogError("Error processing child node: " + ex.Message);
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
                        if(!Database.DatabaseDic.ContainsKey(key))
                        {
                            Database.AddData(key, (Data)obj);
                        }else
                        {
                            Database.RemoveData(key);
                            Database.AddData(key, (Data)obj);
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
                            object key = Convert.ChangeType(keyNode.InnerText, Otype.GetGenericArguments()[0]);
                            object value = Convert.ChangeType(valueNode.InnerText, Otype.GetGenericArguments()[1]);
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

   /// <summary>
    /// Initializes the BuilderProcess system
    /// </summary>
    public static void Init()
    {
        // This method exists for backwards compatibility
        // The static constructor already initializes everything
    }

}
