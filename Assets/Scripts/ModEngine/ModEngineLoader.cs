using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using JetBrains.Annotations;

public static class ModEngineLoader
{
    public static List<ModContentPack> modsLoaded = new List<ModContentPack>();

    // public static void LoadModProcess()
    // {
        
         
    //     //Find all mods foulder in data directory
    //     string[] folders = System.IO.Directory.GetDirectories(FoulderGen.modsDir,"*", System.IO.SearchOption.TopDirectoryOnly);
    //     Debug.LogWarning("" + folders.Count());
    //     foreach(string folder in folders)
    //     {
    //         Debug.LogWarning(Path.Combine(folder,ModInfor.modInforNameFile));
    //         var item = CreateModDataContent(Path.Combine(folder,ModInfor.modInforNameFile));
           
    //     }
    // }
    
    // public static ModContentPack CreateModDataContent(string path)
    // {
    //     ModContentPack mod = new ModContentPack();
    //     if(File.Exists(path))
    //     {
    //         mod.modInfor = DirectXmlLoader.LoadFromXmlFile<ModInfor>(path);
    //     }
        
    //     TypePatch.PrintObjectInfo(mod.modInfor);
    //     return mod;
    // }
}
