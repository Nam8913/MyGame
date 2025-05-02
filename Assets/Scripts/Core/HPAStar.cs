// HPAStarManager.cs
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;


public class HPAStarChunkManager : MonoBehaviour
{
    public AbstractGraph abstractGraph = new AbstractGraph();
    [SerializeField]
    private List<Cluster> clusters = new List<Cluster>();

    private int Size => OverworldData.chunkSize;

    void Awake()
    {
    }

    void Start()
    {
        InitializeClusters();
        GenerateAllEntrances();
        BuildAllAbstractGraph();

        Test test = CurrentGame.GetScenePlay.GetComponent<Test>();
        test.Init();
    }

    void InitializeClusters()
    {
        // Each chunk is a cluster
        clusters = OverWorldManager.GetLoadedChunks().Values
            .Select
            (
                //c => new Cluster(c.Origin)
                c => 
                {
                    Cluster cluster = c.AddComponent<Cluster>();
                    cluster.origin = c.GetLocalPos;
                    return cluster;
                }
            )
            .ToList();
    }

    bool IsWalkable(Vector2Int pos)
    {
        bool flag = false;
        TileObj tile = OverWorldManager.GetTileAt(pos);
        flag = tile != null && tile.IsTileWalkable;
        return flag;
    }

    // void GenerateEntrances()
    // {
    //     int size = Size;
    //     var offsets = new[] { Vector2Int.right, Vector2Int.up };

    //     foreach (var cluster in clusters)
    //     {
    //         foreach (var offset in offsets)
    //         {
    //             Vector2Int neighborOrigin = cluster.origin + offset * size;
    //             if (!clusters.Any(c => c.origin == neighborOrigin)) continue;

    //             for (int i = 0; i < size; i++)
    //             {
    //                 Vector2Int a = cluster.origin + (offset == Vector2Int.right ? new Vector2Int(size - 1, i)
    //                                                                             : new Vector2Int(i, size - 1));
    //                 Vector2Int b = a + offset;
    //                 if (IsWalkable(a) && IsWalkable(b))
    //                     cluster.entrances.Add(new Entrance(a, b, 1));
    //             }
    //         }
    //     }
    // }

    // void BuildAbstractGraph()
    // {
    //     abstractGraph = new AbstractGraph();
    //     int size = Size;

    //     // Inter-cluster (gateways)
    //     foreach (var cl in clusters)
    //         foreach (var e in cl.entrances)
    //             abstractGraph.AddEdge(e.posA, e.posB, e.cost);

    //     // Intra-cluster (within each chunk)
    //     foreach (var cl in clusters)
    //     {
    //         var nodes = abstractGraph.GetNodesInRegion(cl.origin, size);
    //         for (int i = 0; i < nodes.Count; i++)
    //         {
    //             for (int j = i + 1; j < nodes.Count; j++)
    //             {
    //                 var n1 = nodes[i];
    //                 var n2 = nodes[j];
    //                 var path = Pathfinding.AStarGridRestricted(n1.position, n2.position, IsWalkable, cl.origin, size);
    //                 if (path != null)
    //                     abstractGraph.AddEdge(n1.position, n2.position, path.Count);
    //             }
    //         }
    //     }
    // }

    /// <summary>
    /// Incremental update when a new chunk is created.
    /// Only processes new cluster, avoiding full rebuild.
    /// </summary>
    // public void HandleChunkCreated(Chunk newChunk)
    // {
    //     var newCluster =  newChunk.AddComponent<Cluster>();
    //     newCluster.origin = newChunk.GetLocalPos;
    //     clusters.Add(newCluster);

    //     int size = Size;
    //     var offsets = new[] { Vector2Int.right, Vector2Int.up };

    //     // Inter-cluster entrances and graph edges
    //     foreach (var offset in offsets)
    //     {
    //         Vector2Int neighborOrigin = newCluster.origin + offset * size;
    //         var neighbor = clusters.FirstOrDefault(c => c.origin == neighborOrigin);
    //         if (neighbor == null) continue;

    //         var entrances = ComputeEntrancesBetween(newCluster, neighbor);
    //         foreach (var e in entrances)
    //         {
    //             newCluster.entrances.Add(e);
    //             neighbor.entrances.Add(new Entrance(e.posB, e.posA, e.cost));
    //             abstractGraph.AddEdge(e.posA, e.posB, e.cost);
    //         }
    //     }

