using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
[XmlRoot("Infor")]
public class ModInfor
{
    [XmlElement("name")]
    public string name = "Unamed_Mod";
    [XmlElement("version")]
    public string version = "0.0.1";
    [XmlElement("description")]
    public string description = "";
    [XmlElement("author")]
    public string author = "Anonymous";
    [XmlElement("category")]
    public string category = "Other";

    [XmlElement("id_pack_for_mod")]
    public string id_pack_for_mod = "";

    [XmlIgnore]
    public DirectoryInfo directoryInfo;
    
    [XmlArray("supportedVersions")]
    [XmlArrayItem("li")]
    public List<string> supportedVersions;
    [XmlArray("loadBefore")]
        [XmlArrayItem("li")]
    public List<string> loadBefore = new List<string>();
    [XmlArray("loadAfter")]
        [XmlArrayItem("li")]
    public List<string> loadAfter = new List<string>();
    [XmlArray("incompatibleWith")]
        [XmlArrayItem("li")]
    public List<string> incompatibleWith = new List<string>();


    public const string modInforNameFile = "Infor.xml";
    public const string modExtraDllDir = "lib";
}
