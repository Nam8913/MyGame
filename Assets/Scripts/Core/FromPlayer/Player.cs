using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private bool chunkUpdate = false;
    public Vector2Int playerChunkPos;

    public OverworldData data;

    public GameObject gamePrefab;
    public float speed = 5f;  
    OverWorldManager worldManager;
    public Camera playerCamera;
    public MousePlayerCtr mousePlayerCtr;
    private Rigidbody2D rb;
    private bool isGrounded;

    void Start()
    {
        mousePlayerCtr = new MousePlayerCtr();
        worldManager = new OverWorldManager();
        worldManager.CreateWorld(data, "TestSeed");
        rb = GetComponent<Rigidbody2D>();

        // BuildableData dt = Database.GetData("item_wood") as BuildableData;
        // GenSpawn.Spawn(dt, new Vector3(0, 0, 0), Quaternion.identity);
        // Debug.LogWarning(((GraphicSingleType)dt.graphicType).texture.pivot);

        Vector2 test = new Vector2(0, 0);
        Debug.LogWarning(TypePatch.ParseVec2("(0, 1)").ToString());
        Debug.LogWarning(test.ToString());
        Debug.LogWarning(TypePatch.ParseVec2(test.ToString()));
    }

    void Update()
    {
        mousePlayerCtr.Update();
        if(Chunk.GetChunk(this.transform.position) != playerChunkPos || !chunkUpdate)
        {
            if(!chunkUpdate)
            {
                chunkUpdate = true;
            }
            playerChunkPos = Chunk.GetChunk(this.transform.position);
            worldManager.UpdateChunks(playerChunkPos,gamePrefab);
        }
        

        
        Move();
    }

    void Move()
    {
    }
}
