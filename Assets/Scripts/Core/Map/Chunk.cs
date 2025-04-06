using Unity.VisualScripting;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public Vector2Int worldPosition;
    public Vector2Int[,] tiles;

    public void Init(Vector2Int pos)
    {
        worldPosition = pos;
        tiles = new Vector2Int[OverworldData.chunkSize, OverworldData.chunkSize];
    }

    public static void GenerateChunk(Chunk chunk,GameObject prefab)
    {
        int localX = chunk.worldPosition.x * OverworldData.chunkSize;
        int localY = chunk.worldPosition.y * OverworldData.chunkSize;
        chunk.name = "Chunk_" + localX + "_" + localY;
        chunk.transform.position = new Vector3(localX, localY, 0);
        //GameObject chunkObj = new GameObject("Chunk_" + chunk.worldPosition.x * OverworldData.chunkSize + "_" + chunk.worldPosition.y * OverworldData.chunkSize);
        for (int x = 0; x < OverworldData.chunkSize; x++)
        {
            for (int y = 0; y < OverworldData.chunkSize; y++)
            {
                Tile tile = new Tile();
                GameObject tileObj = Instantiate(prefab, new Vector3(localX + x, localY + y, 0), Quaternion.identity);
                tileObj.transform.parent = chunk.transform;
            }
        }
    }

    public void SaveChunk()
    {

    }
    public bool LoadChunk()
    {
        return false;
    }
}
