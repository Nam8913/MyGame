using UnityEngine;

public class SpriteJitter : MonoBehaviour,IAction
{
    public float intensity = 0.05f;     // độ rung
    public float speed = 10f;           // tốc độ rung

    private Vector3 originalPos;

    public string ActionName { get; set; } = "Test22";
    public Entity entity
    {
        get
        {
            return GetComponent<Entity>();
        }
    }
    public void Execute(GameObject user)
    {
        Debug.Log("Test");
    }

    void Start()
    {
        originalPos = transform.localPosition;
    }

    void Update()
    {
        float offsetX = Mathf.PerlinNoise(Time.time * speed, 0f) - 0.5f;
        float offsetY = Mathf.PerlinNoise(0f, Time.time * speed) - 0.5f;
        transform.localPosition = originalPos + new Vector3(offsetX, offsetY, 0) * intensity;
    }
}