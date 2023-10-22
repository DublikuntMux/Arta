using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AerodynamicTarget : Target
{
    public Vector3 startPosition;
    public Vector3 endPosition;
    public Vector3 initialVelocity;
    public float rotationSpeed;
    public float mass;

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
        transform.position = startPosition;
        transform.forward = initialVelocity;
        aStar = GetComponent<AStarAgent>();
        aStar.Speed = initialVelocity.magnitude;
        aStar.TurnSpeed = rotationSpeed;
        aStar.Pathfinding(endPosition);
    }

    void Update()
    {
        if (hasEnded) return;
        lineRenderer.SetPosition(lineRenderer.positionCount++, transform.position);
    }
}
