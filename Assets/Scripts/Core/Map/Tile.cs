using UnityEngine;

public class TileObj : MonoBehaviour
{
    public float height = 0;
    public bool canWalkable = true;
    public BuildableData[] things;

    public float maxVolume = 100;
    public float maxWeight = 50;

    public bool isFull
    {
        get
        {
            if (currVolume >= maxVolume || currWeight >= maxWeight)
            {
                return true;
            }
            return false;
        }
    }
    public bool isEmpty
    {
        get
        {
            if (currVolume <= 0 && currWeight <= 0)
            {
                return true;
            }
            return false;
        }
    }

    public float currVolume 
    {
        get
        {
            float volume = 0;
            foreach (var thing in things)
            {
                volume += thing.volume;
            }
            return volume;
        }
    }
    public float currWeight
    {
        get
        {
            float weight = 0;
            foreach (var thing in things)
            {
                weight += thing.weight;
            }
            return weight;
        }
    }
    public bool IsTileWalkable
    {
        get
        {
            if (canWalkable)
            {
                return true;
            }
            return false;
        }
    }
}