    //     // Intra-cluster graph edges for new cluster
    //     var nodes = abstractGraph.GetNodesInRegion(newCluster.origin, size);
    //     for (int i = 0; i < nodes.Count; i++)
    //     {
    //         for (int j = i + 1; j < nodes.Count; j++)
    //         {
    //             var n1 = nodes[i];
    //             var n2 = nodes[j];
    //             var path = Pathfinding.AStarGridRestricted(n1.position, n2.position,
    //                               IsWalkable, newCluster.origin, size);
    //             if (path != null)
    //                 abstractGraph.AddEdge(n1.position, n2.position, path.Count);
    //         }
    //     }
    // }
    public void HandleChunkCreated(Chunk newChunk) {
        var newCluster =  newChunk.AddComponent<Cluster>();
        newCluster.origin = newChunk.GetLocalPos;
        clusters.Add(newCluster);
        int size = Size;
        var offsets = new[] { Vector2Int.right, Vector2Int.up };

        // For each neighboring direction, compute merged entrances
        foreach (var offset in offsets) {
            Vector2Int neighborOrigin = newCluster.origin + offset * size;
            var neighbor = clusters.FirstOrDefault(c => c.origin == neighborOrigin);
            if (neighbor == null) continue;

            var entrances = ComputeMergedEntrances(newCluster, neighbor, offset);
           foreach (var e in entrances)
            {
                newCluster.entrances.Add(e);
                neighbor.entrances.Add(new Entrance(e.posB, e.posA, e.cost));
                abstractGraph.AddEdge(e.posA, e.posB, e.cost);
            }
        }

        // Intra-cluster edges for new cluster only for new entrances
        var nodes = abstractGraph.GetNodesInRegion(newCluster.origin, size);
        for (int i = 0; i < nodes.Count; i++) {
            for (int j = i + 1; j < nodes.Count; j++) {
                var n1 = nodes[i];
                var n2 = nodes[j];
                var path = Pathfinding.AStarGridRestricted(n1.position, n2.position,
                                  IsWalkable, newCluster.origin, size);
                if (path != null)
                    abstractGraph.AddEdge(n1.position, n2.position, path.Count);
            }
        }
    }

     

    // void GenerateAllEntrances()
    // {
    //     foreach (var cl in clusters)
    //         cl.entrances.Clear();

    //     int size = Size;
    //     var offsets = new[] { Vector2Int.right, Vector2Int.up };

    //     foreach (var cluster in clusters)
    //         foreach (var offset in offsets)
    //         {
    //             Vector2Int neighborOrigin = cluster.origin + offset * size;
    //             if (!clusters.Any(c => c.origin == neighborOrigin)) continue;

    //             for (int i = 0; i < size; i++)
    //             {
    //                 Vector2Int a = cluster.origin +
    //                     (offset == Vector2Int.right
    //                         ? new Vector2Int(size - 1, i)
    //                         : new Vector2Int(i, size - 1));
    //                 Vector2Int b = a + offset;
    //                 if (IsWalkable(a) && IsWalkable(b))
    //                     cluster.entrances.Add(new Entrance(a, b, 1));
    //             }
    //         }
    // }
    void GenerateAllEntrances() {
        foreach (var cl in clusters)
            cl.entrances.Clear();
        int size = Size;
        var offsets = new[] { Vector2Int.right, Vector2Int.up };

        foreach (var cl in clusters) {
            foreach (var offset in offsets) {
                Vector2Int neighborOrigin = cl.origin + offset * size;
                if (!clusters.Any(c => c.origin == neighborOrigin)) continue;

                var merged = ComputeMergedEntrances(cl, clusters.First(c => c.origin == neighborOrigin), offset);
                cl.entrances.AddRange(merged);
            }
        }
    }

    void BuildAllAbstractGraph()
    {
        abstractGraph = new AbstractGraph();
        int size = Size;

        // Inter-cluster edges
        foreach (var cl in clusters)
            foreach (var e in cl.entrances)
                abstractGraph.AddEdge(e.posA, e.posB, e.cost);

        // Intra-cluster edges
        foreach (var cl in clusters)
        {
            var nodes = abstractGraph.GetNodesInRegion(cl.origin, size);
            for (int i = 0; i < nodes.Count; i++)
                for (int j = i + 1; j < nodes.Count; j++)
                {
                    var n1 = nodes[i];
                    var n2 = nodes[j];
                    var path = Pathfinding.AStarGridRestricted(n1.position, n2.position,
                                      IsWalkable, cl.origin, size);
                    if (path != null)
                        abstractGraph.AddEdge(n1.position, n2.position, path.Count);
                }
        }
    }

