using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Jobs;

public class UnitControlSystem : ComponentSystem
{
    private float3 startPosition;
    protected override void OnUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Mouse Pressed
            startPosition = (float3)GetMouseWorldPosition(Camera.main, 0);
            ECS_Manager.instance.selectionAreaTransform.gameObject.SetActive(true);
            ECS_Manager.instance.selectionAreaTransform.position = startPosition;
        }
        if (Input.GetMouseButton(0))
        {
            float3 selectionAreaSize = (float3)GetMouseWorldPosition(Camera.main, 0) - startPosition;
            ECS_Manager.instance.selectionAreaTransform.localScale = selectionAreaSize;
        }
        if (Input.GetMouseButtonUp(0))
        {
            // Mouse Released
            ECS_Manager.instance.selectionAreaTransform.gameObject.SetActive(false);
            float3 endPosition = GetMouseWorldPosition(Camera.main, 0);

            float3 lowerLeftPosition = new float3((float)System.Math.Round(math.min(startPosition.x, endPosition.x), 2), (float)System.Math.Round(math.min(startPosition.y, endPosition.y), 2), 0);
            float3 upperRightPosition = new float3((float)System.Math.Round(math.max(startPosition.x, endPosition.x), 2 ), (float)System.Math.Round(math.max(startPosition.y, endPosition.y), 2 ), 0);

            bool selectOnlyOneEntity = false;
            float selectionAreaSizeMin = 1.5f;
            float selectionAreaSize = math.distance(lowerLeftPosition, upperRightPosition);

            if (selectionAreaSize < selectionAreaSizeMin)
            {
                lowerLeftPosition += new float3(-1, -1, 0) * (selectionAreaSizeMin - selectionAreaSize) * 0.5f;
                upperRightPosition += new float3(1, 1, 0) * (selectionAreaSizeMin - selectionAreaSize) * 0.5f;
                selectOnlyOneEntity = true;
            }

            Entities.WithAll<UnitSelected>().ForEach((Entity entity) => {
                PostUpdateCommands.RemoveComponent<UnitSelected>(entity);
            });

            int selectedEntityCount = 0;
            Entities.ForEach((Entity entity, ref Translation translation, ref MoveTo moveTo) => {
                if (selectOnlyOneEntity == false || selectedEntityCount < 1)
                {
                    float3 entityPosition = new float3((float)System.Math.Round(translation.Value.x, 2), (float)System.Math.Round(translation.Value.y, 2), (float)System.Math.Round(translation.Value.z, 2));
                    if (entityPosition.x >= lowerLeftPosition.x && entityPosition.y >= lowerLeftPosition.y && entityPosition.x <= upperRightPosition.x && entityPosition.y <= upperRightPosition.y)
                    {
                        PostUpdateCommands.AddComponent(entity, new UnitSelected());
                    }
                }
                selectedEntityCount++;
            });
        }

        if (Input.GetMouseButtonDown(1))
        {
            // Right mouse button down
            float3 targetPosition = GetMouseWorldPosition(Camera.main, 0);
            List<float3> movePositionList = GetPositionsAround(targetPosition, 200);
            int positionIndex = 0;
            Entities.WithAll<UnitSelected>().ForEach((Entity entity, ref MoveTo moveTo) => {
                moveTo.position = movePositionList[positionIndex];
                positionIndex = (positionIndex + 1) % movePositionList.Count;
                moveTo.move = true;
            });
        }
    }

    private List<float3> GetPositionsAround(float3 position, int count)
    {
        List<float3> positionList = new List<float3>();
        float3 addX = new float3(1f, 0, 0);
        float3 addY = new float3(0, 1f, 0);

        int rows = (int)math.floor(math.sqrt((float)count));
        int columns = (int)math.ceil((float)count / rows);

        int rowIndex = 0;
        for (int y = 0; y < rows; y++)
        {
            int columnIndex = 0;

            for (int x = 0; x < columns && positionList.Count < count; x++)
            {
                if (columnIndex <= 0)
                {
                    positionList.Add(position + columnIndex * addX + rowIndex * addY);
                    columnIndex = math.abs(columnIndex) + 1;
                }
                else
                {
                    positionList.Add(position + columnIndex * addX + rowIndex * addY);
                    columnIndex *= -1;
                }
            }

            if (rowIndex <= 0)
            {
                rowIndex = math.abs(rowIndex) + 1;
            }
            else
            {
                rowIndex *= -1;
            }
        }

        return positionList;
    }

    private Vector3 GetMouseWorldPosition(Camera worldCamera, float z = 0)
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPosition.z = z;
        return worldPosition;
    }
}

public class UnitSelectedRenderer : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<UnitSelected>().ForEach((ref Translation translation) => {
            float3 position = translation.Value - new float3(0, 0.4f, 0);
            Graphics.DrawMesh(ECS_Manager.instance.unitSelectedMesh, position, Quaternion.identity, ECS_Manager.instance.unitSelectedMaterial, 0);
        });
    }
}

public struct UnitSelected : IComponentData { }

public struct MoveTo : IComponentData
{
    public bool move;
    public float3 position;
    public float3 lastMoveDir;
    public float moveSpeed;
}

// Unit go to Move Position
public class UnitMoveSystem : JobComponentSystem
{

    private struct Job : IJobForEachWithEntity<MoveTo, Translation>
    {

        public float deltaTime;

        public void Execute(Entity entity, int index, ref MoveTo moveTo, ref Translation translation)
        {
            if (moveTo.move)
            {
                float reachedPositionDistance = 0.1f;
                if (math.distance(translation.Value, moveTo.position) > reachedPositionDistance)
                {
                    // Far from target position, Move to position
                    float3 moveDir = math.normalize(moveTo.position - translation.Value);
                    moveTo.lastMoveDir = moveDir;
                    translation.Value += moveDir * moveTo.moveSpeed * deltaTime;
                }
                else
                {
                    // Already there
                    moveTo.move = false;
                }
            }
        }

    }
    
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        Job job = new Job
        {
            deltaTime = Time.deltaTime,
        };
        return job.Schedule(this, inputDeps);
    }
    
}
