using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GenerateNoiseMap
{
    public static float[,] GenerateRandomNoiseMap(int sizeX, int sizeY, PerlinNoiseData[] perlinNoises)
    {
        float[,] noiseMap = new float[sizeX, sizeY];

        for (int n = 0; n < perlinNoises.Length; n++)
        {
            System.Random random = new System.Random(perlinNoises[n].seed);
            Vector2[] octaveOffsets = new Vector2[perlinNoises[n].octaves];
            for (int i = 0; i < perlinNoises[n].octaves; i++)
            {
                float offsetX = random.Next(-100000, 100000) + perlinNoises[n].offset.x;
                float offsetY = random.Next(-100000, 100000) + perlinNoises[n].offset.y;
                octaveOffsets[i] = new Vector2(offsetX, offsetY);
            }

            float maxNoiseHeight = float.MinValue;
            float minNoiseHeight = float.MaxValue;

            float halfWidth = sizeX / 2f;
            float halfHeight = sizeY / 2f;

            // Loop through pixels
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    float amplitude = perlinNoises[n].amplitude;                   
                    float frequency = perlinNoises[n].frequency;
                    float noiseHeight = 0;      // First time per pixel

                    for (int i = 0; i < perlinNoises[n].octaves; i++)
                    {
                        float sampleX = (x - halfWidth) / perlinNoises[n].Scale * frequency + octaveOffsets[i].x;
                        float sampleY = (y - halfHeight) / perlinNoises[n].Scale * frequency + octaveOffsets[i].y;

                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                        noiseHeight += perlinValue * amplitude;

                        amplitude *= perlinNoises[n].persistance;
                        frequency *= perlinNoises[n].Lacunarity;

                    }
                    // Add height to map
                    noiseMap[x, y] += noiseHeight;
                    // Update Max/Min values
                    if (noiseMap[x, y] > maxNoiseHeight)
                    {
                        maxNoiseHeight = noiseMap[x, y];
                    }
                    if (noiseMap[x, y] < minNoiseHeight)
                    {
                        minNoiseHeight = noiseMap[x, y];
                    }
                }
            }
            //Normalize values
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
                }
            }
        }

        return noiseMap;
    }

    public static float[,] GenerateUniformNoiseMap(int sizeX, int sizeY, float offset)
    {
        float[,] noiseMap = new float[sizeX, sizeY];
        float yCenter = sizeY / 2f;

        for (int y = 0; y < sizeY; y++)
        {
            float sampleY = y + offset;

            float noise = Mathf.Abs(sampleY - yCenter) / yCenter;

            for (int x = 0; x < sizeX; x++)
            {
                noiseMap[x, y] = 1 - noise;
            }
        }

        return noiseMap;
    }
}
