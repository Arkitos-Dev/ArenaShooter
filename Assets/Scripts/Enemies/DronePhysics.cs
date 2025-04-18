using UnityEngine;

public class DronePhysics : MonoBehaviour
{
    [SerializeField] private Transform[] propellers;
    [SerializeField] private float targetAltitude = 10f;
    
    public float thrust = 10f;
    public float pGain = 1.0f, iGain = 0.1f, dGain = 0.5f;
    private float integral, lastError;

    private Rigidbody rb;
    private Drone drone;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        drone = GetComponent<Drone>();
    }

    void FixedUpdate()
    {
        float currentAltitude = GetCurrentAltitude();
        float altitudeError = targetAltitude - currentAltitude;
        float pidOutput = CalculatePID(altitudeError, Time.fixedDeltaTime);
        ApplyThrust(pidOutput);
    }

    void ApplyThrust(float pidOutput)
    {
        float adjustedThrust = thrust + pidOutput;
        foreach (Transform propeller in propellers)
        {
            rb.AddForceAtPosition(Vector3.up * adjustedThrust, propeller.position);
        }
    }
    
    float CalculatePID(float error, float deltaTime)
    {
        integral += error * deltaTime;
        float derivative = (error - lastError) / deltaTime;
        lastError = error;
        return pGain * error + iGain * integral + dGain * derivative;
    }
    
    float GetCurrentAltitude()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 20f, drone.layerMask))
        {
            return hit.distance;
        }
        return 0f;
    }
}

