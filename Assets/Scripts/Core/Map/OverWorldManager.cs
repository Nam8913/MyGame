using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public static class OverWorldManager
{
    private static Dictionary<Vector2Int, Chunk> loadedChunks = new Dictionary<Vector2Int, Chunk>();
    public static void CreateWorld(OverworldData data,string seed)
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

    public static IEnumerator UpdateChunksAsync(Vector2Int playerChunkPos,GameObject prefab)
    {
        List<Vector2Int> coords = GetSpiralCoords(playerChunkPos, OverworldData.cachedSize);
        foreach (var pos in coords)
        {
            if (!loadedChunks.ContainsKey(pos))
            {
                LoadOrGenerateChunk(pos,prefab);
                yield return new WaitForSeconds(0.1f); // wait for 0.1 second to avoid freezing the main thread
            }
        }
        yield return null;
    }

    private static List<Vector2Int> GetSpiralCoords(Vector2Int center, int renderDistance)
    {
        List<Vector2Int> coords = new List<Vector2Int>();
        int x = 0, y = 0;
        int dx = 0, dy = -1;
        for (int i = 0; i < renderDistance * renderDistance; i++)
        {
            if ((-renderDistance / 2 <= x && x <= renderDistance / 2) && (-renderDistance / 2 <= y && y <= renderDistance / 2))
            {
                coords.Add(new Vector2Int(center.x + x, center.y + y));
            }
            if (x == y || (x < 0 && x == -y) || (x > 0 && x == 1 - y))
            {
                int temp = dx;
                dx = -dy;
                dy = temp;
            }
            x += dx;
            y += dy;
        }
        return coords;
    }

    public static void UpdateChunks(Vector2Int playerChunkPos,GameObject prefab)
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

        
    }

    public static void UnloadChunk(Vector2Int playerChunkPos)
    {
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

    private static void LoadOrGenerateChunk(Vector2Int pos,GameObject prefab)
    {
        Chunk chunk;
        chunk = GameObject.FindObjectsByType<Chunk>(FindObjectsSortMode.None).Where(x => x.name == $"Chunk:{pos.x}_{pos.y}").FirstOrDefault();
        if(chunk != null)
        {
            loadedChunks[pos] = chunk;
            if(chunk.gameObject.activeSelf)
            {
                return;
            }
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

    public static Chunk GetChunkAt(Vector2Int pos)
    {
        loadedChunks.TryGetValue(pos, out Chunk chunk);
        if (chunk != null)
        {
            return chunk;
        }
        chunk = GameObject.FindObjectsByType<Chunk>(FindObjectsSortMode.None).Where(x => x.name == $"Chunk:{pos.x}_{pos.y}").FirstOrDefault();
        if(chunk != null)
        {
            return chunk;
        }
        return null;
    }
    public static TileObj GetTileAt(Vector2Int pos)
    {
        Chunk chunk = GetChunkAt(Chunk.GetChunk(pos));
        if (chunk != null)
        {
            return chunk.GetTileAt(pos);
        }
        return null;
    }
}
