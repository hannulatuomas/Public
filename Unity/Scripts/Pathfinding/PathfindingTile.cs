using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingTile : IHeapItem<PathfindingTile>
{
    public bool walkable;
    public Vector2 position;
    public int movementPenalty = 10;

    public int gCost;
    public int hCost;
    public PathfindingTile parent;
    int heapIndex;

    public PathfindingTile(bool _walkable, Vector2 _positionInGrid, int _movementPenalty = 10)
    {
        walkable = _walkable;
        position = _positionInGrid;
        movementPenalty = _movementPenalty;
    }

    public int fCost
    {
        get { return gCost + hCost; }
    }

    public int HeapIndex
    {
        get { return heapIndex; }
        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(PathfindingTile tileToCompare)
    {
        int compare = fCost.CompareTo(tileToCompare.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(tileToCompare.hCost);
        }
        return -compare;
    }
}