    List<Entrance> ComputeEntrancesBetween(Cluster a, Cluster b)
    {
        var list = new List<Entrance>();
        int size = Size;
        // Determine direction from a to b (assumes neighbors on right or up)
        Vector2Int offset = (b.origin.x > a.origin.x) ? Vector2Int.right : Vector2Int.up;

        for (int i = 0; i < size; i++)
        {
            Vector2Int pa = a.origin +
                (offset == Vector2Int.right
                    ? new Vector2Int(size - 1, i)
                    : new Vector2Int(i, size - 1));
            Vector2Int pb = pa + offset;
            if (IsWalkable(pa) && IsWalkable(pb))
                list.Add(new Entrance(pa, pb, 1));
        }
        return list;
    }
    /// <summary>
    /// Scans the shared border between two clusters and merges consecutive walkable pairs
    /// into a single entrance at the middle of each contiguous segment.
    /// </summary>
    // List<Entrance> ComputeMergedEntrances(Cluster a, Cluster b, Vector2Int offset) {
    //     var list = new List<Entrance>();
    //     int size = Size;
    //     bool inSegment = false;
    //     int segStart = 0;

    //     // Helper to compute world positions
    //     Func<int, Vector2Int> posAAt = i => a.origin + (offset == Vector2Int.right
    //         ? new Vector2Int(size - 1, i)
    //         : new Vector2Int(i, size - 1));
    //     Func<Vector2Int, Vector2Int> posBFromA = pa => pa + offset;

    //     for (int i = 0; i <= size; i++) {
    //         bool walkable = false;
    //         if (i < size) {
    //             var pa = posAAt(i);
    //             var pb = posBFromA(pa);
    //             walkable = IsWalkable(pa) && IsWalkable(pb);
    //         }

    //         if (walkable && !inSegment) {
    //             inSegment = true;
    //             segStart = i;
    //         } else if ((!walkable || i == size) && inSegment) {
    //             // segment ends at i-1
    //             int segEnd = i - 1;
    //             int mid = (segStart + segEnd) / 2;
    //             var paMid = posAAt(mid);
    //             var pbMid = posBFromA(paMid);
    //             list.Add(new Entrance(paMid, pbMid, segEnd - segStart + 1));
    //             inSegment = false;
    //         }
    //     }
    //     return list;
    // }
    List<Entrance> ComputeMergedEntrances(Cluster a, Cluster b, Vector2Int offset) {
    var list = new List<Entrance>();
    int size = Size;
    bool inSegment = false;
    int segStart = 0;

    Func<int, Vector2Int> posAAt = i => a.origin + (offset == Vector2Int.right
        ? new Vector2Int(size - 1, i)
        : new Vector2Int(i, size - 1));
    Func<Vector2Int, Vector2Int> posBFromA = pa => pa + offset;

    for (int i = 0; i <= size; i++) {
        bool walkable = false;
        if (i < size) {
            var pa = posAAt(i);
            var pb = posBFromA(pa);
            walkable = IsWalkable(pa) && IsWalkable(pb);
        }

        if (walkable && !inSegment) {
            inSegment = true;
            segStart = i;
        } else if ((!walkable || i == size) && inSegment) {
            int segEnd = i - 1;
            int length = segEnd - segStart + 1;

            if (length >= 3) {
                int partLength = length / 3;
                for (int part = 0; part < 3; part++) {
                    int mid = segStart + part * partLength + partLength / 2;
                    var paMid = posAAt(mid);
                    var pbMid = posBFromA(paMid);
                    list.Add(new Entrance(paMid, pbMid, partLength));
                }
                list.Add(new Entrance(posAAt(segStart), posBFromA(posAAt(segStart)), partLength));
                list.Add(new Entrance(posAAt(segEnd), posBFromA(posAAt(segEnd)), partLength));
            } else {
                // Nếu đoạn ngắn quá (<3 tiles), tạo 1 entrance ở giữa như cũ
                int mid = (segStart + segEnd) / 2;
                var paMid = posAAt(mid);
                var pbMid = posBFromA(paMid);
                list.Add(new Entrance(paMid, pbMid, length));
            }

            inSegment = false;
        }
    }
    return list;
}

    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int goal) {
        var startCl = GetClusterAt(start); var goalCl = GetClusterAt(goal);
        if (startCl == goalCl) return Pathfinding.AStarGrid(start, goal, IsWalkable);
        var startNodes = abstractGraph.GetNodesInRegion(startCl.origin, Size);
        var goalNodes  = abstractGraph.GetNodesInRegion(goalCl.origin,  Size);
        List<Vector2Int> bestPath = null; float bestCost = float.MaxValue;
        foreach (var sNode in startNodes) {
            var path1 = Pathfinding.AStarGrid(start, sNode.position, IsWalkable);
            if (path1 == null) continue; float cost1 = Pathfinding.PathCost(path1);
            foreach (var gNode in goalNodes) {
                var (absPath, cost2) = Pathfinding.AStarAbstract(sNode, gNode, abstractGraph);
                if (absPath == null) continue;
                var path3 = Pathfinding.AStarGrid(gNode.position, goal, IsWalkable);
                if (path3 == null) continue; float cost3 = Pathfinding.PathCost(path3);
                float total = cost1 + cost2 + cost3;
                if (total < bestCost) {
                    bestCost = total;
                    var full = new List<Vector2Int>(path1);
                    Vector2Int prev = sNode.position;
                    foreach (var node in absPath.Skip(1)) {
                        if ((prev - node.position).magnitude <= 1.1f) full.Add(node.position);
                        else {
                            var seg = Pathfinding.AStarGrid(prev, node.position, IsWalkable);
                            seg.RemoveAt(0); full.AddRange(seg);
                        }
                        prev = node.position;
                    }
                    path3.RemoveAt(0); full.AddRange(path3);
                    bestPath = full;
                }
            }
        }
        return bestPath;
    }

    Cluster GetClusterAt(Vector2Int pos)
    {
        return clusters.FirstOrDefault(c =>
            pos.x >= c.origin.x && pos.x < c.origin.x + Size &&
            pos.y >= c.origin.y && pos.y < c.origin.y + Size);
    }
    void OnDrawGizmosSelected()
    {
        if (clusters == null || abstractGraph == null) return;

        // Draw clusters
        Gizmos.color = Color.yellow;
        foreach (var cl in clusters)
        {
            Gizmos.DrawWireCube((Vector2)cl.origin + Vector2.one * (Size / 2f),
                                Vector2.one * Size);
        }

        // Draw entrances
        Gizmos.color = Color.green;
        foreach (var cl in clusters)
        {
            foreach (var e in cl.entrances)
            {
                Gizmos.DrawLine((Vector2)e.posA + Vector2.one * 0.5f, (Vector2)e.posB + Vector2.one * 0.5f);
            }
        }

        // Draw abstract graph edges
        Gizmos.color = Color.cyan;
        foreach (var node in abstractGraph.nodes.Values)
        {
            foreach (var edge in node.edges)
            {
                if (node.position.x <= edge.to.position.x) // prevent duplicate draw
                    Gizmos.DrawLine((Vector2)node.position + Vector2.one * 0.5f,
                                    (Vector2)edge.to.position + Vector2.one * 0.5f);
            }
        }
    }
    
}

