using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    static public float gravityAcceleration = 3f;

    public bool isShowingSimulation {get; private set;}
    public float simulationTime {get; private set;}
    public float collisionTime {get; private set;}

    private bool isReadyToRun;

    public void startSimulation(float collisionTime)
    {
        this.collisionTime = collisionTime;
        isReadyToRun = true;
    }

    void Start()
    {
        simulationTime = 0;
    }

    void Update()
    {
        if (!isShowingSimulation) {
            if (isReadyToRun && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))) isShowingSimulation = true;
            else return;
        }
        simulationTime += Time.deltaTime;
        if (simulationTime >= collisionTime) {
            simulationTime = collisionTime;
            isShowingSimulation = false;
        }
    }
}
