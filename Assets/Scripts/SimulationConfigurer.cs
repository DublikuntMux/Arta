using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SimulationConfigurer : MonoBehaviour
{
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
        configurerPrefabs.Add(ObjectConfigurer.ObjectType.MissileCannon, Resources.Load<GameObject>("Prefabs/ZenithCannonConfigurer"));
        newButtons.Add(ObjectConfigurer.ObjectType.BallisticTarget, GameObject.Find("BallisticTargetColumn").transform.Find("Instances").Find("NewButton").gameObject);
        newButtons.Add(ObjectConfigurer.ObjectType.BallisticCannon, GameObject.Find("BallisticCannonColumn").transform.Find("Instances").Find("NewButton").gameObject);
        newButtons.Add(ObjectConfigurer.ObjectType.MissileCannon, GameObject.Find("MissileCannonColumn").transform.Find("Instances").Find("NewButton").gameObject);
        addBallisiticTarget();
    }

    private void loadGlobalSettings()
    {
        Transform settings = GameObject.Find("GlobalSettings").transform;
        SceneController.gravityAcceleration = float.Parse(settings.Find("SceneController.gravityAcceleration").GetComponent<TMP_InputField>().text);
    }

    public void submit()
    {
        foreach (ObjectConfigurer configurer in configurers) {
            SceneController.simulationObjects.Add(configurer.getConfiguration());
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
        ObjectConfigurer configurer = Instantiate(configurerPrefabs[type], GameObject.FindFirstObjectByType<Canvas>().transform).GetComponent<ObjectConfigurer>();
        configurers.Add(configurer);
        GameObject button = Instantiate(buttonPrefab, newButtons[type].transform.parent);
        button.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = (newButtons[type].transform.parent.childCount - 1).ToString();
        button.GetComponent<Button>().onClick.AddListener(() => activateConfigurer(configurer, button));
        button.transform.SetAsLastSibling();
        newButtons[type].transform.SetAsLastSibling();
        if (newButtons[type].transform.parent.childCount > 5)
            GameObject.Destroy(newButtons[type]);
        GameObject.Find("GlobalSettings").transform.SetAsLastSibling();
        activateConfigurer(configurer, button);
    }

    public void addBallisiticTarget () => addConfigurer(ObjectConfigurer.ObjectType.BallisticTarget);
    public void addBallisiticCannon () => addConfigurer(ObjectConfigurer.ObjectType.BallisticCannon);
    public void addZenithCannon () => addConfigurer(ObjectConfigurer.ObjectType.MissileCannon);
}