#region Support Classes
[RequireComponent(typeof(Chunk))]
public class Cluster : MonoBehaviour
{
    public Vector2Int origin;
    [SerializeField]
    public List<Entrance> entrances = new List<Entrance>();


    void Awake()
    {
        if (entrances == null)
            entrances = new List<Entrance>();
    }
}
[System.Serializable]
public class Entrance
{
    public Vector2Int posA, posB;
    public float cost;
    public Entrance(Vector2Int a, Vector2Int b, float c) { posA = a; posB = b; cost = c; }
}
public class AbstractGraph
{
    public Dictionary<Vector2Int, AbstractNode> nodes = new Dictionary<Vector2Int, AbstractNode>();
    public void AddEdge(Vector2Int aPos, Vector2Int bPos, float cost)
    {
        var a = GetOrCreate(aPos);
        var b = GetOrCreate(bPos);
        a.edges.Add(new AbstractEdge(b, cost));
        b.edges.Add(new AbstractEdge(a, cost));
    }
    AbstractNode GetOrCreate(Vector2Int pos)
    {
        if (!nodes.TryGetValue(pos, out var n)) { n = new AbstractNode(pos); nodes[pos] = n; }
        return n;
    }
    public List<AbstractNode> GetNodesInRegion(Vector2Int ori, int size)
        => nodes.Values.Where(n =>
            n.position.x >= ori.x && n.position.x < ori.x + size &&
            n.position.y >= ori.y && n.position.y < ori.y + size)
            .ToList();
}
public class AbstractNode
{
    public Vector2Int position;
    public List<AbstractEdge> edges = new List<AbstractEdge>();
    public AbstractNode(Vector2Int p) { position = p; }
}
public class AbstractEdge { public AbstractNode to; public float cost; public AbstractEdge(AbstractNode t, float c) { to = t; cost = c; } }
#endregion

