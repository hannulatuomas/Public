using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingGrid
{
    float maxWalkableHeight = 0.7f;
    float minWalkableHeight = 0.3f;
    Vector2 gridSize = new Vector2(MapBase.SizeX, MapBase.SizeY);
    Vector3 centerPosition = new Vector3(0, 0, 0);
    public PathfindingTile[,] pathfindingGrid;
    float[,] heightMap = MapBase.HeightMapBase;
    int blurSize = 3;
    public void CreateGrid()
    {
        pathfindingGrid = new PathfindingTile[(int)gridSize.x, (int)gridSize.y];
        if (heightMap == null)
        {
            heightMap = MapBase.HeightMapBase;
        }
        for (int x = 0; x < (int)gridSize.x; x++)
        {
            for (int y = 0; y < (int)gridSize.y; y++)
            {
                Vector2 position = new Vector2(x,y);
                bool walkable = IsWalkable(heightMap[x, y]);
                int movementPenalty = (int)(100 * Mathf.Abs(heightMap[x, y] - 0.5f));
                pathfindingGrid[x, y] = new PathfindingTile(walkable, position, movementPenalty);
            }
        }

        BlurPanaltyMap(blurSize);
    }
    void BlurPanaltyMap(int blurSize)
    {
        int kernelSize = blurSize * 2 - 1;
        int kernelExtents = (kernelSize - 1) / 2;

        int[,] penaltiesHorizontalPass = new int[(int)gridSize.x, (int)gridSize.y];
        int[,] penaltiesVerticalPass = new int[(int)gridSize.x, (int)gridSize.y];
        // HorizontalPass
        for (int y = 0; y < (int)gridSize.y; y++)
        {
            for (int x = -kernelExtents; x <= kernelExtents; x++)
            {
                int sampleX = Mathf.Clamp(x,0,kernelExtents);
                penaltiesHorizontalPass[0, y] += pathfindingGrid[sampleX, y].movementPenalty;
            }
            for (int x = 1; x < (int)gridSize.x; x++)
            {
                int removeIndex = Mathf.Clamp(x - kernelExtents - 1, 0, (int)gridSize.x);
                int addIndex = Mathf.Clamp(x + kernelExtents, 0, (int)gridSize.x - 1);

                penaltiesHorizontalPass[x, y] = penaltiesHorizontalPass[x - 1, y] - pathfindingGrid[removeIndex,y].movementPenalty + pathfindingGrid[addIndex,y].movementPenalty;
            }
        }
        // VerticalPass
        for (int x = 0; x < (int)gridSize.x; x++)
        {
            for (int y = -kernelExtents; y <= kernelExtents; y++)
            {
                int sampleY = Mathf.Clamp(y, 0, kernelExtents);
                penaltiesVerticalPass[x, 0] += penaltiesHorizontalPass[x, sampleY];
            }
            int blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, 0] / (kernelSize * kernelSize));
            pathfindingGrid[x, 0].movementPenalty = blurredPenalty;

            for (int y = 1; y < (int)gridSize.y; y++)
            {
                int removeIndex = Mathf.Clamp(y - kernelExtents - 1, 0, (int)gridSize.y);
                int addIndex = Mathf.Clamp(y + kernelExtents, 0, (int)gridSize.y - 1);

                penaltiesVerticalPass[x, y] = penaltiesVerticalPass[x, y - 1] - penaltiesHorizontalPass[x, removeIndex] + penaltiesHorizontalPass[x, addIndex];
                blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, y] / (kernelSize * kernelSize));
                pathfindingGrid[x, y].movementPenalty = blurredPenalty;
            }
        }
    }
    public List<PathfindingTile> GetNeighbours(PathfindingTile tile)
    {
        List<PathfindingTile> neighbours = new List<PathfindingTile>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }
                else
                {
                    int checkX = (int)tile.position.x + x;
                    int checkY = (int)tile.position.y + y;

                    if (checkX >= 0 && checkX < (int)gridSize.x && checkY >= 0 && checkY < (int)gridSize.y)
                    {
                        neighbours.Add(pathfindingGrid[checkX, checkY]);
                    }
                }
            }
        }
        return neighbours;
    }

    public PathfindingTile TileFromWorldPosition(Vector2 worldPosition)
    {
        float percentX = (worldPosition.x + gridSize.x / 2) / gridSize.x;
        float percentY = (worldPosition.y + gridSize.y / 2) / gridSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSize.x - 1) * percentX);
        int y = Mathf.RoundToInt((gridSize.y - 1) * percentY);
        return pathfindingGrid[x, y];
    }

    public Vector2 WorldPositionFromTile(Vector2 position)
    {
        float x = position.x;
        float y = position.y;

        float percentX = x / gridSize.x;
        float percentY = y / gridSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        x = centerPosition.x + (percentX - 0.5f) * gridSize.x;
        y = centerPosition.y + (percentY - 0.5f) * gridSize.y;

        return new Vector2(x, y);
    }

    bool IsWalkable(float _terrainHeight)
    {
        if (_terrainHeight > maxWalkableHeight || _terrainHeight < minWalkableHeight)
        {
            return true;
        }
        else
        {
            return true;
        }
    }
    public int MaxSize { get { return (int)gridSize.x * (int)gridSize.y; } }
}
