using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallisticInterceptor : MonoBehaviour
{
    public Vector3 velocity;
    public float mass;

    public float launchOffset;

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
        lineRenderer.SetPosition(lineRenderer.positionCount++, transform.position);
        Vector3 acceleration = SceneController.gravityAcceleration * Vector3.down;
        transform.position = velocity * (controller.simulationTime - launchOffset) +
            acceleration * Mathf.Pow(controller.simulationTime - launchOffset, 2) / 2;
    }
}
