using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Mathematics;

public class ECS_Manager : MonoBehaviour
{
    public static ECS_Manager instance;

    public int unitCount = 10;

    [SerializeField] private CameraFollow cameraFollow;
    [SerializeField] float cameraMoveSpeed = 20;
    [SerializeField] float zoomSpeed = 100;
    [SerializeField] float cameraFollowZoom = 25;
    [SerializeField] float zoomMin = 5;
    [SerializeField] float zoomMax = 50;

    private Vector3 cameraFollowPosition;

    private Mesh mesh;
    [SerializeField] private Material material;
    public Mesh unitSelectedMesh;
    [SerializeField] public Material unitSelectedMaterial;
    public Transform selectionAreaTransform;

    EntityManager entityManager;

    private void Awake()
    {
        instance = this;
        mesh = CreateMesh(1f, 1f);
    }

    void Start()
    {
        entityManager = World.Active.EntityManager;
        for (int i = 0; i < unitCount; i++)
        {
            SpawnUnit(new float3(i+1, 0, 0), 2f);
        }
        SpawnUnit(new float3(0, 0, 0), 2f);

        unitSelectedMesh = CreateMesh(0.7f, 0.35f);

        cameraFollow.Setup(() => cameraFollowPosition, () => cameraFollowZoom, true, true);
    }

    public static Mesh CreateMesh(float meshWidth, float meshHeight)
    {
        Vector3[] vertices = new Vector3[4];
        Vector2[] uv = new Vector2[4];
        int[] triangles = new int[6];

        vertices[0] = new Vector3(-meshWidth / 2f, meshHeight / 2f);
        vertices[1] = new Vector3(meshWidth / 2f, meshHeight / 2f);
        vertices[2] = new Vector3(-meshWidth / 2f, -meshHeight / 2f);
        vertices[3] = new Vector3(meshWidth / 2f, -meshHeight / 2f);

        uv[0] = new Vector2(0, 1);
        uv[1] = new Vector2(1, 1);
        uv[2] = new Vector2(0, 0);
        uv[3] = new Vector2(1, 0);

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;
        triangles[3] = 2;
        triangles[4] = 1;
        triangles[5] = 3;

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        return mesh;
    }

    private void SpawnUnit(float3 spawnPosition, float speed)
    {
        EntityArchetype entityArchetype = entityManager.CreateArchetype(
            typeof(RenderMesh),
            typeof(LocalToWorld),
            typeof(Translation),
            typeof(MoveTo)
        );

        Entity entity = entityManager.CreateEntity(entityArchetype);

        entityManager.SetComponentData(entity, new Translation { Value = spawnPosition });
        entityManager.SetSharedComponentData(entity, new RenderMesh { mesh = mesh, material = material });
        entityManager.SetComponentData(entity, new MoveTo { move = true, position = spawnPosition, moveSpeed = speed });
    }

    private void Update()
    {
        HandleCamera();
    }

    private void HandleCamera()
    {
        Vector3 moveDir = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) { moveDir.y = +1f; }
        if (Input.GetKey(KeyCode.S)) { moveDir.y = -1f; }
        if (Input.GetKey(KeyCode.A)) { moveDir.x = -1f; }
        if (Input.GetKey(KeyCode.D)) { moveDir.x = +1f; }

        moveDir = moveDir.normalized;
        cameraFollowPosition += moveDir * cameraMoveSpeed * Time.deltaTime;

        if (Input.mouseScrollDelta.y > 0) cameraFollowZoom -= 1 * zoomSpeed * Time.deltaTime;
        if (Input.mouseScrollDelta.y < 0) cameraFollowZoom += 1 * zoomSpeed * Time.deltaTime;

        cameraFollowZoom = Mathf.Clamp(cameraFollowZoom, zoomMin, zoomMax);
    }
}
