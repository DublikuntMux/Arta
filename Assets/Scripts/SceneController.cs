using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;

public class SceneController : MonoBehaviour
{
    static public List<ISimulationObjectConfiguration> simulationObjects = new List<ISimulationObjectConfiguration>();
    static public float gravityAcceleration;

    public TMP_Text pausedText;
    public TMP_Text statsText;

    public List<BallisticTarget> targets = new List<BallisticTarget>();
    public List<BallisticCannon> cannons = new List<BallisticCannon>();
    public bool isShowingSimulation {get; private set;}
    public float simulationTime {get; private set;}
    public float endTime {get; private set;}

    private float timeScale = 1f;
    private bool isReadyToRun;

    public void startSimulation(List<BallisticInterceptionResult> interceptions)
    {
        endTime = interceptions[interceptions.Count - 1].time;
        isReadyToRun = true;
    }

    public void setTimeScale(float timeScale)
    {
        this.timeScale = timeScale;
    }

    public void updateStats()
    {
        StringBuilder sb = new();
        for (int i = 0; i < cannons.Count; i++)
            sb.AppendFormat("Гармата {0}: {1} снарядів\n", i + 1, cannons[i].interceptorCount);
        statsText.text = sb.ToString();
    }

    void Start()
    {
        simulationTime = 0;
        foreach (ISimulationObjectConfiguration obj in simulationObjects)
            obj.createObject(this);
        targets = BallisticTarget.getByTargetPriority(targets);
        updateStats();
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
        if (simulationTime >= endTime) {
            simulationTime = endTime;
            isShowingSimulation = false;
        }
    }
}
