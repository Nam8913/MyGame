using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class OverWorldManager
{
    public OverworldData world;
    private Dictionary<Vector2Int, Chunk> loadedChunks = new Dictionary<Vector2Int, Chunk>();
    public void CreateWorld(OverworldData data,string seed)
    {
        int seedInt = OverworldData.GetHashSeed(seed);
        GenRandom.SetSeed((uint)seedInt);
        // OverworldData world = new OverworldData
        // {
        //     name = name,
        //     seed = seedInt,
        // };
        
        //CurrentGame.setWorld = world;
        //ALERT: Test
        CurrentGame.setWorld = data;
    }
    public void UpdateChunks(Vector2Int playerChunkPos,GameObject prefab)
    {
        // Load chunk quanh player
        for (int dx = -OverworldData.cachedSize; dx <= OverworldData.cachedSize; dx++)
        {
            for (int dy = -OverworldData.cachedSize; dy <= OverworldData.cachedSize; dy++)
            {
                Vector2Int pos = playerChunkPos + new Vector2Int(dx, dy);
                if (!loadedChunks.ContainsKey(pos))
                {
                    LoadOrGenerateChunk(pos,prefab);
                }
            }
        }

        // Unload chunk xa player
        List<Vector2Int> toRemove = new List<Vector2Int>();
        foreach (var kv in loadedChunks)
        {
            if (Vector2Int.Distance(kv.Key, playerChunkPos) > OverworldData.cachedSize)
            {
                kv.Value.SaveChunk();
                toRemove.Add(kv.Key);
            }
        }
        foreach (var pos in toRemove)
        {
            loadedChunks[pos].gameObject.SetActive(false);
            //GameObject.DestroyImmediate(loadedChunks[pos].gameObject);
            loadedChunks.Remove(pos);
        }
    }

    private void LoadOrGenerateChunk(Vector2Int pos,GameObject prefab)
    {
        Chunk chunk;
        chunk = GameObject.FindObjectsByType<Chunk>(FindObjectsSortMode.None).Where(x => x.name == $"Chunk:{pos.x}_{pos.y}").FirstOrDefault();
        if(chunk != null)
        {
            if(chunk.gameObject.activeSelf)
            {
                return;
            }
            loadedChunks[pos] = chunk;
            chunk.gameObject.SetActive(true);
            return;
        }
        chunk = new GameObject().AddComponent<Chunk>();;
        chunk.Init(pos);
        bool loaded = chunk.LoadChunk();
        if (!loaded)
        {
            Chunk.GenerateChunk(chunk,prefab);
        }

        loadedChunks[pos] = chunk;
    }
}
