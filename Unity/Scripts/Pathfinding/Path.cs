using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path
{
    public readonly Vector2[] lookPoints;
    public readonly TurnBoundary[] turnBoundaries;
    public readonly int finishLineIndex;

    public Path(Vector2[] waypoints, Vector2 startPos, float turnDistance)
    {
        lookPoints = waypoints;
        turnBoundaries = new TurnBoundary[lookPoints.Length];
        finishLineIndex = turnBoundaries.Length - 1;

        Vector2 previousPoint = startPos;

        for (int i = 0; i < lookPoints.Length; i++)
        {
            Vector2 currentPoint = lookPoints[i];
            Vector2 dirToCurrentPoint = (currentPoint - previousPoint).normalized;
            Vector2 turnBoundaryPoint = (i == finishLineIndex) ? currentPoint : currentPoint - dirToCurrentPoint * turnDistance;
            turnBoundaries[i] = new TurnBoundary(turnBoundaryPoint, previousPoint - dirToCurrentPoint * turnDistance);
            previousPoint = turnBoundaryPoint;
        }
    }
}
