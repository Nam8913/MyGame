using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public float heightChunk;
    public Vector2Int worldPosition;
    Dictionary<Vector2Int, TileObj> tilesDict = new Dictionary<Vector2Int, TileObj>();

    public Vector2Int GetLocalPos
    {
        get
        {
            return new Vector2Int(worldPosition.x * OverworldData.chunkSize, worldPosition.y * OverworldData.chunkSize);
        }
    }

    public void Init(Vector2Int pos)
    {
        worldPosition = pos;
        tilesDict = new Dictionary<Vector2Int, TileObj>();
    }

    public void ClearChunk()
    {
        foreach (var tile in tilesDict)
        {
            Destroy(tile.Value.gameObject);
        }
        tilesDict.Clear();
        Destroy(gameObject);
    }
    public TileObj GetTileAt(Vector2Int pos)
    {
        if (tilesDict.TryGetValue(pos, out TileObj tile))
        {
            return tile;
        }else
        {
            Debug.LogError("Tile not found at position: " + pos + " in chunk: " + worldPosition);
        }
        return null;
    }
    public void AddTile(Vector2Int pos, TileObj tile)
    {
        if (tilesDict.ContainsKey(pos))
        {
            tilesDict[pos] = tile;
        }
        else
        {
            tilesDict.Add(pos, tile);
        }
    }
    public void RemoveTile(Vector2Int pos)
    {
        if (tilesDict.ContainsKey(pos))
        {
            tilesDict.Remove(pos);
        }
    }

    public static void GenerateChunk(Chunk chunk,GameObject prefab)
    {
        if(ChunkManager == null)
        {
            ChunkManager = new GameObject("ChunkManager");
        }
        chunk.transform.parent = ChunkManager.transform;
        Vector2Int localPos = chunk.GetLocalPos;
        int localX = localPos.x;
        int localY = localPos.y;
        chunk.name = "Chunk:" + chunk.worldPosition.x + "_" + chunk.worldPosition.y;
        chunk.transform.position = new Vector3(localX, localY, 0);
        //GameObject chunkObj = new GameObject("Chunk_" + chunk.worldPosition.x * OverworldData.chunkSize + "_" + chunk.worldPosition.y * OverworldData.chunkSize);
        
        OverworldData data = CurrentGame.getWorld;
        //float[,] heightMap = GenNoise.GenerateNoiseForChunk(chunk,data.scale,data.octaves, data.persistance, data.lacunarity, data.amplitude, data.frequency);
        
        for (int x = 0; x < OverworldData.chunkSize; x++)
        {
            for (int y = 0; y < OverworldData.chunkSize; y++)
            {
                Vector2Int pos = new Vector2Int(localX + x, localY + y);
                GameObject obj = new GameObject("Tile:" + pos.x + "-" + pos.y);
                obj.transform.position = new Vector3(pos.x,pos.y, 0);
                TileObj tile = obj.AddComponent<TileObj>();
                //tile.height = heightMap[x, y];
                tile.height = 0.9f;
                GameObject SpriteObj = Instantiate(prefab, new Vector3(pos.x, pos.y, 0.001f), Quaternion.identity);
                SpriteObj.transform.parent = obj.transform;
                obj.transform.parent = chunk.transform;

                SpriteRenderer rd = SpriteObj.GetComponent<SpriteRenderer>();
                //Find the layer with the same height as the tile
                //rd.color = CurrentGame.GetScenePlay.layers.Where(x => x.height == tile.height).First().color;
                rd.color = CurrentGame.GetScenePlay.layers.Where(x => tile.height >= x.height).Last().color;
                
                chunk.AddTile(new Vector2Int(pos.x, pos.y), tile);
            }
        }
        // chunk.heightChunk = 
        // (heightMap[0,0] + 
        // heightMap[OverworldData.chunkSize-1,0] + 
        // heightMap[0,OverworldData.chunkSize-1] + 
        // heightMap[OverworldData.chunkSize-1,OverworldData.chunkSize-1] + 
        // heightMap[(OverworldData.chunkSize-1)/2,0] + 
        // heightMap[0,(OverworldData.chunkSize-1)/2] + 
        // heightMap[(OverworldData.chunkSize-1)/2,OverworldData.chunkSize-1] + 
        // heightMap[OverworldData.chunkSize-1,(OverworldData.chunkSize-1)/2]) 
        // +heightMap[(OverworldData.chunkSize-1)/2,(OverworldData.chunkSize-1)/2]/ 8.5f;\
    }

    public void SaveChunk()
    {

    }
    public bool LoadChunk()
    {
        return false;
    }

    public static Vector2Int GetChunk(Vector2 pos)
    {
        return new Vector2Int(Mathf.FloorToInt(pos.x / OverworldData.chunkSize), Mathf.FloorToInt(pos.y / OverworldData.chunkSize));
    }

    public static GameObject ChunkManager;
}
