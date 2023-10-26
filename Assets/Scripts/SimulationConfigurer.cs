using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SimulationConfigurer : MonoBehaviour
{
    static private int presetId = 0; 

    public GameObject buttonPrefab;
    private Dictionary<ObjectConfigurer.ObjectType, GameObject> configurerPrefabs = new Dictionary<ObjectConfigurer.ObjectType, GameObject>();
    private Dictionary<ObjectConfigurer.ObjectType, GameObject> newButtons = new Dictionary<ObjectConfigurer.ObjectType, GameObject>();
    private List<ObjectConfigurer> configurers = new List<ObjectConfigurer>();
    private ObjectConfigurer activeConfigurer;
    private GameObject activeConfigurerButton;

    public void Start()
    {
        configurerPrefabs.Add(ObjectConfigurer.ObjectType.BallisticTarget, Resources.Load<GameObject>("Prefabs/BallisticTargetConfigurer"));
        configurerPrefabs.Add(ObjectConfigurer.ObjectType.BallisticCannon, Resources.Load<GameObject>("Prefabs/BallisticCannonConfigurer"));
        configurerPrefabs.Add(ObjectConfigurer.ObjectType.AerodynamicTarget, Resources.Load<GameObject>("Prefabs/AerodynamicTargetConfigurer"));
        configurerPrefabs.Add(ObjectConfigurer.ObjectType.MissileCannon, Resources.Load<GameObject>("Prefabs/ZenithCannonConfigurer"));
        newButtons.Add(ObjectConfigurer.ObjectType.BallisticTarget, GameObject.Find("BallisticTargetColumn").transform.Find("Instances").Find("NewButton").gameObject);
        newButtons.Add(ObjectConfigurer.ObjectType.BallisticCannon, GameObject.Find("BallisticCannonColumn").transform.Find("Instances").Find("NewButton").gameObject);
        newButtons.Add(ObjectConfigurer.ObjectType.AerodynamicTarget, GameObject.Find("AerodynamicTargetColumn").transform.Find("Instances").Find("NewButton").gameObject);
        newButtons.Add(ObjectConfigurer.ObjectType.MissileCannon, GameObject.Find("MissileCannonColumn").transform.Find("Instances").Find("NewButton").gameObject);
        loadExample(presetId);
    }

    private void loadExample(int preset)
    {
        switch (preset) {
            case 0: addBallisiticTarget(); break;
            case 1: {
                addBallisiticTarget();
                GameObject.Find("Target.startPosition.x").GetComponent<TMP_InputField>().text = "100";
                GameObject.Find("Target.startPosition.y").GetComponent<TMP_InputField>().text = "50";
                GameObject.Find("Target.startPosition.z").GetComponent<TMP_InputField>().text = "30";
                GameObject.Find("Target.initialVelocity.x").GetComponent<TMP_InputField>().text = "-40";
                GameObject.Find("Target.initialVelocity.y").GetComponent<TMP_InputField>().text = "9";
                GameObject.Find("Target.initialVelocity.z").GetComponent<TMP_InputField>().text = "1";
                GameObject.Find("Target.size.x").GetComponent<TMP_InputField>().text = "5";
                GameObject.Find("Target.size.y").GetComponent<TMP_InputField>().text = "2";
                GameObject.Find("Target.size.z").GetComponent<TMP_InputField>().text = "2";
                GameObject.Find("Target.acceleration").GetComponent<TMP_InputField>().text = "3";
                addBallisticCannon();
                GameObject.Find("Cannon.projectileSpeed").GetComponent<TMP_InputField>().text = "90";
                GameObject.Find("Cannon.rotationSpeed.x").GetComponent<TMP_InputField>().text = "2";
                GameObject.Find("Cannon.rotationSpeed.y").GetComponent<TMP_InputField>().text = "1";
                GameObject.Find("Cannon.initialAngle.x").GetComponent<TMP_InputField>().text = "0";
                GameObject.Find("Cannon.initialAngle.y").GetComponent<TMP_InputField>().text = "20";
                GameObject.Find("SceneController.gravityAcceleration").GetComponent<TMP_InputField>().text = "5";
                break;
            }
            case 2: {
                addBallisiticTarget();
                GameObject.Find("Target.startPosition.x").GetComponent<TMP_InputField>().text = "-90";
                GameObject.Find("Target.startPosition.y").GetComponent<TMP_InputField>().text = "15";
                GameObject.Find("Target.startPosition.z").GetComponent<TMP_InputField>().text = "-2";
                GameObject.Find("Target.initialVelocity.x").GetComponent<TMP_InputField>().text = "0";
                GameObject.Find("Target.initialVelocity.y").GetComponent<TMP_InputField>().text = "1";
                GameObject.Find("Target.initialVelocity.z").GetComponent<TMP_InputField>().text = "0.1";
                GameObject.Find("Target.size.x").GetComponent<TMP_InputField>().text = "2.5";
                GameObject.Find("Target.size.y").GetComponent<TMP_InputField>().text = "0.5";
                GameObject.Find("Target.size.z").GetComponent<TMP_InputField>().text = "0.5";
                GameObject.Find("Target.acceleration").GetComponent<TMP_InputField>().text = "3";
                addZenithCannon();
                GameObject.Find("Cannon.projectileSpeed").GetComponent<TMP_InputField>().text = "20";
                GameObject.Find("Cannon.initialAngle.x").GetComponent<TMP_InputField>().text = "0";
                GameObject.Find("Cannon.initialAngle.y").GetComponent<TMP_InputField>().text = "0";
                GameObject.Find("Cannon.projectileRotationSpeed").GetComponent<TMP_InputField>().text = "2";
                GameObject.Find("Cannon.projectileAcceleration").GetComponent<TMP_InputField>().text = "5";
                GameObject.Find("SceneController.gravityAcceleration").GetComponent<TMP_InputField>().text = "3";
                break;
            }
            case 3: {
                addAerodynamicTarget();
                // GameObject.Find("Target.startPosition.x").GetComponent<TMP_InputField>().text = "-35";
                // GameObject.Find("Target.startPosition.y").GetComponent<TMP_InputField>().text = "25";
                // GameObject.Find("Target.startPosition.z").GetComponent<TMP_InputField>().text = "100";
                // GameObject.Find("Target.endPosition.x").GetComponent<TMP_InputField>().text = "10";
                // GameObject.Find("Target.endPosition.y").GetComponent<TMP_InputField>().text = "17";
                // GameObject.Find("Target.endPosition.z").GetComponent<TMP_InputField>().text = "70";
                GameObject.Find("Target.path").GetComponent<TMP_InputField>().text = "-35,25,100;-40,16,110;10,17,70;";
                GameObject.Find("Target.initialVelocity.x").GetComponent<TMP_InputField>().text = "15";
                GameObject.Find("Target.initialVelocity.y").GetComponent<TMP_InputField>().text = "-5";
                GameObject.Find("Target.initialVelocity.z").GetComponent<TMP_InputField>().text = "5";
                GameObject.Find("Target.size.z").GetComponent<TMP_InputField>().text = "3";
                GameObject.Find("Target.size.y").GetComponent<TMP_InputField>().text = "0.5";
                GameObject.Find("Target.size.x").GetComponent<TMP_InputField>().text = "0.5";
                GameObject.Find("Target.rotationSpeed").GetComponent<TMP_InputField>().text = "2";
                addZenithCannon();
                GameObject.Find("Cannon.projectileSpeed").GetComponent<TMP_InputField>().text = "15";
                GameObject.Find("Cannon.initialAngle.x").GetComponent<TMP_InputField>().text = "0";
                GameObject.Find("Cannon.initialAngle.y").GetComponent<TMP_InputField>().text = "30";
                GameObject.Find("Cannon.projectileRotationSpeed").GetComponent<TMP_InputField>().text = "2.5";
                GameObject.Find("Cannon.projectileAcceleration").GetComponent<TMP_InputField>().text = "3";
                GameObject.Find("Cannon.addsRemoteDetonator").GetComponent<Toggle>().isOn = true;
                GameObject.Find("SceneController.gravityAcceleration").GetComponent<TMP_InputField>().text = "3";
                break;
            }
            case 4: {
                addBallisiticTarget();
                addBallisiticTarget();
                ObjectConfigurer[] targets = FindObjectsOfType<ObjectConfigurer>(true);
                Transform currTarget = targets[0].transform;
                currTarget.Find("Target.startPosition.x").GetComponent<TMP_InputField>().text = "100";
                currTarget.Find("Target.startPosition.y").GetComponent<TMP_InputField>().text = "50";
                currTarget.Find("Target.startPosition.z").GetComponent<TMP_InputField>().text = "30";
                currTarget.Find("Target.initialVelocity.x").GetComponent<TMP_InputField>().text = "-40";
                currTarget.Find("Target.initialVelocity.y").GetComponent<TMP_InputField>().text = "9";
                currTarget.Find("Target.initialVelocity.z").GetComponent<TMP_InputField>().text = "1";
                currTarget.Find("Target.size.x").GetComponent<TMP_InputField>().text = "5";
                currTarget.Find("Target.size.y").GetComponent<TMP_InputField>().text = "2";
                currTarget.Find("Target.size.z").GetComponent<TMP_InputField>().text = "2";
                currTarget.Find("Target.acceleration").GetComponent<TMP_InputField>().text = "3";
                currTarget = targets[1].transform;
                currTarget.Find("Target.startPosition.x").GetComponent<TMP_InputField>().text = "-45";
                currTarget.Find("Target.startPosition.y").GetComponent<TMP_InputField>().text = "30";
                currTarget.Find("Target.startPosition.z").GetComponent<TMP_InputField>().text = "-100";
                currTarget.Find("Target.initialVelocity.x").GetComponent<TMP_InputField>().text = "10";
                currTarget.Find("Target.initialVelocity.y").GetComponent<TMP_InputField>().text = "15";
                currTarget.Find("Target.initialVelocity.z").GetComponent<TMP_InputField>().text = "3";
                currTarget.Find("Target.size.x").GetComponent<TMP_InputField>().text = "4";
                currTarget.Find("Target.size.y").GetComponent<TMP_InputField>().text = "2";
                currTarget.Find("Target.size.z").GetComponent<TMP_InputField>().text = "2";
                currTarget.Find("Target.acceleration").GetComponent<TMP_InputField>().text = "5";
                addBallisticCannon();
                GameObject.Find("Cannon.projectileSpeed").GetComponent<TMP_InputField>().text = "60";
                GameObject.Find("Cannon.rotationSpeed.x").GetComponent<TMP_InputField>().text = "2.5";
                GameObject.Find("Cannon.rotationSpeed.y").GetComponent<TMP_InputField>().text = "2";
                GameObject.Find("Cannon.initialAngle.x").GetComponent<TMP_InputField>().text = "0";
                GameObject.Find("Cannon.initialAngle.y").GetComponent<TMP_InputField>().text = "20";
                GameObject.Find("SceneController.gravityAcceleration").GetComponent<TMP_InputField>().text = "9.8";
                break;
            }
        }
    }

    private void loadGlobalSettings()
    {
        Transform settings = GameObject.Find("GlobalSettings").transform;
        SceneController.gravityAcceleration = float.Parse(settings.Find("SceneController.gravityAcceleration").GetComponent<TMP_InputField>().text);
    }

    public void submit()
    {
        try {
            foreach (ObjectConfigurer configurer in configurers)
                SceneController.simulationObjects.Add(configurer.getConfiguration());
        } catch (InvalidPathException e) {
            Debug.Log(e.Message);
        }
        loadGlobalSettings();
        SceneManager.LoadScene("Scenes/Game");
    }

    public void activateConfigurer(ObjectConfigurer configurer, GameObject configurerButton)
    {
        if (activeConfigurer != null) activeConfigurer.gameObject.SetActive(false);
        if (activeConfigurerButton != null)
            activeConfigurerButton.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().fontStyle ^= FontStyles.Bold; 
        activeConfigurer = configurer;
        activeConfigurerButton = configurerButton;
        configurer.gameObject.SetActive(true);
        configurerButton.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().fontStyle |= FontStyles.Bold;
    }

    public void addConfigurer(ObjectConfigurer.ObjectType type)
    {
        ObjectConfigurer configurer = Instantiate(configurerPrefabs[type], FindFirstObjectByType<Canvas>().transform).GetComponent<ObjectConfigurer>();
        configurers.Add(configurer);
        GameObject button = Instantiate(buttonPrefab, newButtons[type].transform.parent);
        button.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = (newButtons[type].transform.parent.childCount - 1).ToString();
        button.GetComponent<Button>().onClick.AddListener(() => activateConfigurer(configurer, button));
        button.transform.SetAsLastSibling();
        newButtons[type].transform.SetAsLastSibling();
        if (newButtons[type].transform.parent.childCount > 5)
            GameObject.Destroy(newButtons[type]);
        switch (type) {
            case ObjectConfigurer.ObjectType.BallisticCannon:
                GameObject.Destroy(newButtons[ObjectConfigurer.ObjectType.AerodynamicTarget]);
                goto case ObjectConfigurer.ObjectType.MissileCannon; 
            case ObjectConfigurer.ObjectType.MissileCannon:
                GameObject.Destroy(newButtons[ObjectConfigurer.ObjectType.BallisticCannon]);
                GameObject.Destroy(newButtons[ObjectConfigurer.ObjectType.MissileCannon]);
                break;
            case ObjectConfigurer.ObjectType.AerodynamicTarget:
                GameObject.Destroy(newButtons[ObjectConfigurer.ObjectType.BallisticCannon]);
                break;
        }
        GameObject.Find("GlobalSettings").transform.SetAsLastSibling();
        activateConfigurer(configurer, button);
    }

    public void selectExample(int exampleNumber)
    {
        presetId = exampleNumber;
        SceneManager.LoadScene(0);
    }

    public void addBallisiticTarget () => addConfigurer(ObjectConfigurer.ObjectType.BallisticTarget);
    public void addBallisticCannon () => addConfigurer(ObjectConfigurer.ObjectType.BallisticCannon);
    public void addAerodynamicTarget () => addConfigurer(ObjectConfigurer.ObjectType.AerodynamicTarget);
    public void addZenithCannon () => addConfigurer(ObjectConfigurer.ObjectType.MissileCannon);
    public void selectExample1() => selectExample(1);
    public void selectExample2() => selectExample(2);
    public void selectExample3() => selectExample(3);
    public void selectExample4() => selectExample(4);
}