#region Pathfinding Utilities
public static class Pathfinding
{
    static readonly Vector2Int[] directions = new Vector2Int[] {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
        new Vector2Int(1,1), new Vector2Int(1,-1), new Vector2Int(-1,1), new Vector2Int(-1,-1)
    };
    static readonly float straightCost = 1f;
    static readonly float diagCost = Mathf.Sqrt(2f);
    public static List<Vector2Int> AStarGrid(Vector2Int start, Vector2Int goal, Func<Vector2Int,bool> isWalkable) {
        var open = new SimplePriorityQueue<NodeRecord>();
        var cameFrom = new Dictionary<Vector2Int,Vector2Int>();
        var costSoFar = new Dictionary<Vector2Int,float> {{start,0f}};
        open.Enqueue(new NodeRecord(start,0f), Heuristic(start,goal));
        while(open.Count>0) {
            var current = open.Dequeue().pos;
            if(current==goal) break;
            foreach(var dir in directions) {
                var next = current + dir;
                if(!isWalkable(next)) continue;
                // Corner-cutting: disallow diagonal if adjacent walls
                if(Mathf.Abs(dir.x)==1 && Mathf.Abs(dir.y)==1) {
                    var side1 = new Vector2Int(current.x + dir.x, current.y);
                    var side2 = new Vector2Int(current.x, current.y + dir.y);
                    if(!isWalkable(side1) || !isWalkable(side2))
                        continue;
                }
                float moveCost = (Mathf.Abs(dir.x)==1 && Mathf.Abs(dir.y)==1) ? diagCost : straightCost;
                float newCost = costSoFar[current] + moveCost;
                if(!costSoFar.ContainsKey(next) || newCost < costSoFar[next]) {
                    costSoFar[next] = newCost;
                    float priority = newCost + Heuristic(next,goal);
                    open.Enqueue(new NodeRecord(next,newCost), priority);
                    cameFrom[next] = current;
                }
            }
        }
        if(!cameFrom.ContainsKey(goal)) return null;
        var path = new List<Vector2Int>(); var cur = goal;
        while(cur != start) { path.Add(cur); cur = cameFrom[cur]; }
        path.Add(start); path.Reverse(); return path;
    }

    public static List<Vector2Int> AStarGridRestricted(Vector2Int start, Vector2Int goal, Func<Vector2Int,bool> isWalkable, Vector2Int ori, int size) {
        var open = new SimplePriorityQueue<NodeRecord>();
        var cameFrom = new Dictionary<Vector2Int,Vector2Int>();
        var costSoFar = new Dictionary<Vector2Int,float> {{start,0f}};
        open.Enqueue(new NodeRecord(start,0f), Heuristic(start,goal));
        while(open.Count>0) {
            var current = open.Dequeue().pos;
            if(current==goal) break;
            foreach(var dir in directions) {
                var next = current + dir;
                if(next.x < ori.x || next.y < ori.y || next.x >= ori.x + size || next.y >= ori.y + size) continue;
                if(!isWalkable(next)) continue;
                if(Mathf.Abs(dir.x)==1 && Mathf.Abs(dir.y)==1) {
                    var side1 = new Vector2Int(current.x + dir.x, current.y);
                    var side2 = new Vector2Int(current.x, current.y + dir.y);
                    if(!isWalkable(side1) || !isWalkable(side2))
                        continue;
                }
                float moveCost = (Mathf.Abs(dir.x)==1 && Mathf.Abs(dir.y)==1) ? diagCost : straightCost;
                float newCost = costSoFar[current] + moveCost;
                if(!costSoFar.ContainsKey(next) || newCost < costSoFar[next]) {
                    costSoFar[next] = newCost;
                    float priority = newCost + Heuristic(next,goal);
                    open.Enqueue(new NodeRecord(next,newCost), priority);
                    cameFrom[next] = current;
                }
            }
        }
        if(!cameFrom.ContainsKey(goal)) return null;
        var path = new List<Vector2Int>(); var cur = goal;
        while(cur != start) { path.Add(cur); cur = cameFrom[cur]; }
        path.Add(start); path.Reverse(); return path;
    }

    public static (List<AbstractNode>,float) AStarAbstract(AbstractNode start, AbstractNode goal, AbstractGraph graph) {
        var open=new SimplePriorityQueue<ANodeRecord>();
        var cameFrom=new Dictionary<AbstractNode,AbstractNode>();
        var costSoFar=new Dictionary<AbstractNode,float>{{start,0f}};
        open.Enqueue(new ANodeRecord(start,0f),0f);
        while(open.Count>0) {
            var current=open.Dequeue().node;
            if(current==goal) break;
            foreach(var edge in current.edges) {
                var next=edge.to; float newCost=costSoFar[current]+edge.cost;
                if(!costSoFar.ContainsKey(next)||newCost<costSoFar[next]) {
                    costSoFar[next]=newCost;
                    open.Enqueue(new ANodeRecord(next,newCost),newCost);
                    cameFrom[next]=current;
                }
            }
        }
        if(!costSoFar.ContainsKey(goal)) return (null,float.MaxValue);
        var path=new List<AbstractNode>(); var curN=goal;
        while(curN!=start){path.Add(curN);curN=cameFrom[curN];}
        path.Add(start);path.Reverse();return (path,costSoFar[goal]);
    }


    
    static float Heuristic(Vector2Int a, Vector2Int b) {
    int dx=Mathf.Abs(a.x-b.x), dy=Mathf.Abs(a.y-b.y);
    return straightCost*(dx+dy) + (diagCost-2*straightCost)*Mathf.Min(dx,dy);
    }

