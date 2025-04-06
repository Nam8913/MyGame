using UnityEngine;

public static class GenRandom
{
    public static void SetSeed(uint seed)
    {
        GenRandom.seed = seed;
    }
    public static uint GetSeed()
    {
        return GenRandom.seed;
    }
    public static int Range(int min, int max)
    {
        return Random.Range(min, max);
    }
    public static float Range(float min, float max)
    {
        return Random.Range(min, max);
    }
    public static Vector2 Range(Vector2 min, Vector2 max)
    {
        return new Vector2(Random.Range(min.x, max.x), Random.Range(min.y, max.y));
    }
    public static Vector3 Range(Vector3 min, Vector3 max)
    {
        return new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));
    }
    public static Vector2Int Range(Vector2Int min, Vector2Int max)
    {
        return new Vector2Int(Random.Range(min.x, max.x), Random.Range(min.y, max.y));
    }
    public static Vector3Int Range(Vector3Int min, Vector3Int max)
    {
        return new Vector3Int(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));
    }
    public static Color Range(Color min, Color max)
    {
        return new Color(Random.Range(min.r, max.r), Random.Range(min.g, max.g), Random.Range(min.b, max.b), Random.Range(min.a, max.a));
    }
    public static Color32 Range(Color32 min, Color32 max)
    {
        return new Color32((byte)Random.Range(min.r, max.r), (byte)Random.Range(min.g, max.g), (byte)Random.Range(min.b, max.b), (byte)Random.Range(min.a, max.a));
    }
    public static Quaternion Range(Quaternion min, Quaternion max)
    {
        return new Quaternion(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z), Random.Range(min.w, max.w));
    }
    public static Vector2 RandomPointInCircle(float radius)
    {
        float angle = Random.Range(0f, 2f * Mathf.PI);
        float x = radius * Mathf.Cos(angle);
        float y = radius * Mathf.Sin(angle);
        return new Vector2(x, y);
    }
    public static Vector3 RandomPointInSphere(float radius)
    {
        float angle = Random.Range(0f, 2f * Mathf.PI);
        float x = radius * Mathf.Cos(angle);
        float y = radius * Mathf.Sin(angle);
        float z = Random.Range(-radius, radius);
        return new Vector3(x, y, z);
    }
    public static Vector2 RandomPointInRectangle(Vector2 min, Vector2 max)
    {
        float x = Random.Range(min.x, max.x);
        float y = Random.Range(min.y, max.y);
        return new Vector2(x, y);
    }
    public static Vector3 RandomPointInBox(Vector3 min, Vector3 max)
    {
        float x = Random.Range(min.x, max.x);
        float y = Random.Range(min.y, max.y);
        float z = Random.Range(min.z, max.z);
        return new Vector3(x, y, z);
    }
    public static Vector2 RandomPointOnCircle(float radius)
    {
        float angle = Random.Range(0f, 2f * Mathf.PI);
        float x = radius * Mathf.Cos(angle);
        float y = radius * Mathf.Sin(angle);
        return new Vector2(x, y);
    }
    public static Vector3 RandomPointOnSphere(float radius)
    {
        float angle = Random.Range(0f, 2f * Mathf.PI);
        float x = radius * Mathf.Cos(angle);
        float y = radius * Mathf.Sin(angle);
        return new Vector3(x, y, radius);
    }
    public static Vector2 RandomPointOnRectangle(Vector2 min, Vector2 max)
    {
        float x = Random.Range(min.x, max.x);
        float y = Random.Range(min.y, max.y);
        return new Vector2(x, y);
    }
    public static Vector3 RandomPointOnBox(Vector3 min, Vector3 max)
    {
        float x = Random.Range(min.x, max.x);
        float y = Random.Range(min.y, max.y);
        float z = Random.Range(min.z, max.z);
        return new Vector3(x, y, z);
    }
    public static Vector2 RandomPointOnLine(Vector2 start, Vector2 end)
    {
        float t = Random.Range(0f, 1f);
        return Vector2.Lerp(start, end, t);
    }
    public static Vector3 RandomPointOnLine(Vector3 start, Vector3 end)
    {
        float t = Random.Range(0f, 1f);
        return Vector3.Lerp(start, end, t);
    }
    public static Vector2 RandomPointOnTriangle(Vector2 a, Vector2 b, Vector2 c)
    {
        float u = Random.Range(0f, 1f);
        float v = Random.Range(0f, 1f);
        if (u + v > 1f)
        {
            u = 1f - u;
            v = 1f - v;
        }
        return a + u * (b - a) + v * (c - a);
    }
    public static Vector3 RandomPointOnTriangle(Vector3 a, Vector3 b, Vector3 c)
    {
        float u = Random.Range(0f, 1f);
        float v = Random.Range(0f, 1f);
        if (u + v > 1f)
        {
            u = 1f - u;
            v = 1f - v;
        }
        return a + u * (b - a) + v * (c - a);
    }
    public static Vector2 RandomPointOnPolygon(Vector2[] vertices)
    {
        int index = Random.Range(0, vertices.Length);
        return vertices[index];
    }
    public static Vector3 RandomPointOnPolygon(Vector3[] vertices)
    {
        int index = Random.Range(0, vertices.Length);
        return vertices[index];
    }
    public static Vector2 RandomPointOnEllipse(float a, float b)
    {
        float angle = Random.Range(0f, 2f * Mathf.PI);
        float x = a * Mathf.Cos(angle);
        float y = b * Mathf.Sin(angle);
        return new Vector2(x, y);
    }
    public static Vector3 RandomPointOnEllipsoid(float a, float b, float c)
    {
        float angle = Random.Range(0f, 2f * Mathf.PI);
        float x = a * Mathf.Cos(angle);
        float y = b * Mathf.Sin(angle);
        float z = c * Random.Range(-1f, 1f);
        return new Vector3(x, y, z);
    }
    public static Vector2 RandomPointOnCylinder(float radius, float height)
    {
        float angle = Random.Range(0f, 2f * Mathf.PI);
        float x = radius * Mathf.Cos(angle);
        float y = radius * Mathf.Sin(angle);
        float z = Random.Range(0f, height);
        return new Vector2(x, y);
    }
    
    private static uint seed;
}
