using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallisticTarget : Target
{
    public Vector3 startPosition;
    public Vector3 initialVelocity;
    public float acceleration;
    public float mass;

    public override float? timeToHitTheGroud()
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

    void Update()
    {
        float time = hasEnded ? endTime : controller.simulationTime;
        lineRenderer.SetPosition(lineRenderer.positionCount++, transform.position);
        Vector3 finalAcceleration = acceleration * initialVelocity.normalized + SceneController.gravityAcceleration * Vector3.down;
        transform.position = startPosition + initialVelocity * time +
            finalAcceleration * Mathf.Pow(time, 2) / 2;
    }
}
