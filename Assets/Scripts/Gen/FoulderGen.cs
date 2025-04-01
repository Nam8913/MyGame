using System.IO;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public static class FoulderGen
{
    static FoulderGen()
    {
        string dir;
        DirectoryInfo dirInfor = new DirectoryInfo(Application.dataPath);
        if(Application.isEditor)
        {
            dir = Path.Combine(dirInfor.ToString(), "Data");
        }else if(Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
        {
            string path = Path.Combine(Directory.GetParent(Application.persistentDataPath).ToString(), "Core");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            dir = path;
        }else
        {
            dir = Path.Combine(dirInfor.ToString(), "Data");
        }
        dataDir = dir;
        DirectoryInfo directoryInfo2 = new DirectoryInfo(dir);
        if (!directoryInfo2.Exists)
        {
            directoryInfo2.Create();
        }
    }
    public static readonly string dataDir;

    public static string FoulderInDataDir(string directory)
    {
        string text = Path.Combine(dataDir, directory);
        DirectoryInfo directoryInfo = new DirectoryInfo(text);
        if (!directoryInfo.Exists)
        {
            directoryInfo.Create();
        }
        return text;
    }

    public static string modsDir => FoulderInDataDir(modsDirName);
    public static string ConfigDir => FoulderInDataDir(configDirName);

    public const string modsDirName = "Mods";
    public const string configDirName = "Config";
    public const string textureDirName = "Textures";
    public const string materialDirName = "Materials";
    public const string soundDirName = "Sounds";
    public const string localizationDirName = "Languages";
}