    public static float PathCost(List<Vector2Int> path) {
        float cost=0f;
        for(int i=1;i<path.Count;i++) {
            var d=path[i]-path[i-1];
            cost += (Mathf.Abs(d.x)==1&&Mathf.Abs(d.y)==1)?diagCost:straightCost;
        }
        return cost;
    }

    class NodeRecord { public Vector2Int pos; public float cost; public NodeRecord(Vector2Int p, float c) { pos = p; cost = c; } }
    class ANodeRecord { public AbstractNode node; public float cost; public ANodeRecord(AbstractNode n, float c) { node = n; cost = c; } }
}
#endregion

// public class HPAStarManager : MonoBehaviour
// {
//     public int clusterSize = 10;
//     public int mapWidth = 100;
//     public int mapHeight = 100;
//     public bool[,] walkableMap;

//     public List<Cluster> clusters = new List<Cluster>();
//     public AbstractGraph abstractGraph = new AbstractGraph();

//     void Start()
//     {
//         // Initialize map (all walkable for demo)
//         walkableMap = new bool[mapWidth, mapHeight];
//         for (int x = 0; x < mapWidth; x++)
//             for (int y = 0; y < mapHeight; y++)
//                 walkableMap[x, y] = true;

//         GenerateClusters();
//         GenerateEntrances();
//         BuildAbstractGraph();
//     }

//     void GenerateClusters()
//     {
//         clusters.Clear();
//         for (int y = 0; y < mapHeight; y += clusterSize)
//             for (int x = 0; x < mapWidth; x += clusterSize)
//                 clusters.Add(new Cluster(new Vector2Int(x, y)));
//     }

//     void GenerateEntrances()
//     {
//         foreach (var cluster in clusters)
//         {
//             var origin = cluster.origin;
//             for (int i = 0; i < clusterSize; i++)
//             {
//                 // Right neighbor
//                 var a = new Vector2Int(origin.x + clusterSize - 1, origin.y + i);
//                 var b = a + Vector2Int.right;
//                 if (IsWalkable(a) && IsWalkable(b))
//                     cluster.entrances.Add(new Entrance(a, b, 1));

//                 // Top neighbor
//                 var c = new Vector2Int(origin.x + i, origin.y + clusterSize - 1);
//                 var d = c + Vector2Int.up;
//                 if (IsWalkable(c) && IsWalkable(d))
//                     cluster.entrances.Add(new Entrance(c, d, 1));
//             }
//         }
//     }

//     public bool IsWalkable(Vector2Int pos)
//         => pos.x >= 0 && pos.y >= 0 && pos.x < mapWidth && pos.y < mapHeight && walkableMap[pos.x, pos.y];

//     void BuildAbstractGraph()
//     {
//         abstractGraph = new AbstractGraph();
//         // Inter-cluster edges (gateways)
//         foreach (var cl in clusters)
//         {
//             foreach (var e in cl.entrances)
//                 abstractGraph.AddEdge(e.posA, e.posB, e.cost);
//         }
//         // Intra-cluster edges (within clusters)
//         foreach (var cl in clusters)
//         {
//             var nodes = abstractGraph.GetNodesInRegion(cl.origin, clusterSize);
//             for (int i = 0; i < nodes.Count; i++)
//             {
//                 for (int j = i + 1; j < nodes.Count; j++)
//                 {
//                     var n1 = nodes[i];
//                     var n2 = nodes[j];
//                     var path = Pathfinding.AStarGridRestricted(n1.position, n2.position, walkableMap, cl.origin, clusterSize);
//                     if (path != null)
//                         abstractGraph.AddEdge(n1.position, n2.position, path.Count);
//                 }
//             }
//         }
//     }

//     public List<Vector2Int> FindPath(Vector2Int start, Vector2Int goal)
//     {
//         // If in the same cluster, use low-level A*
//         var startCl = GetClusterAt(start);
//         var goalCl = GetClusterAt(goal);
//         if (startCl == goalCl)
//             return Pathfinding.AStarGrid(start, goal, walkableMap);

//         // Prepare entrance nodes
//         var startEntrances = abstractGraph.GetNodesInRegion(startCl.origin, clusterSize);
//         var goalEntrances = abstractGraph.GetNodesInRegion(goalCl.origin, clusterSize);

//         List<Vector2Int> bestPath = null;
//         float bestCost = float.MaxValue;

