using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BallisticInterceptionResult {
    public bool isInercepted = false;
    public BallisticTarget target;
    public float delta;
    public Vector2 rotation;
    public float rotationTime;
    public Vector3 velocity;
    public float time;

    public BallisticInterceptionResult(BallisticTarget target, float delta, float rotationX, float rotationY)
    {
        this.target = target;
        this.delta = delta;
        rotation = new Vector2(rotationX, rotationY);
    }
    public BallisticInterceptionResult(BallisticTarget target, float delta, float rotationX, float rotationY, float rotationTime, Vector3 velocity, float time)
    {
        this.isInercepted = true;
        this.target = target;
        this.delta = delta;
        rotation = new Vector2(rotationX, rotationY);
        this.rotationTime = rotationTime;
        this.velocity = velocity;
        this.time = time;
    }
}


public class BallisticCannon : Cannon
{
    public float projectileSpeed;
    public float projectileMass;
    public Vector2 initialAngle;
    public Vector2 rotationSpeed;

    public bool addsRemoteDetonator;
    public float detonatorShardSpeed;
    public float detonatorDistance;

    public BallisticInterceptor interceptorPrefab;
    // public GameObject particlePrefab;
    public float stepFactor;
    public int iterationsPerFrame;
    public float crushCheckTimeDelta;

    private SceneController controller;
    private BallisticInterceptionResult calculationResult;
    private List<BallisticInterceptionResult> interceptions = new List<BallisticInterceptionResult>();
    private Terrain terrain;
    
    void Start()
    {
        controller = FindFirstObjectByType<SceneController>();
        terrain = FindFirstObjectByType<Terrain>();
        calculationResult = new BallisticInterceptionResult(null, 1, 0, 0);
        Vector3 angles = transform.rotation.eulerAngles;
        transform.localRotation = convertRotation(new Vector2(initialAngle.x, 0));
        transform.Find("himarsLauncherWrapper").localRotation = convertRotation(new Vector2(0, initialAngle.y));
    }

    Vector3 getCannonDirection(float angleX, float angleY)
    {
        float alpha = angleX * Mathf.Deg2Rad;
        float beta = angleY * Mathf.Deg2Rad;
        float betaCos = Mathf.Cos(beta); // For perfomance
        return new Vector3(Mathf.Cos(alpha) * betaCos, Mathf.Sin(beta), Mathf.Sin(alpha) * betaCos);
    }

    Quaternion convertRotation(Vector2 rotation)
    {
        return Quaternion.Euler(0, -rotation.x, rotation.y);
    }

    Vector3 getInterceptorPositionAt(float time, Vector3 velocity)
    {
        float gravitationalDelta = SceneController.gravityAcceleration * Mathf.Pow(time, 2) / 2;
        return transform.position + velocity * time + gravitationalDelta * Vector3.down;
    }

    float? getTimeToCollideOnX(Vector3 velocity, float rotationTime, BallisticTarget target)
    {
        // B and c coefficient are reffering to the standard form of a quadratic
        float b_coef = target.initialVelocity.x - velocity.x;
        float c_coef = target.startPosition.x - transform.position.x + velocity.x * rotationTime;
        float acceleration = target.initialVelocity.normalized.x * target.acceleration;
        // Debug.Log(new Vector3(b_coef, c_coef, acceleration));
        if (acceleration == 0) { // To avoid division by zero
            if (b_coef < 0) return -c_coef / b_coef;
        } else {
            // Solving a quadratic equation x_{t0} + v_tt + (1/2)at^2 = x_{i0} + v_i(t - t_r)
            float temp = Mathf.Pow(b_coef, 2) - 2 * acceleration * c_coef;
            if (temp < 0) return null; // Square root of a negative number
            float tempsqrt = Mathf.Sqrt(temp);

            float solution1 = -(b_coef - tempsqrt) / acceleration;
            if (solution1 >= 0) return solution1;
            float solution2 = -(b_coef + tempsqrt) / acceleration;
            if (solution2 >= 0) return solution2;
            // Debug.Log(new Vector2(solution1, solution2));
        }
        return null; // No non-negative real solution
    }

    bool willCrushBeforeInterception(float time, float rotationTime, Vector3 velocity, BallisticTarget target)
    {
        for (float currTime = 0; currTime <= time; currTime += crushCheckTimeDelta) {
            if (currTime > rotationTime) {
                Vector3 interceptorPosition = getInterceptorPositionAt(currTime - rotationTime, velocity);
                if (interceptorPosition.y <= terrain.SampleHeight(interceptorPosition) + terrain.gameObject.transform.position.y)
                    return true;
            }
            Vector3 targetAcceleration = target.initialVelocity.normalized * target.acceleration + Vector3.down * SceneController.gravityAcceleration;
            Vector3 targetPosition = target.startPosition + target.initialVelocity * currTime +
                targetAcceleration * Mathf.Pow(currTime, 2) / 2;
            if (targetPosition.y <= terrain.SampleHeight(targetPosition) + terrain.gameObject.transform.position.y)
                return true;
        }
        return false;
    }

