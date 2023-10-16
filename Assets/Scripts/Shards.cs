using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shards : MonoBehaviour
{
    public BallisticTarget target;
    public float speed;
    public float launchOffset;

    public bool hasEnded;

    private SceneController controller;

    void Start()
    {
        controller = GameObject.FindFirstObjectByType<SceneController>();
        launchOffset = controller.simulationTime;
    }

    void Update()
    {
        if (hasEnded) return;
        float size = (controller.simulationTime - launchOffset) * speed;
        transform.localScale = size * new Vector3(2, 2, 2);
        if (Vector3.Distance(target.transform.position, transform.position) <= size) {
            target.endSimulation();
            hasEnded = true;
        }
    }
}
