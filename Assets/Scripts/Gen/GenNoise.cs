using UnityEngine;

public static class GenNoise
{
    // static float Noise(float x, float y, float scale ,int octaves,  float persistence, float lacunarity, float amplitude,float frequency)
    // {
    //     float total = 0;
    //     float maxValue = 0;
    //     System.Random prng = new System.Random((int)GenRandom.GetSeed());
    //     float offsetX = prng.Next(-100000, 100000);
    //     float offsetY = prng.Next(-100000, 100000);
        
    //     for (int i = 0; i < octaves; i++)
    //     {
    //         total +=  Mathf.PerlinNoise((x + offsetX) * frequency, (y + offsetY) * frequency);
    //         maxValue += amplitude;
    //         amplitude *= persistence;    // Giảm độ mạnh qua tầng
    //         frequency *= lacunarity;     // Tăng độ chi tiết qua tầng
    //     }

    //     return total / maxValue;
    // }
    public static float[,] GenerateNoiseForChunk(Chunk chunk,float scale ,int octaves,  float persistence, float lacunarity, float amplitude,float frequency)
    {
        float[,] noise = new float[OverworldData.chunkSize, OverworldData.chunkSize];
        System.Random prng = new System.Random((int)GenRandom.GetSeed());
        float offsetX = prng.Next(-100000, 100000) + chunk.transform.position.x;
        float offsetY = prng.Next(-100000, 100000) + chunk.transform.position.y;
        for (int x = 0; x < OverworldData.chunkSize; x++)
        {
            for (int y = 0; y < OverworldData.chunkSize; y++)
            {
                float total = 0;
                float maxValue = 0;
                float amplitudeTemp = amplitude;
                float frequencyTemp = frequency;
                for (int i = 0; i < octaves; i++)
                {
                    total += Mathf.PerlinNoise((x + offsetX) * frequencyTemp / scale, (y + offsetY) * frequencyTemp / scale) * amplitudeTemp;
                    maxValue += amplitudeTemp;
                    amplitudeTemp *= persistence;    // Giảm độ mạnh qua tầng
                    frequencyTemp *= lacunarity;     // Tăng độ chi tiết qua tầng
                }
                noise[x, y] = total / maxValue;
            }
        }
        return noise;
    }
    public static float[,] PerlinNoiseMap(Vector2 offset, int width, int height,int seed, float scale ,int octaves, float persistence, float lacunarity, float amplitude, float frequency)
    {
        float[,] noise = new float[width, height];
        System.Random prng = new System.Random(seed);

        Vector2[] octaveOffsets = new Vector2[octaves];
        for(int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        if(scale <= 0)
        {
            scale = 0.0001f;
        }
        float halfWidth = width / 2f;
        float halfHeight = height / 2f;

        float minNoiseValue = float.MaxValue;
        float maxNoiseValue = float.MinValue;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float amplitudeTemp = amplitude;
                float frequencyTemp = frequency;
                float noiseValue = 0;
                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth) / scale * frequencyTemp + octaveOffsets[i].x * frequencyTemp;
                    float sampleY = (y - halfHeight) / scale * frequencyTemp + octaveOffsets[i].y * frequencyTemp;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1; // Normalize to [-1, 1]
                    noiseValue += perlinValue * amplitudeTemp;

                    amplitudeTemp *= persistence;
                    frequencyTemp *= lacunarity;
                }

                if (noise[x, y] < minNoiseValue)
                {
                    minNoiseValue = noise[x, y];
                }
                if (noise[x, y] > maxNoiseValue)
                {
                    maxNoiseValue = noise[x, y];
                }

                noise[x, y] = noiseValue;
            }
        }

        // Normalize the noise values to [0, 1]
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                noise[x, y] = Mathf.InverseLerp(minNoiseValue, maxNoiseValue, noise[x, y]);
            }
        }
        return noise;
    }
}
