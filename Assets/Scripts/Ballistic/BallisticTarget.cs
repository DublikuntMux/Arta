using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallisticTarget : MonoBehaviour
{
    static public Vector3 startPosition;
    static public Vector3 initialVelocity;
    static public Vector3 size;
    static public float acceleration;
    static public float mass;

    private SceneController controller;
    private LineRenderer lineRenderer;

    static public bool isInsideTheTarget(Vector3 point, Vector3 targetPosition)
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
        transform.position = startPosition;
        transform.localScale = size;
        lineRenderer.positionCount = 0;
    }

    void Update()
    {
        lineRenderer.SetPosition(lineRenderer.positionCount++, transform.position);
        Vector3 finalAcceleration = acceleration * initialVelocity.normalized + SceneController.gravityAcceleration * Vector3.down;
        transform.position = startPosition + initialVelocity * controller.simulationTime +
            finalAcceleration * Mathf.Pow(controller.simulationTime, 2) / 2;
    }
}