//         // Try all combinations of gateway entries
//         foreach (var sNode in startEntrances)
//         {
//             var path1 = Pathfinding.AStarGrid(start, sNode.position, walkableMap);
//             if (path1 == null) continue;
//             float cost1 = path1.Count;

//             foreach (var gNode in goalEntrances)
//             {
//                 var (absPath, cost2) = Pathfinding.AStarAbstract(sNode, gNode, abstractGraph);
//                 if (absPath == null) continue;

//                 var path3 = Pathfinding.AStarGrid(gNode.position, goal, walkableMap);
//                 if (path3 == null) continue;
//                 float cost3 = path3.Count;

//                 float total = cost1 + cost2 + cost3;
//                 if (total < bestCost)
//                 {
//                     bestCost = total;
//                     // Reconstruct full path
//                     var full = new List<Vector2Int>(path1);
//                     Vector2Int prev = sNode.position;
//                     foreach (var node in absPath.Skip(1))
//                     {
//                         if (Vector2Int.Distance(prev, node.position) <= 1)
//                         {
//                             full.Add(node.position);
//                         }
//                         else
//                         {
//                             var segment = Pathfinding.AStarGrid(prev, node.position, walkableMap);
//                             segment.RemoveAt(0);
//                             full.AddRange(segment);
//                         }
//                         prev = node.position;
//                     }
//                     path3.RemoveAt(0);
//                     full.AddRange(path3);
//                     bestPath = full;
//                 }
//             }
//         }

//         return bestPath;
//     }

//     Cluster GetClusterAt(Vector2Int pos)
//         => clusters.FirstOrDefault(c => pos.x >= c.origin.x && pos.x < c.origin.x + clusterSize
//                                       && pos.y >= c.origin.y && pos.y < c.origin.y + clusterSize);
// }

// #region Support Classes
// public class Cluster
// {
//     public Vector2Int origin;
//     public List<Entrance> entrances = new List<Entrance>();
//     public Cluster(Vector2Int o) { origin = o; }
// }
// public class Entrance
// {
//     public Vector2Int posA, posB; public float cost;
//     public Entrance(Vector2Int a, Vector2Int b, float c) { posA = a; posB = b; cost = c; }
// }
// public class AbstractGraph
// {
//     public Dictionary<Vector2Int, AbstractNode> nodes = new Dictionary<Vector2Int, AbstractNode>();
//     public void AddEdge(Vector2Int aPos, Vector2Int bPos, float cost)
//     {
//         var a = GetOrCreate(aPos); var b = GetOrCreate(bPos);
//         a.edges.Add(new AbstractEdge(b, cost));
//         b.edges.Add(new AbstractEdge(a, cost));
//     }
//     AbstractNode GetOrCreate(Vector2Int pos)
//     {
//         if (!nodes.TryGetValue(pos, out var n)) { n = new AbstractNode(pos); nodes[pos] = n; }
//         return n;
//     }
//     public List<AbstractNode> GetNodesInRegion(Vector2Int ori, int size)
//         => nodes.Values.Where(n => n.position.x >= ori.x && n.position.x < ori.x + size
//                                  && n.position.y >= ori.y && n.position.y < ori.y + size).ToList();
// }
// public class AbstractNode
// {
//     public Vector2Int position; public List<AbstractEdge> edges = new List<AbstractEdge>();
//     public AbstractNode(Vector2Int p) { position = p; }
// }
// public class AbstractEdge { public AbstractNode to; public float cost; public AbstractEdge(AbstractNode t, float c) { to = t; cost = c; } }
// #endregion

// #region Pathfinding Utilities
// public static class Pathfinding
// {
//     public static List<Vector2Int> AStarGrid(Vector2Int start, Vector2Int goal, bool[,] map)
//     {
//         var open = new SimplePriorityQueue<NodeRecord>();
//         var cameFrom = new Dictionary<Vector2Int, Vector2Int>();
//         var costSoFar = new Dictionary<Vector2Int, float> { [start] = 0 };
//         open.Enqueue(new NodeRecord(start, 0), Heuristic(start, goal));

//         while (open.Count > 0)
//         {
//             var current = open.Dequeue().pos;
//             if (current == goal) break;
//             foreach (var dir in new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
//             {
//                 var next = current + dir;
//                 if (next.x < 0 || next.y < 0 || next.x >= map.GetLength(0) || next.y >= map.GetLength(1) || !map[next.x, next.y]) continue;
//                 float newCost = costSoFar[current] + 1;
//                 if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
//                 {
//                     costSoFar[next] = newCost;
//                     float priority = newCost + Heuristic(next, goal);
//                     open.Enqueue(new NodeRecord(next, newCost), priority);
//                     cameFrom[next] = current;
//                 }
//             }
//         }
//         if (!cameFrom.ContainsKey(goal)) return null;
//         var path = new List<Vector2Int>();
//         var cur = goal;
//         while (cur != start)
//         {
//             path.Add(cur);
//             cur = cameFrom[cur];
//         }
//         path.Add(start);
//         path.Reverse();
//         return path;
//     }

