using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BallisticTarget : MonoBehaviour
{
    public Vector3 startPosition;
    public Vector3 initialVelocity;
    public Vector3 size;
    public float acceleration;
    public float mass;

    public bool hasEnded {get; private set;}

    private float endTime;
    private SceneController controller;
    private LineRenderer lineRenderer;

    static public List<BallisticTarget> getByTargetPriority(List<BallisticTarget> targets)
    {
        IEnumerable<(float?, BallisticTarget)> withTimes = from x in targets select (x.timeToHitTheGroud(), x);
        withTimes.OrderBy((elem) => {
            if (elem.Item1 == null) return float.MaxValue;
            return elem.Item1;
        });
        return (from x in withTimes select x.Item2).ToList();
    }

    public float? timeToHitTheGroud()
    {
        // B and c coefficient are reffering to the standard form of a quadratic
        float finalAcceleration = initialVelocity.normalized.y * acceleration - SceneController.gravityAcceleration;
        if (finalAcceleration == 0) { // To avoid division by zero
            if (initialVelocity.y < 0) return -startPosition.y / initialVelocity.y;
        } else {
            // Solving a quadratic equation x_{t0} + v_tt + (1/2)at^2 = x_{i0} + v_i(t - t_r)
            float temp = Mathf.Pow(initialVelocity.y, 2) - 2 * finalAcceleration * startPosition.y; 
            if (temp < 0) return null; // Square root of a negative number
            float tempsqrt = Mathf.Sqrt(temp);

            float solution1 = -(initialVelocity.y - tempsqrt) / finalAcceleration;
            if (solution1 >= 0) return solution1;
            float solution2 = -(initialVelocity.y + tempsqrt) / finalAcceleration;
            if (solution2 >= 0) return solution2;
        }
        return null; // No non-negative real solution
    }

    public void endSimulation()
    {
        hasEnded = true;
        endTime = controller.simulationTime;
    }

    public bool isInsideTheTarget(Vector3 point, Vector3 targetPosition)
    {
        // Regular is point inside a cuboid collision code
        bool isInsideOnX = targetPosition.x - size.x / 2 <= point.x && point.x <= targetPosition.x + size.x / 2;
        bool isInsideOnY = targetPosition.y - size.y / 2 <= point.y && point.y <= targetPosition.y + size.y / 2;
        bool isInsideOnZ = targetPosition.z - size.z / 2 <= point.z && point.z <= targetPosition.z + size.z / 2;
        return isInsideOnX && isInsideOnY && isInsideOnZ;
    }

    void Start()
    {
        controller = GameObject.FindFirstObjectByType<SceneController>();
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
    }

    void Update()
    {
        float time = hasEnded ? endTime : controller.simulationTime;
        lineRenderer.SetPosition(lineRenderer.positionCount++, transform.position);
        Vector3 finalAcceleration = acceleration * initialVelocity.normalized + SceneController.gravityAcceleration * Vector3.down;
        transform.position = startPosition + initialVelocity * time +
            finalAcceleration * Mathf.Pow(time, 2) / 2;
    }
}
