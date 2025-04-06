using System.Text;
using UnityEngine;

public class OverworldData
{
    public string name;
    public int seed;

    public static readonly int chunkSize = 24;
    public static int cachedSize = 2; // Số lượng chunk được lưu trong bộ nhớ xung quanh người chơi

    public static int GetHashSeed(string seed)
    {
        return (int)System.Convert.ToInt32(seed);
    }
}
