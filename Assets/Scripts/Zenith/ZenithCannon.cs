using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZenithCannon : Cannon
{
    public float projectileSpeed;
    public float projectileRotationSpeed;
    public float projectileAcceleration;
    public Vector2 initialAngle;

    public bool addsRemoteDetonator;
    public float detonatorShardSpeed;
    public float detonatorDistance;

    public int interceptorsArrived;
    public ZenithInterceptor interceptorPrefab;

    private SceneController controller;
    
    void Start()
    {
        controller = FindFirstObjectByType<SceneController>();
        transform.localRotation = convertRotation(new Vector2(initialAngle.x, 0));
        transform.Find("himarsLauncherWrapper").localRotation = convertRotation(new Vector2(0, initialAngle.y));
        controller.isReadyToRun = true;
    }

    Quaternion convertRotation(Vector2 rotation)
    {
        return Quaternion.Euler(0, -rotation.x, rotation.y);
    }

    public void launchInterceptor(BallisticTarget target)
    {
        ZenithInterceptor interceptor = Instantiate(interceptorPrefab, transform.position, convertRotation(initialAngle));
        interceptor.target = target;
        interceptor.startSpeed = projectileSpeed;
        interceptor.rotationSpeed = projectileRotationSpeed;
        interceptor.acceleration = projectileAcceleration;
        interceptor.hasRemoteDetonator = addsRemoteDetonator;
        interceptor.detonatorShardSpeed = detonatorShardSpeed;
        interceptor.detonatorDistance = detonatorDistance;
        controller.updateStats();
        // GameObject particles = Instantiate(particlePrefab, transform.position + transform.up * -transform.localScale.y / 2, Quaternion.identity);
        // particles.transform.SetParent(gameObject.transform);
        // particles.transform.localRotation = Quaternion.Euler(90, -90, 0);
    }

    void Update()
    {
        if (interceptorsArrived == controller.targets.Count) controller.endSimulation();
        if (!controller.isShowingSimulation || interceptorCount >= controller.targets.Count) return;
        launchInterceptor(controller.targets[interceptorCount++]);
    }
}