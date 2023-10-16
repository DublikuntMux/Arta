using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallisticInterceptor : MonoBehaviour
{
    public BallisticTarget target;
    public Vector3 velocity;
    public float mass;

    public bool hasRemoteDetonator;
    public bool hasSpawnedShards;
    public float detonatorShardSpeed;
    public float detonatorDistance;

    public float launchOffset;
    public float endTime;

    public Shards shardsPrefab;

    private SceneController controller;
    private LineRenderer lineRenderer;

    void Start()
    {
        controller = GameObject.FindFirstObjectByType<SceneController>();
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
    }

    void Update()
    {
        if (hasSpawnedShards) return;
        if (controller.simulationTime >= endTime && !target.hasEnded && !hasRemoteDetonator) target.endSimulation();
        float time = controller.simulationTime < endTime ? controller.simulationTime : endTime;
        lineRenderer.SetPosition(lineRenderer.positionCount++, transform.position);
        Vector3 acceleration = SceneController.gravityAcceleration * Vector3.down;
        transform.position = velocity * (time - launchOffset) +
            acceleration * Mathf.Pow(time - launchOffset, 2) / 2;
        if (hasRemoteDetonator && !hasSpawnedShards && Vector3.Distance(transform.position, target.transform.position) <= detonatorDistance) {
            hasSpawnedShards = true;
            Shards shards = Instantiate(shardsPrefab, transform.position, Quaternion.identity);
            shards.speed = detonatorShardSpeed;
            shards.target = target;
        }
    }
}
