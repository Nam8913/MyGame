using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject gamePrefab;
    public float speed = 5f;  
    OverWorldManager worldManager;
    private Rigidbody2D rb;
    private bool isGrounded;

    void Start()
    {
        worldManager = new OverWorldManager();
        worldManager.CreateWorld("TestWorld", "TestSeed");
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        worldManager.UpdateChunks(this.transform.position,gamePrefab);

        
        Move();
    }

    void Move()
    {
    }
}
