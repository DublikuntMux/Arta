using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZenithInterceptor : MonoBehaviour
{
    enum AlgorithmType {
        AStar, Direct, ChangeHeight
    };

    public float startSpeed;
    public float rotationSpeed;
    public float acceleration;
    public Target target;

    public bool hasRemoteDetonator;
    public float detonatorShardSpeed;
    public float detonatorDistance;

    public Shards shardsPrefab;

    private bool hasReachedTheTarget;
    private float startTime;
    private float speed;
    private AlgorithmType algorithmType;

    private SceneController controller;
    private AStarAgent aStar;
    private LineRenderer lineRenderer;


    void Start()
    {
        controller = FindFirstObjectByType<SceneController>();
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        aStar = GetComponent<AStarAgent>();
        aStar.Speed = speed;
        aStar.TurnSpeed = rotationSpeed;
        aStar.Pathfinding(target.transform.position);
        startTime = controller.simulationTime;
        speed = startSpeed;
    }
    
    void Update()
    {
        if (hasReachedTheTarget || !controller.isShowingSimulation) return;
        speed = startSpeed + (controller.simulationTime - startTime) * acceleration;
        aStar.Speed = speed;
        if (hasRemoteDetonator && Vector3.Distance(transform.position, target.transform.position) <= detonatorDistance) {
            hasReachedTheTarget = true;
            Shards shards = Instantiate(shardsPrefab, transform.position, Quaternion.identity);
            shards.speed = detonatorShardSpeed;
            shards.target = target;
        }
        lineRenderer.SetPosition(lineRenderer.positionCount++, transform.position);
        if (!Physics.Linecast(transform.position, target.transform.position) && Physics.OverlapSphere(transform.position, 0).Length == 0) {
            if (algorithmType == AlgorithmType.AStar) {
                aStar.stopExecution();
                algorithmType = AlgorithmType.Direct;
            }
        } else if (algorithmType != AlgorithmType.AStar)
            algorithmType = AlgorithmType.AStar;
        if (algorithmType == AlgorithmType.AStar &&
            PathfindingWorldManager.Instance.GetClosestPointWorldSpace(target.transform.position).Coords != aStar._end.Coords)
        {
            aStar.Pathfinding(target.transform.position);
        }
        float upperBound = PathfindingWorldManager.Instance.getUpperBound();
        if (algorithmType == AlgorithmType.AStar && transform.position.y > upperBound ^ target.transform.position.y > upperBound) {
            aStar.stopExecution();
            algorithmType = AlgorithmType.ChangeHeight;
        }
        if (algorithmType != AlgorithmType.AStar) {
            Vector3 endRotation;
            if (algorithmType == AlgorithmType.Direct) endRotation = target.transform.position - transform.position;
            else if (algorithmType == AlgorithmType.ChangeHeight)
                if (target.transform.position.y >= PathfindingWorldManager.Instance.getUpperBound()) endRotation = Vector3.up;
                else endRotation = Vector3.down;
            else throw new System.NotImplementedException("Alogrithm type not implemented");
            transform.localRotation = Quaternion.LookRotation(Vector3.RotateTowards(
                transform.forward,
                endRotation,
                rotationSpeed * controller.getScaledTimeDelta() * Mathf.Deg2Rad, 0f
            ));
            transform.position += transform.forward * speed * controller.getScaledTimeDelta();
        }
        if (target.isInsideTheTarget(transform.position, target.transform.position)) {
            hasReachedTheTarget = true;
            FindFirstObjectByType<ZenithCannon>().interceptorsArrived++;
            target.endSimulation();
        }
        // if (aStar.Status == AStarAgentStatus.Finished) {
        //     if (PathfindingWorldManager.Instance.GetClosestPointWorldSpace(transform.position).Coords != aStar._end.Coords) {
        //         aStar.Pathfinding(target.transform.position);
        //         return;
        //     }
        //     transform.localRotation = Quaternion.LookRotation(Vector3.RotateTowards(
        //         transform.forward,
        //         target.transform.position - transform.position,
        //         rotationSpeed * controller.getScaledTimeDelta() * Mathf.Deg2Rad, 0f
        //     ));
        //     transform.position += transform.forward * speed * controller.getScaledTimeDelta();
        //     if (target.isInsideTheTarget(transform.position, target.transform.position)) controller.endSimulation();
        // }
    }
}