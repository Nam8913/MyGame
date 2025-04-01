using System.IO;
using UnityEngine;

public class ModContentPack
{
    public ModInfor modInfor;
    public ModAssembly modAssembly;

    public string[] getAllFileExtraDll()
    {
        string path = modInfor.directoryInfo.FullName;
        string[] extra = System.IO.Directory.GetFiles(Path.Combine(path,ModInfor.modExtraDllDir),"*.dll",SearchOption.AllDirectories);
        return extra;
    }
}
