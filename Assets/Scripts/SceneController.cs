using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SceneController : MonoBehaviour
{
    static public float gravityAcceleration;

    public TMP_Text pausedText;

    public bool isShowingSimulation {get; private set;}
    public float simulationTime {get; private set;}
    public float collisionTime {get; private set;}

    private float timeScale = 1f;
    private bool isReadyToRun;

    public void startSimulation(float collisionTime)
    {
        this.collisionTime = collisionTime;
        isReadyToRun = true;
    }

    public void setTimeScale(float timeScale)
    {
        this.timeScale = timeScale;
    }

    void Start()
    {
        simulationTime = 0;
    }

    void Update()
    {
        if (!isShowingSimulation) {
            if (isReadyToRun && (Input.GetKeyDown(KeyCode.Space))) {
                isShowingSimulation = true;
                pausedText.text = "";
            }
            else return;
        } else if (Input.GetKeyDown(KeyCode.Space)) {
            isShowingSimulation = false;
            pausedText.text = "Paused";
            return;
        }
        simulationTime += Time.deltaTime * timeScale;
        if (simulationTime >= collisionTime) {
            simulationTime = collisionTime;
            isShowingSimulation = false;
        }
    }
}
