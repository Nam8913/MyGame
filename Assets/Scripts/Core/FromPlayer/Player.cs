using UnityEngine;

public class Player : MonoBehaviour
{
    private bool chunkUpdate = false;
    public Vector2Int playerChunkPos;

    public OverworldData data;

    public GameObject gamePrefab;
    public float speed = 1f;  
    OverWorldManager worldManager;
    public Camera playerCamera;
    public MousePlayerCtr mousePlayerCtr;
    private Rigidbody2D rb;
    private bool isGrounded;

    void Start()
    {
        playerCamera = CameraCtr.CreateCameraCtr(this);
        mousePlayerCtr = new MousePlayerCtr();
        worldManager = new OverWorldManager();
        worldManager.CreateWorld(data, "TestSeed");
        rb = GetComponent<Rigidbody2D>();
        Debug.LogWarning("Test: "+Singleton<DataStorage>.Instance.Equals(Database<DataStorage>.Get()));
        BuildableData dt = DataStorage.GetData("item_wood") as BuildableData;
        GenSpawn.Spawn(dt, new Vector3(0, 0, 0), Quaternion.identity);
        // Debug.LogWarning(((GraphicSingleType)dt.graphicType).texture.pivot);
        
    }

    void Update()
    {
        moveHorizontal = Input.GetAxis("Horizontal");
        moveVertical = Input.GetAxis("Vertical");
        mousePlayerCtr.Update(this);
        transform.rotation = Quaternion.Euler(0f, 0f, mousePlayerCtr.angle);
        if(Chunk.GetChunk(this.transform.position) != playerChunkPos || !chunkUpdate)
        {
            if(!chunkUpdate)
            {
                chunkUpdate = true;
            }
            playerChunkPos = Chunk.GetChunk(this.transform.position);
            worldManager.UpdateChunks(playerChunkPos,gamePrefab);
        }
        

        
        
    }
    float moveHorizontal;
    float moveVertical;
    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        Vector2 movement = new Vector2(moveHorizontal, moveVertical);
        movement.Normalize(); // Đảm bảo tốc độ di chuyển đồng đều trong mọi hướng
        rb.linearVelocity = movement * speed;
    }
}
