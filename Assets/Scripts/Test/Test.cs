using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Vector2Int start;
    public Vector2Int end;

    public List<Vector2Int> paths;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        start = GenRandom.Range(new Vector2Int(-36,-36), new Vector2Int(36, 36));
        end = GenRandom.Range(new Vector2Int(-36, -36), new Vector2Int(36, 36));

        paths = OverWorldManager.HPA.FindPath(start, end);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(new Vector3(start.x, start.y), 0.5f);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(new Vector3(end.x, end.y), 0.5f);
        Gizmos.color = Color.blue;
        for (int i = 0; i < paths.Count; i++)
        {
            var item = paths[i];
            if( i == 0 || i == paths.Count - 1)
            {
                continue;
            }
            Gizmos.DrawSphere(new Vector3(item.x, item.y), 0.5f);
        }
    }
}
