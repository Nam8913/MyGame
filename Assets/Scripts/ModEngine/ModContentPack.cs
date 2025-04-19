using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class ModContentPack
{
    public int LoadOrder;
    public ModInfor modInfor;
    public ModAssembly modAssembly;
    public DirectoryInfo directoryInfo;

    public Dictionary<string,Texture2D> textureAssets;
    public Dictionary<string,string> stringAssets;
    public Dictionary<string,AudioClip> audioAssets;

    public string GetRoot
    {
        get{return directoryInfo.FullName;}
    }
    public string GetModName
    {
        get{return modInfor.name;}
    }
    public string PackID
    {
        get{return modInfor.id_pack_for_mod;}
    }

    public string[] getAllFileExtraDll()
    {
        return System.IO.Directory.GetFiles(Path.Combine(directoryInfo.FullName,ModInfor.modExtraDllDir),"*.dll",SearchOption.AllDirectories);
    }
    public IEnumerable<string> getTextureFiles()
    {
        return Directory.EnumerateFiles(Path.Combine(directoryInfo.FullName,ModInfor.modTextureDir), "*", SearchOption.AllDirectories)
        .Where(file => ModInfor.TextureExtensions.Contains(Path.GetExtension(file)));
    }
    public IEnumerable<string> getSoundFiles()
    {
        return Directory.EnumerateFiles(Path.Combine(directoryInfo.FullName,ModInfor.modSoundDir), "*", SearchOption.AllDirectories)
        .Where(file => ModInfor.AudioClipExtensions.Contains(Path.GetExtension(file)));
    }
    public IEnumerable<string> GetStringFiles()
    {
        return Directory.EnumerateFiles(directoryInfo.FullName, "*", SearchOption.AllDirectories)
        .Where(file => ModInfor.AcceptableExtensionsString.Contains(Path.GetExtension(file)));
    }
    public IEnumerable<string> GetDataXmlFiles()
    {
        string path = Path.Combine(directoryInfo.FullName,ModInfor.modDataDir);
        if(Directory.Exists(path))
        {
            return Directory.EnumerateFiles(path, "*.xml", SearchOption.AllDirectories);
        }
        
        return null;
    }

    public void Init(int LoadOrder,string path)
    {
        this.LoadOrder = LoadOrder;
        directoryInfo = new DirectoryInfo(Path.GetDirectoryName(path));
        modAssembly = new ModAssembly(this);
    }
    
    public void LoadModContent()
    {
        textureAssets = new Dictionary<string,Texture2D>();
        stringAssets = new Dictionary<string,string>();
        //audioAssets = new Dictionary<string,AudioClip>();

        textureAssets = getTextureFiles().ToDictionary(
            file => file.Replace(Path.Combine(directoryInfo.FullName, ModInfor.modTextureDir + "\\"), ""),
            file => ModEngineLoader.LoadItem<Texture2D>(file)
        );

        foreach (var texture in textureAssets)
        {
            TextureStorage.GetDatabase.AddTexture(texture.Key, texture.Value);
        }

        stringAssets = GetStringFiles().ToDictionary(
            file => file,
            file =>  ModEngineLoader.LoadItem<string>(file)
        );
        //this.modAssembly.LoadAssembly();
    }
    public List<FileInfo> LoadModXmlData()
    {
        return GetDataXmlFiles().Select(file => new FileInfo(file)).ToList();
    }
    
    
}
