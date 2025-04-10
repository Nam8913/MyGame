using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private bool chunkUpdate = false;
    public Vector2Int playerChunkPos;

    public OverworldData data;

    public GameObject gamePrefab;
    public float speed = 5f;  
    OverWorldManager worldManager;
    private Rigidbody2D rb;
    private bool isGrounded;

    void Start()
    {
        worldManager = new OverWorldManager();
        worldManager.CreateWorld(data, "TestSeed");
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
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
