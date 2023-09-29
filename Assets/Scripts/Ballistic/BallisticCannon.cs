using System.Collections;
using System.Collections.Generic;
using UnityEngine;


class BallisticInterceptionResult {
    public bool isInercepted = false;
    public float delta;
    public Vector2 rotation;
    public float rotationTime = 0f;
    public Vector3 velocity;
    public float time;

    public BallisticInterceptionResult(float delta, float rotationX, float rotationY)
    {
        this.delta = delta;
        rotation = new Vector2(rotationX, rotationY);
    }
    public BallisticInterceptionResult(float delta, float rotationX, float rotationY, float rotationTime, Vector3 velocity, float time)
    {
        this.isInercepted = true;
        this.delta = delta;
        rotation = new Vector2(rotationX, rotationY);
        this.rotationTime = rotationTime;
        this.velocity = velocity;
        this.time = time;
    }
}


public class BallisticCannon : MonoBehaviour
{
    static public float projectileSpeed = 7;
    static public float projectileMass;
    static public Vector2 initialAngle = new Vector2(0, 0);
    static public Vector2 rotationSpeed = new Vector2(0.5f, 0.5f);

    public BallisticInterceptor interceptorPrefab;
    // public GameObject particlePrefab;
    public float stepFactor;
    public int iterationsPerFrame;

    private SceneController controller;
    private BallisticInterceptionResult calculationResult;
    private bool hasInterceptorBeenSent;
    
    void Start()
    {
        controller = GameObject.FindFirstObjectByType<SceneController>();
        calculationResult = new BallisticInterceptionResult(1, 0, 0);
        Vector3 angles = transform.rotation.eulerAngles;
        transform.localRotation = convertRotation(initialAngle);

        handleCalculations();
    }

    Vector3 getCannonDirection(float angleX, float angleY) {
        float alpha = angleX * Mathf.Deg2Rad;
        float beta = angleY * Mathf.Deg2Rad;
        float betaCos = Mathf.Cos(beta); // For perfomance
        return new Vector3(Mathf.Cos(alpha) * betaCos, Mathf.Sin(beta), Mathf.Sin(alpha) * betaCos);
    }
    Quaternion convertRotation(Vector2 rotation) {
        return Quaternion.Euler(-rotation.x, 0, rotation.y);
    }

    float? getTimeToCollideOnX(Vector3 velocity, float rotationTime)
    {
        // B and c coefficient are reffering to the standard form of a quadratic
        float b_coef = BallisticTarget.initialVelocity.x - velocity.x;
        float c_coef = BallisticTarget.startPosition.x - transform.position.x + velocity.x * rotationTime;
        float acceleration = BallisticTarget.initialVelocity.normalized.x * BallisticTarget.acceleration;
        if (acceleration == 0) // To avoid division by zero
            return -c_coef / b_coef;
        else {
            // Solving a quadratic equation x_{t0} + v_tt + (1/2)at^2 = x_{i0} + v_i(t - t_r)
            float temp = Mathf.Pow(b_coef, 2) - 2 * acceleration * c_coef; 
            if (temp < 0) return null; // Square root of a negative number

            float solution1 = -(b_coef - Mathf.Sqrt(temp)) / acceleration;
            if (solution1 >= 0) return solution1;
            float solution2 = -(b_coef + Mathf.Sqrt(temp)) / acceleration;
            if (solution2 >= 0) return solution2;
            
            return null; // No non-negative real solution
        }
    }

    bool isInterceptingAt(Vector3 point, float time)
    {
        float acceleration = BallisticTarget.initialVelocity.normalized.y * BallisticTarget.acceleration - SceneController.gravityAcceleration;
        float targetY = BallisticTarget.startPosition.y + BallisticTarget.initialVelocity.y * time + acceleration * Mathf.Pow(time, 2) / 2;
        acceleration = BallisticTarget.initialVelocity.normalized.z * BallisticTarget.acceleration;
        float targetZ = BallisticTarget.startPosition.z + BallisticTarget.initialVelocity.z * time + acceleration * Mathf.Pow(time, 2) / 2;
        return BallisticTarget.isInsideTheTarget(new Vector3(point.x, targetY, targetZ), point);
    }

    BallisticInterceptionResult findInterceptionPoint(float delta, Vector2 startRotation)
    {
        if (delta < 1) new BallisticInterceptionResult(delta, 0, 0);
        float angleX = startRotation.x;
        float angleY = startRotation.y;
        int iterations = 0;
        while (iterations < iterationsPerFrame) {
            if (angleX > 360f) {
                angleX = angleY = 0f;
                delta *= stepFactor;
            }
            while (angleY <= 180f && iterations++ < iterationsPerFrame) {
                float rotationTime = Mathf.Max(angleX / 360f / rotationSpeed.x, angleY / 360f / rotationSpeed.y);
                Vector3 velocity = getCannonDirection(angleX, angleY) * projectileSpeed;
                float? time = getTimeToCollideOnX(velocity, rotationTime);
                if (time == null) continue;
                float gravitationalDelta = SceneController.gravityAcceleration * Mathf.Pow((float)time - rotationTime, 2) / 2;
                Vector3 interceptorPosition = new Vector3(
                    transform.position.x + velocity.x * ((float)time - rotationTime),
                    transform.position.y + velocity.y * ((float)time - rotationTime) - gravitationalDelta,
                    transform.position.z + velocity.z * ((float)time - rotationTime)
                );
                if (isInterceptingAt(interceptorPosition, (float)time))
                    return new BallisticInterceptionResult(delta, angleX, angleY, rotationTime, velocity, (float)time);
                angleY += delta;
            }
            angleY = 0f;
            angleX += delta;
        }
        return new BallisticInterceptionResult(delta, angleX, angleY);
    }

    public void launchInterceptor()
    {
        BallisticInterceptor interceptor = Instantiate(interceptorPrefab, transform.position, Quaternion.identity);
        interceptor.velocity = calculationResult.velocity;
        interceptor.mass = projectileMass;
        interceptor.launchOffset = calculationResult.rotationTime;
        // GameObject particles = Instantiate(particlePrefab, transform.position + transform.up * -transform.localScale.y / 2, Quaternion.identity);
        // particles.transform.SetParent(gameObject.transform);
        // particles.transform.localRotation = Quaternion.Euler(90, -90, 0);
    }

    void handleCalculations()
    {
        if (calculationResult.isInercepted) return;
        calculationResult = findInterceptionPoint(calculationResult.delta, calculationResult.rotation);
        if (calculationResult.isInercepted) controller.startSimulation(calculationResult.time);
    }

    void Update()
    {
        handleCalculations();
        if (!controller.isShowingSimulation) return;
        if (controller.simulationTime < calculationResult.rotationTime) {
            // transform.localRotation = Quaternion.Lerp(
            //     convertRotation(initialAngle),
            //     convertRotation(calculationResult.rotation),
            //     controller.simulationTime / calculationResult.rotationTime
            // );
        } else if (!hasInterceptorBeenSent) {
            hasInterceptorBeenSent = true;
            launchInterceptor();
            // transform.localRotation = convertRotation(calculationResult.rotation);
        }
    }
}
