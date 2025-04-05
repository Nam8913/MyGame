using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using System.Xml.Serialization;

public static class ModEngineConfig
{
    static ModEngineConfig()
    {
    }
    public static void Init()
    {
        BuilderProcess.Init();
        string path = (Path.Combine(FoulderGen.ConfigDir, CONFIG_FILE_NAME));
        data = DirectXmlLoader.ProcessLoadFromXmlFile<ModConfigData>(path);
        TypePatch.PrintObjectInfo(data);
        if(data == null)
        {
            File.Delete(path);
            if(!File.Exists(path))
            {
                using (FileStream fs = File.Create(path))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(CONFIG_FILE_DEFAULT);
                    // Add some information to the file.
                    fs.Write(info, 0, info.Length);
                }

            }
            data = DirectXmlLoader.SimpleLoadFromXmlFile<ModConfigData>(path);
        }
    }
    public static ModConfigData data;
    public const string CONFIG_FILE_NAME = "ModConfig.xml";
    public const string CONFIG_FILE_DEFAULT = @"<?xml version=""1.0"" encoding=""utf-8""?>
<ModConfigData version=""0.0.1"">
    <activeMods>
        <li>Core_of_game</li>
    </activeMods>
</ModConfigData>";
    public class ModConfigData
    {
        public string version;
        public List<string> activeMods;
    }
}
