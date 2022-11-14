using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverGenerator
{
    public int numberOfRivers = 14;
    public float heightThreshold = 0.65f;
    public float waterLevel = 0.18f;
    public float riverDeepness = 0.05f;
    public int riverWidth = 5;
    public bool smooth = true;
    public int smoothFactor = 2;
    public List<Vector2> riverPixels;
    public List<Vector2> randomIndexes = new List<Vector2>();
    public Color riverColor = new Color( 0, 150, 225, 120);

    public float[,] GenerateRivers(float[,] terrainHightMap)
    {
        int width = terrainHightMap.GetLength(0);
        int height = terrainHightMap.GetLength(1);
        float[,] mapWithRivers = terrainHightMap;
        riverPixels = new List<Vector2>();

        for (int riverIndex = 0; riverIndex < numberOfRivers; riverIndex++)
        {
            // choose a origin for the river
            Vector3 riverOrigin = ChooseRiverOrigin(terrainHightMap);
            // build the river starting from the origin
            mapWithRivers = BuildRiver(mapWithRivers, riverOrigin);
        }
        return mapWithRivers;
    }

    private Vector2 ChooseRiverOrigin(float[,] terrainHightMap)
    {
        int randomXIndex = 0;
        int randomYIndex = 0;

        int width = terrainHightMap.GetLength(0);
        int height = terrainHightMap.GetLength(1);
        int terrainPixels = width * height;

        // iterates until finding a good river origin
        for (int i = 0; i < terrainPixels; i++)
        {
            if (i >= randomIndexes.Count)
            {
                // pick a random coordinate inside the level
                randomXIndex = Random.Range(0, width);
                randomYIndex = Random.Range(0, height);
                randomIndexes.Add(new Vector2(randomXIndex, randomYIndex));
            }
            else
            {
                randomXIndex = (int)randomIndexes[i].x;
                randomYIndex = (int)randomIndexes[i].y;
            }

            // if the height value of this coordinate is higher than the threshold, choose it as the river origin
            float heightValue = terrainHightMap[randomXIndex, randomYIndex];
            if (heightValue >= this.heightThreshold)
            {
                break;
            }
        }
        return new Vector2(randomXIndex, randomYIndex);
    }

    private float[,] BuildRiver(float[,] terrainHightMap, Vector2 riverOrigin)
    {
        HashSet<Vector2> visitedPixels = new HashSet<Vector2>();
        List<Vector2> smoothedPixels = new List<Vector2>();
        // the first coordinate is the river origin
        Vector2 currentPixel = riverOrigin;
        float[,] mapWithRivers = terrainHightMap;
        int width = terrainHightMap.GetLength(0);
        int height = terrainHightMap.GetLength(1);
        int terrainPixels = width * height;
        float heightValue;

        for (int i = 0; i < terrainPixels-1; i++)
        {
            heightValue = terrainHightMap[(int)currentPixel.x, (int)currentPixel.y];
            // save the current coordinate as visited
            visitedPixels.Add(currentPixel);
            riverPixels.Add(currentPixel);

            // check if we have found water
            if (heightValue <= waterLevel)
            {
                // if we found water, we stop
                break;
            }
            else
            {
                mapWithRivers[(int)currentPixel.x, (int)currentPixel.y] = mapWithRivers[(int)currentPixel.x, (int)currentPixel.y] -riverDeepness;

                // pick neighbor coordinates, if they exist
                List<Vector2> neighbors = new List<Vector2>();
                for (int j = 0; j < riverWidth; j++)
                {
                    
                    if (currentPixel.x > j && currentPixel.x - (1 - j) < width)
                    {
                        Vector2 target = new Vector2(currentPixel.x - (1 - j), currentPixel.y);
                        if (j == 0)
                        {
                            neighbors.Add(target);
                        }
                        if (!smoothedPixels.Contains(target) && smooth)
                        {
                            mapWithRivers[(int)currentPixel.x - (1 - j), (int)currentPixel.y] = mapWithRivers[(int)currentPixel.x - (1 - j), (int)currentPixel.y] - (1f / (smoothFactor + j)) * (mapWithRivers[(int)currentPixel.x - (1 - j), (int)currentPixel.y] - mapWithRivers[(int)currentPixel.x, (int)currentPixel.y]);
                            smoothedPixels.Add(target);
                        }
                    }
                    if (currentPixel.x < width - (1 + j) && currentPixel.x + (1 + j) > j)
                    {
                        Vector2 target = new Vector2(currentPixel.x + (1 + j), currentPixel.y);
                        if (j == 0)
                        {
                            neighbors.Add(target);
                        }
                        if (!smoothedPixels.Contains(target) && smooth)
                        {
                            mapWithRivers[(int)target.x, (int)target.y] = mapWithRivers[(int)target.x, (int)target.y] - (1f / (smoothFactor + j)) * (mapWithRivers[(int)target.x, (int)target.y] - mapWithRivers[(int)currentPixel.x, (int)currentPixel.y]);
                            smoothedPixels.Add(target);
                        }

                    }
                    if (currentPixel.y > j && currentPixel.y - (1 - j) < height)
                    {
                        Vector2 target = new Vector2(currentPixel.x, currentPixel.y - (1 - j));
                        if (j == 0)
                        {
                            neighbors.Add(target);
                        }
                        if (!smoothedPixels.Contains(target) && smooth)
                        {
                            mapWithRivers[(int)target.x, (int)target.y] = mapWithRivers[(int)target.x, (int)target.y] - (1f / (smoothFactor + j)) * (mapWithRivers[(int)target.x, (int)target.y] - mapWithRivers[(int)currentPixel.x, (int)currentPixel.y]);
                            smoothedPixels.Add(target);
                        }

                    }
                    if (currentPixel.y < height - (1 + j) && currentPixel.y + (1 + j) > j)
                    {
                        Vector2 target = new Vector2(currentPixel.x, currentPixel.y + (1 + j));
                        if (j == 0)
                        {
                            neighbors.Add(target);
                        }
                        if (!smoothedPixels.Contains(target) && smooth)
                        {
                            mapWithRivers[(int)target.x, (int)target.y] = mapWithRivers[(int)target.x, (int)target.y] - (1f / (smoothFactor + j)) * (mapWithRivers[(int)target.x, (int)target.y] - mapWithRivers[(int)currentPixel.x, (int)currentPixel.y]);
                            smoothedPixels.Add(target);
                        }
                    }
                }

                // find the minimum neighbor that has not been visited yet and flow to it
                float minHeight = float.MaxValue;
                Vector2 minNeighbor = new Vector2(0, 0);
                foreach (Vector2 neighbor in neighbors)
                {
                    // if the neighbor is the lowest one and has not been visited yet, save it
                    float neighborHeight = terrainHightMap[(int)neighbor.x, (int)neighbor.y];

                    if (neighborHeight < minHeight && !visitedPixels.Contains(neighbor))
                    {
                        minHeight = neighborHeight;
                        minNeighbor = neighbor;
                    }
                }
                if (minHeight < float.MaxValue)
                {
                    // flow to the lowest neighbor
                    currentPixel = minNeighbor;
                }
                else
                {
                    break;
                }
            }
        }
        return mapWithRivers;
    }
}
