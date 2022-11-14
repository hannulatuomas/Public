using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class PathRequestManager : MonoBehaviour
{
    //Queue<PathRequest> pathRequestQueue= new Queue<PathRequest>();
    //PathRequest currentPathRequest;
    Queue<PathResult> results = new Queue<PathResult>();

    static PathRequestManager instance;
    Pathfinding pathfinding;

    //bool isProcessingPath;

    void Awake()
    {
        instance = this;
        pathfinding = GetComponent<Pathfinding>();
    }

    private void Update()
    {
        if (results.Count > 0)
        {
            int itemsInQueue = results.Count;
            lock (results)
            {
                for (int i = 0; i < itemsInQueue; i++)
                {
                    PathResult result = results.Dequeue();
                    result.callback(result.path, result.success);
                }
            }
        }
    }

    public static void RequestPath(PathRequest request)
    {
        //PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
        //instance.pathRequestQueue.Enqueue(newRequest);
        //instance.TryProcessNext();

        ThreadStart threadStart = delegate { instance.pathfinding.FindPath(request, instance.FinishedProcessingPath); };
        threadStart.Invoke();
    }

    //void TryProcessNext()
    //{
    //    if (!isProcessingPath && pathRequestQueue.Count > 0)
    //    {
    //        currentPathRequest = pathRequestQueue.Dequeue();
    //        isProcessingPath = true;
    //        pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
    //    }
    //}

    public void FinishedProcessingPath(PathResult result)
    {
        //currentPathRequest.callback(path, success);
        //isProcessingPath = false;
        //TryProcessNext();
        lock (results)
        {
            results.Enqueue(result);
        }
        
    }
}

public struct PathRequest
{
    public Vector2 pathStart;
    public Vector2 pathEnd;
    public Action<Vector2[], bool> callback;
    public PathRequest(Vector2 _start, Vector2 _end, Action<Vector2[], bool> _callback)
    {
        pathStart = _start;
        pathEnd = _end;
        callback = _callback;
    }
}
public struct PathResult
{
    public Vector2[] path;
    public bool success;
    public Action<Vector2[], bool> callback;

    public PathResult(Vector2[] _path, bool _success, Action<Vector2[], bool> _callback)
    {
        this.path = _path;
        this.success = _success;
        this.callback = _callback;
    }
}