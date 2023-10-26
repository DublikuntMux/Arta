using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


abstract public class Target : MonoBehaviour
{
    public Vector3 size;

    public bool hasEnded {get; protected set;}

    protected float endTime;
    protected SceneController controller;
    protected LineRenderer lineRenderer;

    static public List<Target> getByTargetPriority(List<Target> targets)
    {
        IEnumerable<(float?, Target)> withTimes = from x in targets select (x.timeToHitTheGroud(), x);
        withTimes.OrderBy((elem) => {
            if (elem.Item1 == null) return float.MaxValue;
            return elem.Item1;
        });
        return (from x in withTimes select x.Item2).ToList();
    }

    public abstract float? timeToHitTheGroud();

    protected virtual void Start()
    {
        controller = FindFirstObjectByType<SceneController>();
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
    }

    public bool isInsideTheTarget(Vector3 point, Vector3 targetPosition)
    {
        // Regular is point inside a cuboid collision code
        bool isInsideOnX = targetPosition.x - size.x / 2 <= point.x && point.x <= targetPosition.x + size.x / 2;
        bool isInsideOnY = targetPosition.y - size.y / 2 <= point.y && point.y <= targetPosition.y + size.y / 2;
        bool isInsideOnZ = targetPosition.z - size.z / 2 <= point.z && point.z <= targetPosition.z + size.z / 2;
        return isInsideOnX && isInsideOnY && isInsideOnZ;
    }

    public virtual void endSimulation()
    {
        hasEnded = true;
        endTime = controller.simulationTime;
    }

    public virtual void endSimulation(float endTime)
    {
        hasEnded = true;
        this.endTime = endTime;
    }
}