    bool isInterceptingAt(Vector3 point, float time, BallisticTarget target)
    {
        float acceleration = target.initialVelocity.normalized.y * target.acceleration - SceneController.gravityAcceleration;
        float targetY = target.startPosition.y + target.initialVelocity.y * time + acceleration * Mathf.Pow(time, 2) / 2;
        acceleration = target.initialVelocity.normalized.z * target.acceleration;
        float targetZ = target.startPosition.z + target.initialVelocity.z * time + acceleration * Mathf.Pow(time, 2) / 2;
        return target.isInsideTheTarget(new Vector3(point.x, targetY, targetZ), point);
    }

    BallisticInterceptionResult findInterceptionPoint(float delta, Vector2 startRotation, Vector2 initialRotation, float timeOffset, BallisticTarget target)
    {
        float angleX = startRotation.x;
        float angleY = startRotation.y;
        int iterations = 0;
        while (iterations < iterationsPerFrame) {
            if (angleX > 360f) {
                angleX = angleY = 0f;
                delta *= stepFactor;
            }
            while (angleY <= 90f && iterations++ < iterationsPerFrame) {
                float rotationTime = Mathf.Max(
                    Mathf.Abs(angleX - initialRotation.x) / 360f / rotationSpeed.x,
                    Mathf.Abs(angleY - initialRotation.y) / 360f / rotationSpeed.y
                ) + timeOffset;
                Vector3 velocity = getCannonDirection(angleX, angleY) * projectileSpeed;
                // Debug.Log(new Vector2(angleX, angleY));
                float? time = getTimeToCollideOnX(velocity, rotationTime, target);
                // Debug.Log(time);
                if (time == null) {
                    angleY += delta;
                    continue;
                }
                float gravitationalDelta = SceneController.gravityAcceleration * Mathf.Pow((float)time - rotationTime, 2) / 2;
                Vector3 interceptorPosition = getInterceptorPositionAt((float)time - rotationTime, velocity);
                if (isInterceptingAt(interceptorPosition, (float)time, target))
                    if (!willCrushBeforeInterception((float)time, rotationTime, velocity, target))
                        return new BallisticInterceptionResult(target, delta, angleX, angleY, rotationTime, velocity, (float)time);
                angleY += delta;
            }
            angleY = 0f;
            angleX += delta;
        }
        return new BallisticInterceptionResult(target, delta, angleX, angleY);
    }

    public void launchInterceptor(BallisticInterceptionResult interception)
    {
        BallisticInterceptor interceptor = Instantiate(interceptorPrefab, transform.position, Quaternion.identity);
        interceptor.target = interception.target;
        interceptor.velocity = interception.velocity;
        interceptor.mass = projectileMass;
        interceptor.launchOffset = interception.rotationTime;
        interceptor.endTime = interception.time;
        interceptor.hasRemoteDetonator = addsRemoteDetonator;
        interceptor.detonatorShardSpeed = detonatorShardSpeed;
        interceptor.detonatorDistance = detonatorDistance;
        controller.updateStats();
        // GameObject particles = Instantiate(particlePrefab, transform.position + transform.up * -transform.localScale.y / 2, Quaternion.identity);
        // particles.transform.SetParent(gameObject.transform);
        // particles.transform.localRotation = Quaternion.Euler(90, -90, 0);
    }

    void handleCalculations(BallisticTarget target)
    {
        if (calculationResult.isInercepted) return;
        float timeOffset;
        Vector2 initialRotation;
        if (interceptions.Count == 0) {
            timeOffset = 0;
            initialRotation = initialAngle;
        } else {
            timeOffset = interceptions[interceptions.Count - 1].rotationTime;
            initialRotation = interceptions[interceptions.Count - 1].rotation;
        }
        calculationResult = findInterceptionPoint(calculationResult.delta, calculationResult.rotation, initialRotation, timeOffset, target);
        if (calculationResult.isInercepted) {
            interceptions.Add(calculationResult);
            Debug.Log(interceptions.Count);
            Debug.Log(initialRotation);
            Debug.Log(calculationResult.rotation);
            calculationResult = new BallisticInterceptionResult(null, 1, 0, 0);
        }
    }

    void displayRotation()
    {
        float timeOffset;
        Vector2 initialRotation;
        if (interceptorCount == 0) {
            timeOffset = 0;
            initialRotation = initialAngle;
        } else {
            timeOffset = interceptions[interceptorCount - 1].rotationTime;
            initialRotation = interceptions[interceptorCount - 1].rotation;
        }
        transform.localRotation = Quaternion.Lerp(
            convertRotation(new Vector2(initialRotation.x, 0)),
            convertRotation(new Vector2(interceptions[interceptorCount].rotation.x, 0)),
            (controller.simulationTime - timeOffset) / (interceptions[interceptorCount].rotationTime - timeOffset)
        );
        transform.Find("himarsLauncherWrapper").localRotation = Quaternion.Lerp(
            convertRotation(new Vector2(0, initialRotation.y)),
            convertRotation(new Vector2(0, interceptions[interceptorCount].rotation.y)),
            (controller.simulationTime - timeOffset) / (interceptions[interceptorCount].rotationTime - timeOffset)
        );
    }

    void Update()
    {
        if (interceptions.Count < controller.targets.Count) {
            handleCalculations(controller.targets[interceptions.Count]);
            if (interceptions.Count == controller.targets.Count)
                controller.startSimulation(interceptions);
        }
        if (!controller.isShowingSimulation || interceptorCount >= interceptions.Count) return;
        displayRotation();
        if (controller.simulationTime >= interceptions[interceptorCount].rotationTime)
            launchInterceptor(interceptions[interceptorCount++]);
    }
}
