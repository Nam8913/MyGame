using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
public class ModInfor
{
    public string name = "Unamed_Mod";
    public string version = "0.0.1";
    public string description = "";
    public string author = "Anonymous";
    public string category = "Other";
    public string id_pack_for_mod = "";
    public List<string> supportedVersions;
    public List<string> loadBefore = new List<string>();
    public List<string> loadAfter = new List<string>();
    public List<string> incompatibleWith = new List<string>();


    public const string modInforNameFile = "Infor.xml";
    public const string modExtraDllDir = "lib";
    public const string modDataDir = "data";
    public const string modTextureDir = "textures";
    public const string modSoundDir = "sounds";
    public const string modLocalizationDir = "languages";
    public const string modConfigDir = "config";

    public static readonly string[] TextureExtensions = new string[]
    {
        ".png",
        ".psd",
        ".jpg",
        ".jpeg"
    };
    public static readonly string[] AudioClipExtensions = new string[]
    {
        ".wav",
        ".mp3",
        ".ogg",
        ".xm",
        ".it",
        ".mod",
        ".s3m",
        ".mp4"

    };
    public static string[] AcceptableExtensionsString = new string[]
    {
        ".txt"
    };
}
