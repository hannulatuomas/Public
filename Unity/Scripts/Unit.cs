using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    const float pathUpdateMoveThreshold = 0.5f;
    const float minPathUpdateTime = 0.2f;

    public Transform target;
    public float speed = 1;
    public float turnSpeed = 3;
    public float turnDistance = 1;
    public float stoppingDistance = 5;

    Path path;
    // int targetIndex;

    private void Start()
    {
        StartCoroutine(UpdatePath());
        //PathRequestManager.RequestPath(new PathRequest(new Vector2(transform.position.x, transform.position.y), new Vector2(target.position.x, target.position.y), OnPathFound));
    }

    public void OnPathFound(Vector2[] waypoints, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = new Path(waypoints, pos2D(transform), turnDistance);
            //targetIndex = 0;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
        else
        {
            StopCoroutine(UpdatePath());
            StartCoroutine(UpdatePath());
        }
    }

    IEnumerator UpdatePath()
    {
        if (Time.timeSinceLevelLoad < 0.3f)
        {
            yield return new WaitForSeconds(0.3f);
        }

        PathRequestManager.RequestPath(new PathRequest(pos2D(transform), pos2D(target), OnPathFound));
        float sqrtMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
        Vector3 targetPosOld = target.position;

        while (true)
        {
            yield return new WaitForSeconds(minPathUpdateTime);
            if ((target.position - targetPosOld).sqrMagnitude > sqrtMoveThreshold)
            {
                PathRequestManager.RequestPath(new PathRequest(pos2D(transform), pos2D(target), OnPathFound));
                targetPosOld = target.position;
            }
        }
    }

    IEnumerator FollowPath()
    {
        bool followingPath = true;
        int pathIndex = 0;
        //transform.LookAt(path.lookPoints[0]);
        //Vector2 currentWaypoint = path[0];
        float speedPercent = 1;

        while (followingPath)
        {
            Vector2 position = pos2D(transform);
            while (path.turnBoundaries[pathIndex].HasCrossedLine(position))
            {
                if (pathIndex == path.finishLineIndex)
                {
                    followingPath = false;
                    break;
                }
                else
                {
                    pathIndex++;
                }
            }

            if (followingPath)
            {
                //Quaternion targetRotation = Quaternion.LookRotation(path.lookPoints[pathIndex] - position);
                //transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
                //transform.Translate(Vector3.forward * Time.deltaTime * speed, Space.Self);
                transform.position = Vector3.MoveTowards(pos2D(transform), path.lookPoints[pathIndex], Time.deltaTime * speed);
            }
            //transform.position = Vector3.MoveTowards(new Vector3(transform.position.x, transform.position.y, 0), new Vector3(currentWaypoint.x, currentWaypoint.y, 0), speed);
            yield return null;
        }
    }

    public Vector2 pos2D(Transform transform)
    {
        return new Vector2(transform.position.x, transform.position.y);
    }
}
