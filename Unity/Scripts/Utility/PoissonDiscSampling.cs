using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PoissonDiscSampling
{
    // Poisson disc samples with integers !!
    public static List<Vector2> GeneratePoints(float radius, Vector2 sampleRegionSize, int numSamplesBeforeRejection)
    {
        Vector2 startPoint = sampleRegionSize / 2; 
        float cellSize = radius / 1.4f;
        int[,] grid = new int[Mathf.CeilToInt(sampleRegionSize.x / cellSize), Mathf.CeilToInt(sampleRegionSize.y / cellSize)];
        List<Vector2> points = new List<Vector2>();
        List<Vector2> spawnPoints = new List<Vector2>();

        spawnPoints.Add(startPoint);
        while (spawnPoints.Count > 0)
        {
            int spawnIndex = Random.Range(0,spawnPoints.Count);
            float pointRadius = Random.Range(radius/2, 2*radius); // Only with integers !!
            Vector2 spawnCenter = spawnPoints[spawnIndex];
            bool candidateAccepted = false;

            for (int i = 0; i < numSamplesBeforeRejection; i++)
            {
                float angle = Random.value * Mathf.PI * 2;
                Vector2 direction = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
                int candidateX = Mathf.FloorToInt(spawnCenter.x + direction.x * Random.Range(radius, 2 * radius));  // int !!
                int candidateY = Mathf.FloorToInt(spawnCenter.y + direction.y * Random.Range(radius, 2 * radius));  // int !!
                Vector2 candidate = new Vector2(candidateX, candidateY);

                if (IsValid(candidate, sampleRegionSize, cellSize, pointRadius, points, grid))
                {
                    points.Add(candidate);
                    spawnPoints.Add(candidate);
                    grid[(int)(candidate.x / cellSize), (int)(candidate.y / cellSize)] = points.Count; // First index = 1 !!
                    candidateAccepted = true;
                    break;
                }
            }
            if (!candidateAccepted)
            {
                spawnPoints.RemoveAt(spawnIndex);
            }
        }
        return points;
    }

    static bool IsValid(Vector2 candidate, Vector2 sampleRegionSize, float cellSize, float radius, List<Vector2> points, int[,] grid)
    {
        if (candidate.x >= 0 && candidate.x < sampleRegionSize.x && candidate.y >= 0 && candidate.y < sampleRegionSize.y)
        {
            int cellX = (int)(candidate.x / cellSize);
            int cellY = (int)(candidate.y / cellSize);
            int searchStartX = Mathf.Max(0, cellX - (1 + Mathf.CeilToInt(0.7f * radius - cellSize)));         // Mathf.Max(0, cellX - 2);
            int searchEndX = Mathf.Min(cellX + (1 + Mathf.CeilToInt(0.7f * radius - cellSize)), grid.GetLength(0) - 1); // int !!
            int searchStartY = Mathf.Max(0, cellY - (1 + Mathf.CeilToInt(0.7f * radius - cellSize)));         // Mathf.Max(0, cellY - 2)
            int searchEndY = Mathf.Min(cellY + (1 + Mathf.CeilToInt(0.7f * radius - cellSize)), grid.GetLength(1) - 1); // int !!

            for (int x = searchStartX; x < searchEndX; x++)
            {
                for (int y = searchStartY; y < searchEndY; y++)
                {
                    int pointIndex = grid[x, y] - 1;
                    if (pointIndex != -1)
                    {
                        float sqrDistance = (candidate - points[pointIndex]).sqrMagnitude;
                        if (sqrDistance < radius * radius)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }
}
