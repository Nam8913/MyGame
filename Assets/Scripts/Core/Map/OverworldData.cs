using System.Text;
using UnityEngine;
[System.Serializable]
public class OverworldData
{
    public string name;
    public int seed;

    #region Noise
    public float scale = 30f; // Tỉ lệ của noise
    public int octaves = 1; // Độ zoom noise
    public float persistance = 0.5f; // Độ giảm biên độ
    public float lacunarity = 1f; // Tần số tăng
    
    public float frequency = 1f; //
    public float amplitude = 1; //
    #endregion

    public static readonly int chunkSize = 24;
    public static int cachedSize = 4; // Số lượng chunk được lưu trong bộ nhớ xung quanh người chơi

    public static int GetHashSeed(string seed)
    {
        if (seed == null)
        {
            return 0;
        }
        int num = 23;
        int length = seed.Length;
        for (int i = 0; i < length; i++)
        {
            num = num * 31 + (int)seed[i];
        }
        return num;
    }
}
