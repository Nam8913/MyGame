using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using JetBrains.Annotations;
using System.Xml;

public static class ModEngineLoader
{
    public static List<ModContentPack> modActive = new List<ModContentPack>();

    public static void LoadModProcess()
    {
        LoadModsFromConfig();

        LoadModsContent();

        var list = GenTypes.GetAllTypeData();
        foreach(var item in list)
        {
            Debug.LogWarning(item.FullName);
        }

        List<FileInfo> files = LoadModsData();

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc = DirectXmlLoader.CombineIntoUnifiedXML(files);
        Debug.LogWarning(xmlDoc.OuterXml.ToString());
        DirectXmlLoader.LoadUnifiedXML(xmlDoc,list);
    }
    
    private static void CheckAndCreateContentForActiveMod(string path , ref int count)
    {
        if(File.Exists(path))
        {
            ModContentPack mod = new ModContentPack();
            mod.modInfor = DirectXmlLoader.ProcessLoadFromXmlFile<ModInfor>(path);
            if(ModEngineConfig.data.activeMods.Contains(mod.modInfor.id_pack_for_mod))
            {
                count++;
                modActive.Add(mod);
                mod.Init(count,path);
                Debug.LogWarning("Mod " + mod.modInfor.name + " loaded");
            }else
            {
                return;
            }
        }
    }
    private static void LoadModsFromConfig()
    {
        int num = 0;
        //Find all mods foulder in data directory
        string[] folders = System.IO.Directory.GetDirectories(FoulderGen.modsDir,"*", System.IO.SearchOption.TopDirectoryOnly);
        foreach(string folder in folders)
        {
           CheckAndCreateContentForActiveMod(Path.Combine(folder,ModInfor.modInforNameFile),ref num);
        }
    }
    private static void LoadModsContent()
    {
        foreach (var mod in modActive)
        {
            try
            {
                mod.LoadModContent();
            }
            catch (System.Exception ex)
            {
                Debug.LogError(string.Concat(new object[]
                {
                    "Could not reload mod content for mod ",
                    mod.GetModName,
                    ": ",
                    ex
                }));
            }
        }  
    }

    private static List<FileInfo> LoadModsData()
    {
        List<FileInfo> files = new List<FileInfo>();
        foreach (var mod in modActive)
        {
            try
            {
                files.AddRange(mod.LoadModXmlData());
            }
            catch (System.Exception ex)
            {
                Debug.LogError(string.Concat(new object[]
                {
                    "Could not load mod data xml for mod ",
                    mod.GetModName,
                    ": ",
                    ex
                }));
            }
        }
        return files;
    }

    public static T LoadItem<T>(string path) where T : class
    {
        try
        {
            if(typeof(T) == typeof(string))
            {
                return (T)((object)File.ReadAllText(path));
            }
            if(typeof(T) == typeof(Texture2D) || (typeof(T) == typeof(Sprite)))
            {
                return (T)((object)LoadTexture(new FileInfo(path)));
            }
            if(typeof(T) == typeof(AudioClip))
            {
                Debug.LogError("AudioClip is not supported yet");
                return null;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError(string.Concat(new object[]
            {
                "Exception loading ",
                typeof(T),
                " from file.\nabsFilePath: ",
                path,
                "\nException: ",
                ex.ToString()
            }));
        }
        return null;
    }

    private static Texture2D LoadTexture(FileInfo file)
    {
        Texture2D texture2D = null;
        if (file.Exists)
        {
            byte[] data = File.ReadAllBytes(file.FullName);
            texture2D = new Texture2D(2, 2, TextureFormat.Alpha8, true);
            texture2D.LoadImage(data);
            texture2D.name = Path.GetFileNameWithoutExtension(file.Name);
            texture2D.filterMode = FilterMode.Trilinear;
            texture2D.anisoLevel = 2;
            texture2D.Apply(true, true);
        }
        return texture2D;
    }
}
