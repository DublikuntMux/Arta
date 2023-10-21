using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;


public class SceneController : MonoBehaviour
{
    static public List<ISimulationObjectConfiguration> simulationObjects = new List<ISimulationObjectConfiguration>();
    static public float gravityAcceleration;

    public GameObject obstaclesWrapper;
    public TMP_Text pausedText;
    public TMP_Text statsText;

    public List<BallisticTarget> targets = new List<BallisticTarget>();
    public List<Cannon> cannons = new List<Cannon>();
    public bool isShowingSimulation {get; private set;}
    public float simulationTime {get; private set;}
    public float endTime {get; private set;}
    public bool isReadyToRun;

    private float timeScale = 1f;

    public void startSimulation(List<BallisticInterceptionResult> interceptions)
    {
        endTime = interceptions[interceptions.Count - 1].time;
        isReadyToRun = true;
    }

    public void endSimulation()
    {
        isReadyToRun = false;
        isShowingSimulation = false;
    }

    public float getScaledTimeDelta()
    {
        if (!isShowingSimulation) return 0f;
        return Time.deltaTime * timeScale;
    }

    public void setTimeScale(float timeScale)
    {
        this.timeScale = timeScale;
    }

    public void updateStats()
    {
        StringBuilder sb = new();
        for (int i = 0; i < cannons.Count; i++) {
            string projectiles;
            int lastDigit = cannons[i].interceptorCount % 10;
            if (lastDigit == 1) projectiles = "снаряд";
            else if (lastDigit > 1 && lastDigit < 5) projectiles = "снаряди";
            else projectiles = "снарядів";
            sb.AppendFormat("Гармата {0}: {1} {2}\n", i + 1, cannons[i].interceptorCount, projectiles);
        }
        statsText.text = sb.ToString();
    }

    void Start()
    {
        simulationTime = 0;
        foreach (ISimulationObjectConfiguration obj in simulationObjects) {
            obj.createObject(this);
            if (obj is ZenithCannonConfigurer)
                obstaclesWrapper.SetActive(true);
        }
        targets = BallisticTarget.getByTargetPriority(targets);
        PathfindingWorldManager.Instance.InitializeGrid();
        updateStats();
    }

    void Update()
    {
        if (!isShowingSimulation) {
            if (isReadyToRun && (Input.GetKeyDown(KeyCode.Space))) {
                isShowingSimulation = true;
                pausedText.text = "";
            } else return;
        } else if (Input.GetKeyDown(KeyCode.Space)) {
            isShowingSimulation = false;
            pausedText.text = "Paused";
            return;
        }
        simulationTime += getScaledTimeDelta();
    }
}
