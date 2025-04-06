using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class OverWorldManager
{
    public OverworldData world;
    private Dictionary<Vector2Int, Chunk> loadedChunks = new Dictionary<Vector2Int, Chunk>();
    public OverworldData CreateWorld(string name, string seed)
    {
        int seedInt = OverworldData.GetHashSeed(seed);
        GenRandom.SetSeed((uint)seedInt);
        OverworldData world = new OverworldData
        {
            name = name,
            seed = seedInt,
        };
        return world;
    }
    public void UpdateChunks(Vector2 playerPos,GameObject prefab)
    {
        Vector2Int playerChunkPos = new Vector2Int(
            Mathf.FloorToInt(playerPos.x / OverworldData.chunkSize),
            Mathf.FloorToInt(playerPos.y / OverworldData.chunkSize)
        );

        // Load chunk quanh player
        for (int dx = -OverworldData.cachedSize; dx <= OverworldData.cachedSize; dx++)
        {
            for (int dy = -OverworldData.cachedSize; dy <= OverworldData.cachedSize; dy++)
            {
                Vector2Int pos = playerChunkPos + new Vector2Int(dx, dy);
                if (!loadedChunks.ContainsKey(pos))
                {
                    LoadOrGenerateChunkAsync(pos,prefab);
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
            GameObject.DestroyImmediate(loadedChunks[pos].gameObject);
            loadedChunks.Remove(pos);
        }
    }

    private void LoadOrGenerateChunkAsync(Vector2Int pos,GameObject prefab)
    {
        Chunk chunk = new GameObject().AddComponent<Chunk>();;
        chunk.Init(pos);
        bool loaded = chunk.LoadChunk();
        if (!loaded)
        {
            Chunk.GenerateChunk(chunk,prefab);
        }

        loadedChunks[pos] = chunk;
    }
}
