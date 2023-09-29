using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SimulationConfigurer : MonoBehaviour
{
    public void onClick()
    {
        BallisticTarget.startPosition.x = float.Parse(transform.Find("Target.startPosition.x").GetComponent<TMP_InputField>().text);
        BallisticTarget.startPosition.y = float.Parse(transform.Find("Target.startPosition.y").GetComponent<TMP_InputField>().text);
        BallisticTarget.startPosition.z = float.Parse(transform.Find("Target.startPosition.z").GetComponent<TMP_InputField>().text);
        BallisticTarget.initialVelocity.x = float.Parse(transform.Find("Target.initialVelocity.x").GetComponent<TMP_InputField>().text);
        BallisticTarget.initialVelocity.y = float.Parse(transform.Find("Target.initialVelocity.y").GetComponent<TMP_InputField>().text);
        BallisticTarget.initialVelocity.z = float.Parse(transform.Find("Target.initialVelocity.z").GetComponent<TMP_InputField>().text);
        BallisticTarget.size.x = float.Parse(transform.Find("Target.size.x").GetComponent<TMP_InputField>().text);
        BallisticTarget.size.y = float.Parse(transform.Find("Target.size.y").GetComponent<TMP_InputField>().text);
        BallisticTarget.size.z = float.Parse(transform.Find("Target.size.z").GetComponent<TMP_InputField>().text);
        BallisticCannon.projectileSpeed = float.Parse(transform.Find("Cannon.projectileSpeed").GetComponent<TMP_InputField>().text);
        BallisticCannon.initialAngle.x = float.Parse(transform.Find("Cannon.initialAngle.x").GetComponent<TMP_InputField>().text);
        BallisticCannon.initialAngle.y = float.Parse(transform.Find("Cannon.initialAngle.y").GetComponent<TMP_InputField>().text);
        BallisticCannon.rotationSpeed.x = float.Parse(transform.Find("Cannon.rotationSpeed.x").GetComponent<TMP_InputField>().text);
        BallisticCannon.rotationSpeed.y = float.Parse(transform.Find("Cannon.rotationSpeed.y").GetComponent<TMP_InputField>().text);
        BallisticTarget.acceleration = float.Parse(transform.Find("Target.acceleration").GetComponent<TMP_InputField>().text);
        SceneController.gravityAcceleration = float.Parse(transform.Find("SceneController.gravityAcceleration").GetComponent<TMP_InputField>().text);       
        SceneManager.LoadScene("Scenes/Game");
    }
}
