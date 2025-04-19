using System;
using System.Collections.Generic;
using UnityEngine;

public class TextureStorage : Database<TextureStorage>
{
    private Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
    [SerializeField]
    private List<string> showpath = new List<string>();
    public static TextureStorage GetDatabase
    {
        get { return Singleton<TextureStorage>.Instance; }
    }

    private void OnGUI()
    {
        if (showpath == null)
        {
            showpath = new List<string>();
        }
        else
        {
            showpath.Clear();
        }
        foreach (var texture in textures)
        {
            showpath.Add(texture.Key);
        }
    }

    public Texture2D GetTexture(string str)
    {
        Texture2D texture = Resources.Load<Texture2D>(str);
        if (texture == null)
        {
            textures.TryGetValue(str, out texture);
        }
        if (texture == null)
        {
            Debug.LogWarning($"Texture with key {str} not found.");
            return null;
        }
        return texture;
    }
    public List<Texture2D> GetAllTexturesInMod(string id)
    {
        List<Texture2D> textureList = new List<Texture2D>();
        foreach (var x in ModEngineLoader.GetModContentPack(id).textureAssets)
        {
            if (textures.TryGetValue(x.Key, out Texture2D texture))
            {
                textureList.Add(texture);
            }
            else
            {
                Debug.LogWarning($"Texture with key {x.Key} not found.");
            }
        }
        return textureList;
    }
    public Texture2D GetTextureInMod(string id_mod, string key)
    {
        Texture2D texture = null;
        if (ModEngineLoader.GetModContentPack(id_mod).textureAssets.TryGetValue(key, out texture))
        {
            return texture;
        }
        else
        {
            Debug.LogWarning($"Texture with key {key} not found in mod {id_mod}.");
            return null;
        }
    }

    public void AddTexture(string key, Texture2D texture)
    {
        if (!textures.ContainsKey(key))
        {
            textures.Add(key, texture);
        }
        else
        {
            // if (replaced)
            // {
            //     textures[key] = texture;
            // }
            // else
            {
                Debug.LogWarning($"Texture with key {key} already exists.");
            }
        }
    }

    public override void Dispose()
    {
        textures.Clear();
    }

}
