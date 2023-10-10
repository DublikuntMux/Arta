using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public interface ISimulationObjectConfiguration
{
    abstract public void createObject(SceneController controller);
}

public class BallisticTargetConfigurer : ISimulationObjectConfiguration
{
    public Vector3 startPosition {get; private set;}
    public Vector3 initialVelocity {get; private set;}
    public Vector3 size {get; private set;}
    public float acceleration {get; private set;}

    static private BallisticTarget prefab = Resources.Load<BallisticTarget>("Prefabs/BallisticTarget");

    public BallisticTargetConfigurer(Transform transform) 
    {
        startPosition = new Vector3(
            float.Parse(transform.Find("Target.startPosition.x").GetComponent<TMP_InputField>().text),
            float.Parse(transform.Find("Target.startPosition.y").GetComponent<TMP_InputField>().text),
            float.Parse(transform.Find("Target.startPosition.z").GetComponent<TMP_InputField>().text)
        );
        initialVelocity = new Vector3(
            float.Parse(transform.Find("Target.initialVelocity.x").GetComponent<TMP_InputField>().text),
            float.Parse(transform.Find("Target.initialVelocity.y").GetComponent<TMP_InputField>().text),
            float.Parse(transform.Find("Target.initialVelocity.z").GetComponent<TMP_InputField>().text)
        );
        size = new Vector3(
            float.Parse(transform.Find("Target.size.x").GetComponent<TMP_InputField>().text),
            float.Parse(transform.Find("Target.size.y").GetComponent<TMP_InputField>().text),
            float.Parse(transform.Find("Target.size.z").GetComponent<TMP_InputField>().text)
        );
        acceleration = float.Parse(transform.Find("Target.acceleration").GetComponent<TMP_InputField>().text);
    }

    public void createObject(SceneController controller)
    {
        BallisticTarget target = GameObject.Instantiate(prefab, startPosition, Quaternion.identity);
        target.transform.localScale = size;
        target.startPosition = startPosition;
        target.initialVelocity = initialVelocity;
        target.size = size;
        target.acceleration = acceleration;
        controller.targets.Add(target);
    }
}

public class BallisticCannonConfigurer : ISimulationObjectConfiguration
{
    public float projectileSpeed {get; private set;}
    public Vector2 initialAngle {get; private set;}
    public Vector2 rotationSpeed {get; private set;}

    static private BallisticCannon prefab = Resources.Load<BallisticCannon>("Prefabs/BallisticCannon");

    public BallisticCannonConfigurer(Transform transform)
    {
        projectileSpeed = float.Parse(transform.Find("Cannon.projectileSpeed").GetComponent<TMP_InputField>().text);
        initialAngle = new Vector2(
            float.Parse(transform.Find("Cannon.initialAngle.x").GetComponent<TMP_InputField>().text),
            float.Parse(transform.Find("Cannon.initialAngle.y").GetComponent<TMP_InputField>().text)
        );
        rotationSpeed = new Vector2(
            float.Parse(transform.Find("Cannon.rotationSpeed.x").GetComponent<TMP_InputField>().text),
            float.Parse(transform.Find("Cannon.rotationSpeed.y").GetComponent<TMP_InputField>().text)
        );
    }

    public void createObject(SceneController controller)
    {
        BallisticCannon cannon = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity);
        cannon.projectileSpeed = projectileSpeed;
        cannon.initialAngle = initialAngle;
        cannon.rotationSpeed = rotationSpeed;
        controller.cannons.Add(cannon);
    }
}

public class ObjectConfigurer : MonoBehaviour
{
    public enum ObjectType {
        BallisticTarget,
        BallisticCannon,
        AerodynamicTarget,
        MissileCannon
    };
    public ObjectType type;

    public ISimulationObjectConfiguration getConfiguration()
    {
        switch (type) {
            case ObjectType.BallisticTarget: return new BallisticTargetConfigurer(transform);
            case ObjectType.BallisticCannon: return new BallisticCannonConfigurer(transform);
            default: throw new System.NotImplementedException("ObjectConfigurer.ObjectType not implemented");
        }
    }
}
