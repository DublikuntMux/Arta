using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AerodynamicTarget : Target
{
    public List<Vector3> path;
    public Vector3 initialVelocity;
    public float rotationSpeed;
    public float mass;

    private int pathSegmentIndex;
    private AStarAgent aStar;

    public override float? timeToHitTheGroud()
    {
        return null;
    }

    public override void endSimulation()
    {
        base.endSimulation();
        aStar.stopExecution();
    }

    public override void endSimulation(float endTime)
    {
        base.endSimulation(endTime);
        aStar.stopExecution();
    }

    protected override void Start()
    {
        base.Start();
        transform.forward = initialVelocity;
        aStar = GetComponent<AStarAgent>();
        aStar.Speed = initialVelocity.magnitude;
        aStar.TurnSpeed = rotationSpeed;
        aStar.Pathfinding(path[1]);
    }

    void performRandomAction()
    {
        if (Random.value < 10 * controller.getScaledTimeDelta()) {
            transform.eulerAngles += new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f));
        }
        if (Random.value < 10 * controller.getScaledTimeDelta()) {
            transform.position += new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f));
        }
    }

    void Update()
    {
        if (hasEnded || !controller.isShowingSimulation) return;
        lineRenderer.SetPosition(lineRenderer.positionCount++, transform.position);
        performRandomAction();
        if (aStar.Status == AStarAgentStatus.Finished && pathSegmentIndex != path.Count - 2)
            aStar.Pathfinding(path[++pathSegmentIndex + 1]);
    }
}
