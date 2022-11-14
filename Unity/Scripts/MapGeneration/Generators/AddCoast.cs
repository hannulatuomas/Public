using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AddCoast
{
    public static float[,] AddCoastToNoisemap(float[,] noiseMap, int coastDistance, HeightMapData.CoastDirection direction)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        
        switch (direction)
        {
            case HeightMapData.CoastDirection.North:
                if (height > coastDistance)
                {
                    for (int x = 0; x < width; x++)
                    {
                        float coastValue = noiseMap[x, height - coastDistance];
                        float lerpIndex = 1 / (float)coastDistance;

                        for (int y = height - coastDistance; y < height; y++)
                        {
                            noiseMap[x, y] = Mathf.Lerp(coastValue, 0f, lerpIndex);
                            lerpIndex += 1 / (float)coastDistance;
                        }
                    }
                }
                else if (width < coastDistance || height < coastDistance)
                {
                    return noiseMap;
                }
                break;
            case HeightMapData.CoastDirection.East:
                if (width > coastDistance)
                {
                    for (int y = 0; y < width; y++)
                    {
                        float coastValue = noiseMap[width - coastDistance, y];
                        float lerpIndex = 1 / (float)coastDistance;

                        for (int x = width - coastDistance; x < width; x++)
                        {
                            noiseMap[x, y] = Mathf.Lerp(coastValue, 0f, lerpIndex);
                            lerpIndex += 1 / (float)coastDistance;
                        }
                    }
                }
                else if (width < coastDistance || height < coastDistance)
                {
                    return noiseMap;
                }
                break;
            case HeightMapData.CoastDirection.South:
                if (height > coastDistance)
                {
                    for (int x = 0; x < width; x++)
                    {
                        float coastValue = noiseMap[x, coastDistance];
                        float lerpIndex = 1 / (float)coastDistance;

                        for (int y = coastDistance; y >= 0; y--)
                        {
                            noiseMap[x, y] = Mathf.Lerp(coastValue, 0f, lerpIndex);
                            lerpIndex += 1 / (float)coastDistance;
                        }
                    }
                }
                else if (width < coastDistance || height < coastDistance)
                {
                    return noiseMap;
                }
                break;
            case HeightMapData.CoastDirection.West:
                if (width > coastDistance)
                {
                    for (int y = 0; y < width; y++)
                    {
                        float coastValue = noiseMap[coastDistance, y];
                        float lerpIndex = 1 / (float)coastDistance;

                        for (int x = coastDistance; x >= 0; x--)
                        {
                            noiseMap[x, y] = Mathf.Lerp(coastValue, 0f, lerpIndex);
                            lerpIndex += 1 / (float)coastDistance;
                        }
                    }
                }
                else if (width < coastDistance || height < coastDistance)
                {
                    return noiseMap;
                }
                break;
            default:
                break;
        }

        return noiseMap;
    }
}
