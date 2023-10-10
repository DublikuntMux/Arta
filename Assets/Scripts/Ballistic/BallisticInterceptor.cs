using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallisticInterceptor : MonoBehaviour
{
    public Vector3 velocity;
    public float mass;

    public float launchOffset;
    public float endTime;

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
        float time = controller.simulationTime < endTime ? controller.simulationTime : endTime;
        lineRenderer.SetPosition(lineRenderer.positionCount++, transform.position);
        Vector3 acceleration = SceneController.gravityAcceleration * Vector3.down;
        transform.position = velocity * (time - launchOffset) +
            acceleration * Mathf.Pow(time - launchOffset, 2) / 2;
    }
}
