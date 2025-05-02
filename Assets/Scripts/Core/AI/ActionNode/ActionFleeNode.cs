using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ActionFleeNode : ActionNode
{
    public float clock = 0f;
    public float timePanic = 16f;
    public float moveSpeed = 1f;

    public Entity target;
    public Entity owner;

    public float radViewSight = 5f;
    public float safeRad = 10f;

    public int currPathIndex = 0;
    public Vector2 nextPosToMove;
    public Vector2 safePosition;
    public List<Vector2Int> paths = new List<Vector2Int>();

    protected override State DoUpdateState()
    {
         

        //Check target every 3s
        
        // if (clock % 4.5f < Time.deltaTime)
        // {
        //     Collider2D[] hits = TargetDetected();
        //     if (hits.Length == 0)
        //     {
        //         target = null;
        //     }
        // }

        if (target == null)
        {
            return State.Success;
        }
    
        DoMove();

        return state == State.Running ? State.Running : state;
    }

    protected override void EnterNode()
    {
        owner = GetContextTree().GetComponent<Entity>();
        
        
        Collider2D[] hits = TargetDetected();
        if(hits.Length == 0)
        {
            return;
        }
        Collider2D nearest = null;
        float minDist = float.MaxValue;
        foreach (var hit in hits) {
            float d = Vector3.Distance(owner.transform.position, hit.transform.position);
            if (d < minDist) {
                minDist = d;
                nearest = hit;
            }
        }
        
        if (nearest != null) {
            target = nearest.GetComponent<Entity>();
            safePosition = TryFindSafePosition(nearest.transform.position, safeRad);
        }

        DoFindSafePoint(safePosition);
    }

    Collider2D[] TargetDetected()
    {
        List<Collider2D> hits = new List<Collider2D>();
        foreach (var hit in Physics2D.OverlapCircleAll(owner.transform.position, radViewSight))
        {
            if(hit.gameObject == owner.gameObject)
            {
                continue;
            }
            hits.Add(hit);
        }
        if(hits.Count == 0)
        {
            target = null;
        }
        return hits.ToArray();
    }

    Vector2 TryFindSafePosition(Vector3 dangerPos, float safeDistance)
    {
        Vector3 dir = (owner.transform.position - dangerPos).normalized;
        Vector3 targetPos = owner.transform.position + dir * safeDistance;
        
        return targetPos;
    }

    void DoFindSafePoint(Vector2 safePos)
    {
        paths = OverWorldManager.HPA.FindPath(Vector2Int.RoundToInt(owner.transform.position), Vector2Int.RoundToInt(safePos));

        if(paths.Count == 0)
        {
            Debug.Log("CurrPos" + Vector2Int.RoundToInt(owner.transform.position)            + " TargetPos" + Vector2Int.RoundToInt(safePos) + " SafePos" + Vector2Int.RoundToInt(safePosition));
            Debug.Log("No path found, try again");
            return;
        }
    }
    void DoMove()
    {
        Vector2 currPos = owner.transform.position;
        nextPosToMove = (Vector2)paths[currPathIndex];

        float step = Time.deltaTime * moveSpeed;

        owner.transform.position = Vector2.MoveTowards(currPos, nextPosToMove, step);
        if (Vector2.Distance(currPos, nextPosToMove) < 0.1f)
        {
            currPathIndex++;
            if (currPathIndex >= paths.Count)
            {
                currPathIndex = 0;
                state = State.Success;
            }
        }
    }

    protected override void ExitNode()
    {
    }

    public override void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(owner.transform.position, radViewSight);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(safePosition, .5f);

        if(target != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(target.transform.position, .5f);
        }
        

        Gizmos.color = Color.yellow;
        for (int i = 0; i < paths.Count; i++)
        {
            var item = paths[i];
            if( i == 0 || i == paths.Count - 1)
            {
                continue;
            }
            Gizmos.DrawWireSphere(new Vector2(item.x, item.y), 0.5f);
        }
    }
}
