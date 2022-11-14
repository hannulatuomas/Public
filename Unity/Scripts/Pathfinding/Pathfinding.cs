using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Pathfinding : MonoBehaviour
{
    int sizeX = MapBase.SizeX;  // WARNING !!
    int sizeY = MapBase.SizeY;  // grid.gridSize.x should be same
    int startTileX;
    int startTileY;
    int targetTileX;
    int targetTileY;

    PathfindingGrid grid = new PathfindingGrid();
    PathfindingTile[,] pathfindingGrid;
    List<PathfindingTile> path;
    // PathRequestManager requestManager;

    private void Awake()
    {
        // requestManager = GetComponent<PathRequestManager>();
        UpdatePathfindingGrid();
    }
    void UpdatePathfindingGrid()
    {
        grid.CreateGrid();
        pathfindingGrid = grid.pathfindingGrid;
    }
    //public void StartFindPath(Vector2 pathStart, Vector2 pathEnd)
    //{
    //    StartCoroutine(FindPath(pathStart, pathEnd));
    //}
    public void FindPath(PathRequest request, Action<PathResult> callback)
    {
        PathfindingTile failedTileStart = pathfindingGrid[0, 0];
        PathfindingTile failedTileTarget = pathfindingGrid[0, 0];
        PathfindingTile currentTile = pathfindingGrid[0, 0];

        findPathStart:

        Vector2[] waypoints = new Vector2[0];
        bool pathSuccess = false;

        PathfindingTile startTile = grid.TileFromWorldPosition(request.pathStart);
        startTileX = Mathf.Clamp((int)startTile.position.x, 0, sizeX-1);
        startTileY = Mathf.Clamp((int)startTile.position.y, 0, sizeY-1);
        PathfindingTile targetTile = grid.TileFromWorldPosition(request.pathEnd);
        targetTileX = Mathf.Clamp((int)targetTile.position.x, 0, sizeX - 1);
        targetTileY = Mathf.Clamp((int)targetTile.position.y, 0, sizeY - 1);
        if (startTile.position.x < 0 || startTile.position.y < 0 || startTile.position.x > sizeX || startTile.position.y > sizeY)
        {
            startTile = pathfindingGrid[startTileX, startTileY];
        }
        if (targetTile.position.x < 0 || targetTile.position.y < 0 || targetTile.position.x > sizeX || targetTile.position.y > sizeY)
        {
            targetTile = pathfindingGrid[targetTileX, targetTileY];
        }

        if (startTile.walkable && targetTile.walkable)
        {
            Heap<PathfindingTile> openSet = new Heap<PathfindingTile>(grid.MaxSize);
            HashSet<PathfindingTile> closedSet = new HashSet<PathfindingTile>();
            openSet.Add(startTile);

            while (openSet.Count > 0)
            {
                currentTile = openSet.RemoveFirst();
                closedSet.Add(currentTile);
                if (currentTile == targetTile)
                {
                    // Path found
                    pathSuccess = true;
                    //Debug.Log("Path found");
                    break;
                }

                foreach (PathfindingTile neighbour in grid.GetNeighbours(currentTile))
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    int newMovementCostToNeighbour = currentTile.gCost + GetDistance(currentTile, neighbour) + neighbour.movementPenalty;

                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetTile);
                        neighbour.parent = currentTile;

                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                        else
                        {
                            openSet.UpdateItem(neighbour);
                        }
                    }
                }
            }
        }
        //yield return null;
        if (pathSuccess)
        {
            waypoints = RetracePath(startTile, targetTile);
            pathSuccess = waypoints.Length > 0;
            if (!pathSuccess)
            {
                Debug.LogError("No Waypoints !");
            }
        }
        else
        {
            if (Mathf.RoundToInt(startTile.position.x) == Mathf.RoundToInt(failedTileStart.position.x) && Mathf.RoundToInt(startTile.position.y) == Mathf.RoundToInt(failedTileStart.position.y))
            {
                if (Mathf.RoundToInt(targetTile.position.x) == Mathf.RoundToInt(failedTileTarget.position.x) && Mathf.RoundToInt(targetTile.position.y) == Mathf.RoundToInt(failedTileTarget.position.y))
                {
                    Debug.Log("StartPos = " + startTile.position.x.ToString() + ", " + startTile.position.y.ToString());
                    Debug.Log("TargetPos = " + targetTile.position.x.ToString() + ", " + targetTile.position.y.ToString());
                    Debug.LogError("No Path ? Is start walkable? " + startTile.walkable.ToString() + ", Is target walkable? " + targetTile.walkable.ToString());
                }
                else
                {
                    Debug.LogError("Failed to find a path to " + targetTile.position.x.ToString() + ", " + targetTile.position.y.ToString() + ". Try again.");
                    failedTileTarget = targetTile;
                    goto findPathStart;
                }
            }
            else
            {
                Debug.LogError("Failed to find a path from " + startTile.position.x.ToString() + ", " + startTile.position.y.ToString() + ". Try again.");
                failedTileStart = startTile;
                goto findPathStart;
            }
        }
        // requestManager.FinishedProcessingPath(waypoints, pathSuccess);
        callback(new PathResult(waypoints, pathSuccess, request.callback));
    }

    Vector2[] RetracePath(PathfindingTile startTile, PathfindingTile targetTile)
    {
        path = new List<PathfindingTile>();
        PathfindingTile checkedTile = targetTile;

        while (checkedTile != startTile)
        {
            path.Add(checkedTile);
            checkedTile = checkedTile.parent;
        }
        Vector2[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        return waypoints; 
    }
    Vector2[] SimplifyPath(List<PathfindingTile> path)
    {
        List<Vector2> waypoints = new List<Vector2>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i-1].position.x - path[i].position.x, path[i - 1].position.y - path[i].position.y);
            if (directionNew != directionOld)
            {
                path[i].position = grid.WorldPositionFromTile(path[i].position);
                waypoints.Add(path[i].position);
            }
        }

        return waypoints.ToArray();
    }
    int GetDistance(PathfindingTile tileA, PathfindingTile tileB)
    {
        int distanceX = Mathf.Abs(Mathf.RoundToInt(tileA.position.x - tileB.position.x));
        int distanceY = Mathf.Abs(Mathf.RoundToInt(tileA.position.y - tileB.position.y));
        int costMultiplyVertical = 10;
        int costMultiplyDiagonal = 14;

        if (distanceX > distanceY)
        {
            return costMultiplyDiagonal * distanceY + costMultiplyVertical * (distanceX-distanceY);
        }
        else
        {
            return costMultiplyDiagonal * distanceX + costMultiplyVertical * (distanceY - distanceX);
        }
    }
}