//     public static List<Vector2Int> AStarGridRestricted(Vector2Int start, Vector2Int goal, bool[,] map, Vector2Int ori, int size)
//     {
//         var open = new SimplePriorityQueue<NodeRecord>();
//         var cameFrom = new Dictionary<Vector2Int, Vector2Int>();
//         var costSoFar = new Dictionary<Vector2Int, float> { [start] = 0 };
//         open.Enqueue(new NodeRecord(start, 0), Heuristic(start, goal));

//         while (open.Count > 0)
//         {
//             var current = open.Dequeue().pos;
//             if (current == goal) break;
//             foreach (var dir in new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
//             {
//                 var next = current + dir;
//                 if (next.x < ori.x || next.y < ori.y || next.x >= ori.x + size || next.y >= ori.y + size || !map[next.x, next.y]) continue;
//                 float newCost = costSoFar[current] + 1;
//                 if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
//                 {
//                     costSoFar[next] = newCost;
//                     float priority = newCost + Heuristic(next, goal);
//                     open.Enqueue(new NodeRecord(next, newCost), priority);
//                     cameFrom[next] = current;
//                 }
//             }
//         }
//         if (!cameFrom.ContainsKey(goal)) return null;
//         var path = new List<Vector2Int>();
//         var cur = goal;
//         while (cur != start)
//         {
//             path.Add(cur);
//             cur = cameFrom[cur];
//         }
//         path.Add(start);
//         path.Reverse();
//         return path;
//     }

//     public static (List<AbstractNode>, float) AStarAbstract(AbstractNode start, AbstractNode goal, AbstractGraph graph)
//     {
//         var open = new SimplePriorityQueue<ANodeRecord>();
//         var cameFrom = new Dictionary<AbstractNode, AbstractNode>();
//         var costSoFar = new Dictionary<AbstractNode, float> { [start] = 0 };
//         open.Enqueue(new ANodeRecord(start, 0), 0);

//         while (open.Count > 0)
//         {
//             var current = open.Dequeue().node;
//             if (current == goal) break;
//             foreach (var edge in current.edges)
//             {
//                 var next = edge.to;
//                 float newCost = costSoFar[current] + edge.cost;
//                 if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
//                 {
//                     costSoFar[next] = newCost;
//                     open.Enqueue(new ANodeRecord(next, newCost), newCost);
//                     cameFrom[next] = current;
//                 }
//             }
//         }
//         if (!costSoFar.ContainsKey(goal)) return (null, float.MaxValue);
//         var path = new List<AbstractNode>();
//         var cur = goal;
//         while (cur != start)
//         {
//             path.Add(cur);
//             cur = cameFrom[cur];
//         }
//         path.Add(start);
//         path.Reverse();
//         return (path, costSoFar[goal]);
//     }

//     static float Heuristic(Vector2Int a, Vector2Int b)
//         => Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);

//     class NodeRecord { public Vector2Int pos; public float cost; public NodeRecord(Vector2Int p, float c) { pos = p; cost = c; } }
//     class ANodeRecord { public AbstractNode node; public float cost; public ANodeRecord(AbstractNode n, float c) { node = n; cost = c; } }
// }
// #endregion
public class SimplePriorityQueue<T>
{
    private List<(T item, float priority)> heap = new List<(T, float)>();

    public int Count => heap.Count;

    public void Enqueue(T item, float priority)
    {
        heap.Add((item, priority));
        SiftUp(heap.Count - 1);
    }

    public T Dequeue()
    {
        var root = heap[0].item;
        heap[0] = heap[heap.Count - 1];
        heap.RemoveAt(heap.Count - 1);
        SiftDown(0);
        return root;
    }

    void SiftUp(int i)
    {
        while (i > 0)
        {
            int parent = (i - 1) / 2;
            if (heap[i].priority >= heap[parent].priority) break;
            (heap[i], heap[parent]) = (heap[parent], heap[i]);
            i = parent;
        }
    }

    void SiftDown(int i)
    {
        int left, right, smallest;
        while (true)
        {
            left = 2 * i + 1; right = 2 * i + 2; smallest = i;
            if (left < heap.Count && heap[left].priority < heap[smallest].priority) smallest = left;
            if (right < heap.Count && heap[right].priority < heap[smallest].priority) smallest = right;
            if (smallest == i) break;
            (heap[i], heap[smallest]) = (heap[smallest], heap[i]);
            i = smallest;
        }
    }
}