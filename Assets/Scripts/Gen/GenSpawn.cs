using UnityEngine;

public class GenSpawn
{
    public static void Spawn(BuildableData data , Vector3 position, Quaternion rotation)
    {
        if(CurrentGame.getWorld == null)
        {
            Debug.LogError("World is not initialized.");
            return;
        }

        GameObject obj = Entity.MakeEntityFor(data);
        obj.transform.position = position;
        obj.transform.rotation = rotation;
    }
    
}
