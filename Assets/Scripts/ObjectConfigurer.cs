using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
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
    public bool addsRemoteDetonator {get; private set;}
    public float detonatorShardSpeed {get; private set;}
    public float detonatorDistance {get; private set;}

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
        addsRemoteDetonator = transform.Find("Cannon.addsRemoteDetonator").GetComponent<Toggle>().isOn;
        detonatorShardSpeed = float.Parse(transform.Find("Cannon.detonatorShardSpeed").GetComponent<TMP_InputField>().text);
        detonatorDistance = float.Parse(transform.Find("Cannon.detonatorDistance").GetComponent<TMP_InputField>().text);
    }

    public void createObject(SceneController controller)
    {
        BallisticCannon cannon = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity);
        cannon.projectileSpeed = projectileSpeed;
        cannon.initialAngle = initialAngle;
        cannon.rotationSpeed = rotationSpeed;
        cannon.addsRemoteDetonator = addsRemoteDetonator;
        cannon.detonatorShardSpeed = detonatorShardSpeed;
        cannon.detonatorDistance = detonatorDistance;
        controller.cannons.Add(cannon);
    }
}

[System.Serializable]
public class InvalidPathException : System.Exception {
    public InvalidPathException(string message)
        : base(message) { }
}

public class AerodynamicTargetConfigurer : ISimulationObjectConfiguration
{
    public List<Vector3> path;
    public Vector3 initialVelocity {get; private set;}
    public Vector3 size {get; private set;}
    public float rotationSpeed {get; private set;}

    static private AerodynamicTarget prefab = Resources.Load<AerodynamicTarget>("Prefabs/AerodynamicTarget");

    public AerodynamicTargetConfigurer(Transform transform) 
    {
        // startPosition = new Vector3(
        //     float.Parse(transform.Find("Target.startPosition.x").GetComponent<TMP_InputField>().text),
        //     float.Parse(transform.Find("Target.startPosition.y").GetComponent<TMP_InputField>().text),
        //     float.Parse(transform.Find("Target.startPosition.z").GetComponent<TMP_InputField>().text)
        // );
        // endPosition = new Vector3(
        //     float.Parse(transform.Find("Target.endPosition.x").GetComponent<TMP_InputField>().text),
        //     float.Parse(transform.Find("Target.endPosition.y").GetComponent<TMP_InputField>().text),
        //     float.Parse(transform.Find("Target.endPosition.z").GetComponent<TMP_InputField>().text)
        // );
        path = parsePath(transform.Find("Target.path").GetComponent<TMP_InputField>().text);
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
        rotationSpeed = float.Parse(transform.Find("Target.rotationSpeed").GetComponent<TMP_InputField>().text);
    }

    private List<Vector3> parsePath(string str)
    {
        int cursor = 0;
        List<Vector3> res = new List<Vector3>();   
        while (cursor < str.Length) {
            int separatorIndex = str.IndexOf(',', cursor);
            if (separatorIndex == -1) throw new InvalidPathException("The point is missing Y and Z coordinates");
            float x;
            try {
                x = float.Parse(str.Substring(cursor, separatorIndex - cursor));
            } catch (System.FormatException) {
                throw new InvalidPathException("Invalid float for X");
            }
            cursor = separatorIndex + 1;

            separatorIndex = str.IndexOf(',', cursor);
            if (separatorIndex == -1) throw new InvalidPathException("The point is missing Z coordinates");
            float y;
            try {
                y = float.Parse(str.Substring(cursor, separatorIndex - cursor));
            } catch (System.FormatException) {
                throw new InvalidPathException("Invalid float for Y");
            }
            cursor = separatorIndex + 1;

            separatorIndex = str.IndexOf(';', cursor);
            if (separatorIndex == -1) throw new InvalidPathException("The point is missing the semicolon");
            float z;
            try {
                z = float.Parse(str.Substring(cursor, separatorIndex - cursor));
            } catch (System.FormatException) {
                throw new InvalidPathException("Invalid float for Z");
            }
            cursor = separatorIndex + 1;

            res.Add(new Vector3(x, y, z));
        }
        if (res.Count < 2)
            throw new InvalidPathException("Path must have a starting and an ending point");
        return res;
    }

    public void createObject(SceneController controller)
    {
        AerodynamicTarget target = GameObject.Instantiate(prefab, path[0], Quaternion.identity);
        target.transform.localScale = size;
        target.path = path;
        target.initialVelocity = initialVelocity;
        target.size = size;
        target.rotationSpeed = rotationSpeed * 360;
        controller.targets.Add(target);
    }
}

public class ZenithCannonConfigurer : ISimulationObjectConfiguration
{
    public float projectileSpeed {get; private set;}
    public float projectileRotationSpeed {get; private set;}
    public float projectileAcceleration {get; private set;}
    public Vector2 initialAngle {get; private set;}
    public bool addsRemoteDetonator {get; private set;}
    public float detonatorShardSpeed {get; private set;}
    public float detonatorDistance {get; private set;}

    static private ZenithCannon prefab = Resources.Load<ZenithCannon>("Prefabs/ZenithCannon");

    public ZenithCannonConfigurer(Transform transform)
    {
        projectileSpeed = float.Parse(transform.Find("Cannon.projectileSpeed").GetComponent<TMP_InputField>().text);
        initialAngle = new Vector2(
            float.Parse(transform.Find("Cannon.initialAngle.x").GetComponent<TMP_InputField>().text),
            float.Parse(transform.Find("Cannon.initialAngle.y").GetComponent<TMP_InputField>().text)
        );
        projectileRotationSpeed = float.Parse(transform.Find("Cannon.projectileRotationSpeed").GetComponent<TMP_InputField>().text);
        projectileAcceleration = float.Parse(transform.Find("Cannon.projectileAcceleration").GetComponent<TMP_InputField>().text);
        addsRemoteDetonator = transform.Find("Cannon.addsRemoteDetonator").GetComponent<Toggle>().isOn;
        detonatorShardSpeed = float.Parse(transform.Find("Cannon.detonatorShardSpeed").GetComponent<TMP_InputField>().text);
        detonatorDistance = float.Parse(transform.Find("Cannon.detonatorDistance").GetComponent<TMP_InputField>().text);
    }

    public void createObject(SceneController controller)
    {
        ZenithCannon cannon = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity);
        cannon.projectileSpeed = projectileSpeed;
        cannon.initialAngle = initialAngle;
        cannon.projectileRotationSpeed = projectileRotationSpeed * 360;
        cannon.projectileAcceleration = projectileAcceleration;
        cannon.addsRemoteDetonator = addsRemoteDetonator;
        cannon.detonatorShardSpeed = detonatorShardSpeed;
        cannon.detonatorDistance = detonatorDistance;
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
            case ObjectType.AerodynamicTarget: return new AerodynamicTargetConfigurer(transform);
            case ObjectType.MissileCannon: return new ZenithCannonConfigurer(transform);
            default: throw new System.NotImplementedException("ObjectConfigurer.ObjectType not implemented");
        }
    }
}
